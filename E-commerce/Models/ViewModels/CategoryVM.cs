using E_commerceEntity.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace E_commerce.Models.ViewModels
{
    public class CategoryVM
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(100, ErrorMessage = "Category Name Must be Less than 100 Characters."), MinLength(3, ErrorMessage = "Category Name Must be Greater Than 3 Characters.")]
        [Remote(action: "CheckName", controller: "Categories", areaName: "Admin", AdditionalFields = nameof(CategoryId), ErrorMessage = "Category name already exists.")]
        public string Name { get; set; }

        [Display(Name = "Parent Category")]
        public int? ParentCategoryId { get; set; }


        [ValidateNever]
        public IEnumerable<SelectListItem> ParentCategoryList { get; set; }

        public Category ToCategory(int CategoryId)
        {
            Category category = ToCategory();
            category.CategoryId = CategoryId;
            return category;

        }
        public Category ToCategory()
        {
            return new Category
            {
                Name = Name,
                ParentCategoryId = ParentCategoryId
            };
        }
        public CategoryVM() { }
        public CategoryVM(Category category)
        {
            Name = category.Name;
            CategoryId = category.CategoryId;
            ParentCategoryId = category.ParentCategoryId;
        }
        public void UpdateCategory(Category category)
        {
            category.Name = this.Name;
            category.ParentCategoryId = this.ParentCategoryId;
        }

    }
}