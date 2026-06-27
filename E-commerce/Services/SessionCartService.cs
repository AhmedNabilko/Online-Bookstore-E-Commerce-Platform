using E_commerce.Models.ViewModels;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class SessionCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork; // Needed to fetch prices/stock limits

        public SessionCartService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        private string GetSessionKey()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            string sessionId = session?.GetString("_CartId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                session?.SetString("_CartId", sessionId);
            }
            return $"GuestCart_{sessionId}";
        }

        public ShoppingCart GetRawCart()
        {
            var cartJson = _httpContextAccessor.HttpContext?.Session.GetString(GetSessionKey());
            if (string.IsNullOrEmpty(cartJson)) return new ShoppingCart { Items = new HashSet<Cart_Item>() };

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var cart = JsonSerializer.Deserialize<ShoppingCart>(cartJson, options);

                // Populate product details for the UI
                if (cart != null)
                {
                    foreach (var item in cart.Items)
                        item.Product = _unitOfWork.Product.GetById(item.ProductId);
                }
                return cart ?? new ShoppingCart { Items = new HashSet<Cart_Item>() };
            }
            catch { return new ShoppingCart { Items = new HashSet<Cart_Item>() }; }
        }

        public void SaveCart(ShoppingCart cart)
        {
            foreach (var item in cart.Items) { item.Product = null; item.ShoppingCart = null; }
            var cartJson = JsonSerializer.Serialize(cart);
            _httpContextAccessor.HttpContext?.Session.SetString(GetSessionKey(), cartJson);
        }

        public void ClearCart() => _httpContextAccessor.HttpContext?.Session.Remove(GetSessionKey());

        public async Task<CartVM> GetCartViewModelAsync()
        {
            var cart = GetRawCart();
            var items = cart.Items.Select(i => new CartItemVM(i)).ToList();
            return new CartVM { CartItems = items, OrderTotal = items.Sum(i => i.LineTotal) };
        }

        public async Task AddToCartAsync(int productId, int count)
        {
            var cart = GetRawCart();
            var product = _unitOfWork.Product.GetById(productId);
            if (product == null) return;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity = Math.Min(existingItem.Quantity + count, product.StockQuantity);
            }
            else
            {
                cart.Items.Add(new Cart_Item
                {
                    Id = cart.Items.Any() ? cart.Items.Max(i => i.Id) + 1 : 1, // Fake ID for session
                    ProductId = productId,
                    Quantity = Math.Min(count, product.StockQuantity)
                });
            }
            SaveCart(cart);
            await Task.CompletedTask;
        }

        public async Task UpdateItemQuantityAsync(int cartItemId, int newCount)
        {
            var cart = GetRawCart();
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item == null) return;

            if (newCount <= 0) cart.Items.Remove(item);
            else
            {
                var product = _unitOfWork.Product.GetById(item.ProductId);
                item.Quantity = product != null ? Math.Min(newCount, product.StockQuantity) : newCount;
            }
            SaveCart(cart);
            await Task.CompletedTask;
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            var cart = GetRawCart();
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
            }
            await Task.CompletedTask;
        }
    }
}