using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace E_commerceEntity.Entity
{
    public class Product : IEntity
    {
        public int ProductId { get; set; }


        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string SKU { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<Order_Item> OrderItems { get; set; }
        public ICollection<Cart_Item> CartItems { get; set; }


    }
}
