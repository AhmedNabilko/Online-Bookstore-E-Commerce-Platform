using E_commerceEntity.Entity;
using System.Collections.Generic;

namespace E_commerce.Models.ViewModels
{
    public class CartVM
    {
        public IEnumerable<CartItemVM> CartItems { get; set; }
        public decimal OrderTotal { get; set; }
    }

    public class CartItemVM
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public bool IsAvailable { get; set; }
        public string StockWarning { get; set; }
        public int AvailableStock { get; set; }
        public bool IsAtMaxStock => Quantity >= AvailableStock;

        public CartItemVM()
        {
        }

        public CartItemVM(Cart_Item item)
        {
            Id = item.Id;
            ProductId = item.ProductId;
            ProductName = item.Product?.Name;
            Price = item.Product?.Price ?? 0;
            Quantity = item.Quantity;
            LineTotal = Price * Quantity;
            AvailableStock = item.Product?.StockQuantity ?? 0;
            IsAvailable = AvailableStock > 0;
            StockWarning = GetStockWarning(item.Product);
        }

        private static string GetStockWarning(Product product)
        {
            if (product == null)
                return "Product no longer available";
            
            if (product.StockQuantity == 0)
                return "Out of stock";
            
            if (product.StockQuantity < 5)
                return $"Only {product.StockQuantity} left in stock";
            
            return string.Empty;
        }
    }
}
