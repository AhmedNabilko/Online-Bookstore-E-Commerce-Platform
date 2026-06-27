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
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? pageNumber, string search)
        {
            int pageSize = 20;
            int pageIndex = pageNumber ?? 1;

            var products = unitOfWork.Product.GetPaginated(
                pageNumber: pageIndex,
                pageSize: pageSize,
                condition: p => string.IsNullOrEmpty(search) || p.Name.Contains(search),
                preloads: p => p.Category
            );

            var productVMs = products.Select(p => new ProductVM(p)).ToList();

            ViewData["CurrentSearch"] = search;
            ViewData["PageNumber"] = pageIndex;
            ViewData["TotalCount"] = unitOfWork.Product.GetTotalCount(p => string.IsNullOrEmpty(search) || p.Name.Contains(search));
            ViewData["PageSize"] = pageSize;

            return View(productVMs);
        }


        [HttpGet]
        public IActionResult CheckSku(string SKU, int ProductId)
        {
            var isSkuExists = unitOfWork.Product.FindAll(p => p.SKU == SKU && p.ProductId != ProductId).Any();
            if (isSkuExists)
            {
                return Json($"SKU {SKU} is already in use.");
            }
            return Json(true);
        }



        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM();
            var categories = unitOfWork.Category.GetAll();

            productVM.CategoryList = categories.Select(c => new SelectListItem(text: c.Name, value: c.CategoryId.ToString()));

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Create(ProductVM productVM)
        {
            // Unique SKU validation
            if (unitOfWork.Product.FindAll(p => p.SKU == productVM.SKU).Any())
            {
                ModelState.AddModelError("SKU", "Product SKU must be unique.");
            }

            if (ModelState.IsValid)
            {
                Product newProduct = productVM.ToProduct();

                unitOfWork.Product.Add(newProduct);
                unitOfWork.Save();

                TempData["success"] = "Product created successfully!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = unitOfWork.Category.GetAll()
                    .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });

                return View(productVM);
            }
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            { return NotFound(); }

            var product = unitOfWork.Product.GetById(id.Value);
            if (product == null)
            { return NotFound(); }


            ProductVM productVM = new ProductVM(product);

            productVM.CategoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });

            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM)
        {
            // Unique SKU validation (excluding current product)
            if (unitOfWork.Product.FindAll(p => p.SKU == productVM.SKU && p.ProductId != productVM.ProductId).Any())
            {
                ModelState.AddModelError("SKU", "Product SKU must be unique.");
            }

            if (ModelState.IsValid)
            {
                var productToUpdate = unitOfWork.Product.GetById(productVM.ProductId);
                if (productToUpdate == null) return NotFound();

                // Use the dedicated update method within ProductVM to modify the tracked entity
                productVM.UpdateProduct(productToUpdate);

                unitOfWork.Product.Update(productToUpdate);
                unitOfWork.Save();

                TempData["success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            productVM.CategoryList = unitOfWork.Category.GetAll()
                .Select(c => new SelectListItem { Text = c.Name, Value = c.CategoryId.ToString() });

            return View(productVM);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            { return NotFound(); }

            var product = unitOfWork.Product.FindAll(p => p.ProductId == id.Value, p => p.Category).FirstOrDefault();
            if (product == null)
            { return NotFound(); }

            // Wrap the retrieved database entity in a ViewModel before passing it to the View()
            ProductVM productVM = new ProductVM(product);

            return View(productVM);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            var product = unitOfWork.Product.GetById(id);
            if (product == null)
            { return NotFound(); }


            var orderItems = unitOfWork.Order_Item.FindAll(oi => oi.ProductId == id);

            if (orderItems.Any())
            {
                TempData["error"] = "Cannot delete: Customers have already ordered this product. Please set 'IsActive' to false instead of deleting it.";
                return RedirectToAction(nameof(Index));
            }

            unitOfWork.Product.Delete(product);
            unitOfWork.Save();

            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}