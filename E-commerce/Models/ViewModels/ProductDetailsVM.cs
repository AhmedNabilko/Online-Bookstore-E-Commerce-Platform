using E_commerceEntity.Entity;
using System.Collections.Generic;

namespace E_commerce.Models.ViewModels
{
    public class ProductDetailsVM
    {
        public Product Product { get; set; }
        public int Count { get; set; } = 1;
        public List<Category> CategoryBreadcrumb { get; set; } = new List<Category>();
    }
}
