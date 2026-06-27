// 2. Create Models/ViewModels/OrderDetailsVM.cs
using E_commerceEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E_commerce.Models.ViewModels
{
    public class OrderDetailsItemVM
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        public OrderDetailsItemVM(Order_Item item)
        {
            ProductName = item.Product?.Name;
            Quantity = item.Quantity;
            UnitPrice = item.UnitPrice;
            LineTotal = item.LineTotal;
        }
    }

    public class OrderDetailsVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public AddressVM ShippingAddress { get; set; }
        public List<OrderDetailsItemVM> Items { get; set; }

        // Smart Constructor: Maps Entities -> VM entirely
        public OrderDetailsVM(Order order, List<Order_Item> items)
        {
            OrderId = order.OrderId;
            OrderNumber = order.OrderNumber;
            OrderDate = order.OrderDate;
            Status = order.Status;
            TotalAmount = order.TotalAmount;
            CustomerName = order.User?.FullName;
            CustomerEmail = order.User?.Email;
            ShippingAddress = new AddressVM(order.Address);
            Items = items.Select(i => new OrderDetailsItemVM(i)).ToList();
        }
    }
}