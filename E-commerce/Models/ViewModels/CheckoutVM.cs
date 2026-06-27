using E_commerceEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_commerce.Models.ViewModels
{
    public class CheckoutVM
    {
        public string UserId { get; set; }
        public AddressVM ShippingAddress { get; set; }
        public List<CheckoutItemVM> CartItems { get; set; } = new List<CheckoutItemVM>();
        public decimal TotalAmount => CartItems.Sum(x => x.LineTotal);
        public decimal FlatRateShipping { get; set; } = 10.00m;
        public decimal GrandTotal => TotalAmount + FlatRateShipping;



        public CheckoutVM() { }

        public CheckoutVM(CartVM cart, string userId)
        {
            UserId = userId;
            ShippingAddress = new AddressVM();
            RefreshFromCart(cart);
        }

        public void RefreshFromCart(CartVM cart)
        {

            CartItems = cart.CartItems.Select(item => new CheckoutItemVM(item)).ToList();
        }
    }
}
