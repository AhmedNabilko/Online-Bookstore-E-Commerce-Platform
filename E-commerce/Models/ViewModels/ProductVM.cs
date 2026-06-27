using E_commerceEntity.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [MaxLength(100, ErrorMessage = "Product Name Must be Less than 100 Characters."), MinLength(3, ErrorMessage = "Product Name Must be Greater Than 3  Characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "SKU is required")]
        [Remote(action: "CheckSku", controller: "Products", areaName: "Admin", AdditionalFields = "ProductId")]
        public string SKU { get; set; }

        [Required]
        [Range(0.01, Int32.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a Category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public string CategoryName { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }



        public ProductVM()
        {
        }


        public ProductVM(Product product)
        {


            ProductId = product.ProductId;
            Name = product.Name;
            SKU = product.SKU;
            Price = product.Price;
            StockQuantity = product.StockQuantity;
            IsActive = product.IsActive;
            CategoryId = product.CategoryId;
            CategoryName = product.Category?.Name;
        }



        public Product ToProduct()
        {
            return new Product
            {

                Name = this.Name,
                SKU = this.SKU,
                Price = this.Price,
                StockQuantity = this.StockQuantity,
                IsActive = this.IsActive,
                CategoryId = this.CategoryId
            };
        }

        public Product ToProduct(int ProductId)
        {
            Product product = ToProduct();
            product.ProductId = ProductId;
            return product;
        }

        public void UpdateProduct(Product product)
        {
            product.Name = this.Name;
            product.SKU = this.SKU;
            product.Price = this.Price;
            product.StockQuantity = this.StockQuantity;
            product.IsActive = this.IsActive;
            product.CategoryId = this.CategoryId;
        }

    }

}