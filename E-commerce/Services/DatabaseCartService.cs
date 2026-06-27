using E_commerce.Models.ViewModels;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class DatabaseCartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DatabaseCartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ShoppingCart GetOrCreateUserCart(string userId)
        {
            var cart = _unitOfWork.ShoppingCart.FindAll(c => c.UserId == userId, c => c.Items).FirstOrDefault();
            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId, Items = new HashSet<Cart_Item>() };
                _unitOfWork.ShoppingCart.Add(cart);
                _unitOfWork.Save();
            }

            // Populate products
            foreach (var item in cart.Items)
                item.Product = _unitOfWork.Product.GetById(item.ProductId);

            return cart;
        }

        public async Task<CartVM> GetCartViewModelAsync(string userId)
        {
            var cart = GetOrCreateUserCart(userId);
            var items = cart.Items.Select(i => new CartItemVM(i)).ToList();
            return new CartVM { CartItems = items, OrderTotal = items.Sum(i => i.LineTotal) };
        }

        public async Task AddToCartAsync(string userId, int productId, int count)
        {
            var cart = GetOrCreateUserCart(userId);
            var product = _unitOfWork.Product.GetById(productId);
            if (product == null) return;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = Math.Min(existingItem.Quantity + count, product.StockQuantity);
                _unitOfWork.Cart_Item.Update(existingItem);
            }
            else
            {
                _unitOfWork.Cart_Item.Add(new Cart_Item
                {
                    ShoppingCartId = cart.Id,
                    ProductId = productId,
                    Quantity = Math.Min(count, product.StockQuantity)
                });
            }
            _unitOfWork.Save();
            await Task.CompletedTask;
        }

        public async Task UpdateItemQuantityAsync(string userId, int cartItemId, int newCount)
        {
            var item = _unitOfWork.Cart_Item.GetById(cartItemId);
            if (item == null) return;

            if (newCount <= 0) _unitOfWork.Cart_Item.Delete(item);
            else
            {
                var product = _unitOfWork.Product.GetById(item.ProductId);
                item.Quantity = product != null ? Math.Min(newCount, product.StockQuantity) : newCount;
                _unitOfWork.Cart_Item.Update(item);
            }
            _unitOfWork.Save();
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync(string userId, int cartItemId)
        {
            var item = _unitOfWork.Cart_Item.GetById(cartItemId);
            if (item != null)
            {
                _unitOfWork.Cart_Item.Delete(item);
                _unitOfWork.Save();
            }
            await Task.CompletedTask;
        }

        // Dedicated method to handle merging session items into the DB
        public async Task MergeItemsAsync(string userId, IEnumerable<Cart_Item> sessionItems)
        {
            var cart = GetOrCreateUserCart(userId);
            foreach (var guestItem in sessionItems)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == guestItem.ProductId);
                var product = _unitOfWork.Product.GetById(guestItem.ProductId);
                int stockLimit = product?.StockQuantity ?? int.MaxValue;

                if (existingItem != null)
                {
                    existingItem.Quantity = Math.Min(existingItem.Quantity + guestItem.Quantity, stockLimit);
                    _unitOfWork.Cart_Item.Update(existingItem);
                }
                else
                {
                    _unitOfWork.Cart_Item.Add(new Cart_Item
                    {
                        ShoppingCartId = cart.Id,
                        ProductId = guestItem.ProductId,
                        Quantity = Math.Min(guestItem.Quantity, stockLimit)
                    });
                }
            }
            _unitOfWork.Save();
            await Task.CompletedTask;
        }
    }
}