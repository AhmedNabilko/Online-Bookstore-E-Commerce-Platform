using E_commerce.Models.ViewModels;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace E_commerce.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CatalogController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private List<int> GetCategoryAndSubCategoryIds(int? categoryId)
        {
            var allCategories = unitOfWork.Category.GetAll().ToList();
            var result = new List<int>();

            if (!categoryId.HasValue)
                return result;

            void AddCategoryAndSubCategories(int catId)
            {
                result.Add(catId);
                var subCategories = allCategories.Where(c => c.ParentCategoryId == catId);
                foreach (var sub in subCategories)
                {
                    AddCategoryAndSubCategories(sub.CategoryId);
                }
            }

            AddCategoryAndSubCategories(categoryId.Value);
            return result;
        }

        public IActionResult Index(int? categoryId, string? q, int page = 1)
        {
            int pageSize = 12;

            var allCategories = unitOfWork.Category.GetAll().ToList();
            var rootCategories = allCategories.Where(c => c.ParentCategoryId == null).ToList();

            foreach (var cat in rootCategories)
            {
                LoadSubCategories(cat, allCategories);
            }

            var productsQuery = unitOfWork.Product.FindAll(p => p.IsActive);

            if (categoryId.HasValue)
            {
                var categoryIds = GetCategoryAndSubCategoryIds(categoryId);
                productsQuery = productsQuery.Where(p => categoryIds.Contains(p.CategoryId));
            }

            if (!string.IsNullOrEmpty(q))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(q));
            }

            int totalProducts = productsQuery.Count();
            int totalPages = (int)System.Math.Ceiling((double)totalProducts / pageSize);

            var products = productsQuery
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ProductListVM productListVM = new ProductListVM
            {
                Products = products,
                Categories = rootCategories,
                CurrentCategoryId = categoryId,
                SearchQuery = q,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(productListVM);
        }

        private void LoadSubCategories(Category parent, List<Category> allCategories)
        {
            parent.SubCategories = allCategories
                .Where(c => c.ParentCategoryId == parent.CategoryId)
                .ToList();

            foreach (var sub in parent.SubCategories ?? new List<Category>())
            {
                LoadSubCategories(sub, allCategories);
            }
        }

        public IActionResult Details(int id)
        {
            var product = unitOfWork.Product.FindAll(p => p.ProductId == id, p => p.Category).FirstOrDefault();

            if (product == null || !product.IsActive)
            {
                return NotFound();
            }

            var breadcrumb = new List<Category>();
            var allCategories = unitOfWork.Category.GetAll().ToList();
            BuildBreadcrumb(product.CategoryId, allCategories, breadcrumb);
            breadcrumb.Reverse();

            ProductDetailsVM productDetailsVM = new ProductDetailsVM
            {
                Product = product,
                Count = 1,
                CategoryBreadcrumb = breadcrumb
            };

            return View(productDetailsVM);
        }

        private void BuildBreadcrumb(int categoryId, List<Category> allCategories, List<Category> breadcrumb)
        {
            var category = allCategories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category != null)
            {
                breadcrumb.Add(category);
                if (category.ParentCategoryId.HasValue)
                {
                    BuildBreadcrumb(category.ParentCategoryId.Value, allCategories, breadcrumb);
                }
            }
        }
    }
}
