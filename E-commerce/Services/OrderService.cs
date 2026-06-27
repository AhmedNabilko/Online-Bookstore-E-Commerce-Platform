// File: Services/OrderService.cs
using E_commerce.Models.ViewModels;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public interface IOrderService
    {
        Task<OrderResult> PlaceOrderAsync(string userId, CheckoutVM model);
        OrderDetailsVM GetOrderDetails(int orderId, string userId);
    }

    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderResult> PlaceOrderAsync(string userId, CheckoutVM model)
        {
            var result = new OrderResult();

            var cart = _unitOfWork.ShoppingCart.FindAll(c => c.UserId == userId, c => c.Items).FirstOrDefault();
            if (cart == null || !cart.Items.Any())
            {
                result.AdjustmentMessages.Add("Your cart is empty.");
                return result;
            }

            foreach (var item in cart.Items)
            {
                item.Product = _unitOfWork.Product.GetById(item.ProductId);
            }

            decimal calculatedTotal = 0;
            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    result.AdjustmentMessages.Add("A product in your cart is no longer available.");
                    continue;
                }

                if (item.Product.StockQuantity < item.Quantity)
                {
                    result.AdjustmentMessages.Add($"Insufficient stock for '{item.Product.Name}'. Only {item.Product.StockQuantity} available.");
                }

                calculatedTotal += (item.Product.Price * item.Quantity);
            }

            if (result.AdjustmentMessages.Any())
            {
                return result;
            }

            var address = _unitOfWork.Address.FindAll(a => a.UserId == userId).FirstOrDefault();
            if (address == null)
            {
                address = model.ShippingAddress.ToEntity(userId);
                _unitOfWork.Address.Add(address);
            }
            else
            {
                address.Street = model.ShippingAddress.Street;
                address.City = model.ShippingAddress.City;
                address.Zip = model.ShippingAddress.Zip;
                address.Country = model.ShippingAddress.Country;
                _unitOfWork.Address.Update(address);
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                OrderNumber = GenerateOrderNumber(),
                TotalAmount = calculatedTotal,
                Address = address,
                OrderItems = new List<Order_Item>()
            };

            foreach (var cartItem in cart.Items.ToList())
            {
                order.OrderItems.Add(new Order_Item
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price,
                    LineTotal = cartItem.Product.Price * cartItem.Quantity
                });

                cartItem.Product.StockQuantity -= cartItem.Quantity;
                _unitOfWork.Product.Update(cartItem.Product);

                _unitOfWork.Cart_Item.Delete(cartItem);
            }

            _unitOfWork.Order.Add(order);

            // The Single Atomic Save
            _unitOfWork.Save();

            result.Success = true;
            result.OrderId = order.OrderId;
            result.OrderNumber = order.OrderNumber;
            return result;
        }

        public OrderDetailsVM GetOrderDetails(int orderId, string userId)
        {
            var order = _unitOfWork.Order.FindAll(o => o.OrderId == orderId && o.UserId == userId, o => o.Address).FirstOrDefault();
            if (order == null) return null;

            var orderItems = _unitOfWork.Order_Item.FindAll(oi => oi.OrderId == orderId, oi => oi.Product).ToList();


            return new OrderDetailsVM(order, orderItems);
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }
    }
}