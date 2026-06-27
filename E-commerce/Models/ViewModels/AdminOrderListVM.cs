using E_commerceEntity.Entity;
using System;

namespace E_commerce.Models.ViewModels
{
    public class AdminOrderListVM
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }

        public AdminOrderListVM() { }


        public AdminOrderListVM(Order order)
        {
            OrderId = order.OrderId;
            OrderNumber = order.OrderNumber;
            OrderDate = order.OrderDate;
            TotalAmount = order.TotalAmount;
            Status = order.Status;


            CustomerName = order.User?.FullName ?? "Guest/Unknown";
            CustomerEmail = order.User?.Email ?? "N/A";
        }
    }
}
