using E_commerce.Models.ViewModels;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace E_commerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller

    {
        private readonly IUnitOfWork unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        public IActionResult Index(string? search)
        {
            var categories = unitOfWork.Category.FindAll(c => string.IsNullOrEmpty(search) || c.Name.Contains(search), c => c.ParentCategory);
            ViewData["CurrentSearch"] = search;
            return View(categories);
        }


        public IActionResult Create()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.ParentCategoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });

            return View(categoryVM);
        }


        [HttpGet]
        public IActionResult CheckName(string name, int? CategoryId)
        {
            bool exists = unitOfWork.Category.FindAll(u => u.Name.ToLower() == name.ToLower() && (CategoryId == null || u.CategoryId != CategoryId)).Any();
            return Json(!exists);
        }


        [HttpPost]
        public IActionResult Create(CategoryVM categoryVM)
        {
            // Uniqueness validation
            if (unitOfWork.Category.FindAll(u => u.Name.ToLower() == categoryVM.Name.ToLower()).Any())
            {
                ModelState.AddModelError("Name", "Category name already exists.");
            }

            if (ModelState.IsValid)
            {
                Category newCategory = categoryVM.ToCategory();

                unitOfWork.Category.Add(newCategory);
                unitOfWork.Save();

                TempData["success"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }


            categoryVM.ParentCategoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });

            return View(categoryVM);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            { return NotFound(); }

            var category = unitOfWork.Category.GetById(id.Value);

            if (category == null)
            { return NotFound(); }

            CategoryVM categoryVM = new CategoryVM(category);



            categoryVM.ParentCategoryList = unitOfWork.Category.GetAll()
                .Where(c => c.CategoryId != id.Value)
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.CategoryId.ToString()
                });

            return View(categoryVM);
        }


        [HttpPost]
        public IActionResult Edit(CategoryVM categoryVM)
        {
            // Uniqueness validation
            if (unitOfWork.Category.FindAll(u => u.Name.ToLower() == categoryVM.Name.ToLower() && u.CategoryId != categoryVM.CategoryId).Any())
            {
                ModelState.AddModelError("Name", "Category name already exists.");
            }

            if (ModelState.IsValid)
            {
                var categoryToUpdate = unitOfWork.Category.GetById(categoryVM.CategoryId);
                if (categoryToUpdate == null)
                { return NotFound(); }

                categoryVM.UpdateCategory(categoryToUpdate);

                unitOfWork.Category.Update(categoryToUpdate);
                unitOfWork.Save();

                TempData["success"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {

                categoryVM.ParentCategoryList = unitOfWork.Category.GetAll()
                    .Where(c => c.CategoryId != categoryVM.CategoryId)
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });


                return View(categoryVM);
            }

        }



        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            { return NotFound(); }

            var category = unitOfWork.Category.FindAll(c => c.CategoryId == id.Value, c => c.ParentCategory).FirstOrDefault();

            if (category == null)
            { return NotFound(); }

            CategoryVM categoryVM = new CategoryVM(category);
            ViewBag.ParentName = category.ParentCategory?.Name;

            return View(categoryVM);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = unitOfWork.Category.GetById(id);
            if (category == null) { return NotFound(); }


            var subCategories = unitOfWork.Category.FindAll(c => c.ParentCategoryId == id);

            var products = unitOfWork.Product.FindAll(p => p.CategoryId == id);

            if (subCategories.Any() || products.Any())
            {
                TempData["error"] = "Cannot delete: This category contains sub-categories or products.";
                return RedirectToAction(nameof(Index));
            }

            unitOfWork.Category.Delete(category);
            unitOfWork.Save();

            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}