using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Models.ViewModels
{
    public class CheckoutItemVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxAvailable { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
        public CheckoutItemVM() { }

        public CheckoutItemVM(CartItemVM cartItem)
        {
            ProductId = cartItem.ProductId;
            ProductName = cartItem.ProductName;
            UnitPrice = cartItem.Price;
            Quantity = cartItem.Quantity;
            MaxAvailable = cartItem.AvailableStock;

        }
    }
}
