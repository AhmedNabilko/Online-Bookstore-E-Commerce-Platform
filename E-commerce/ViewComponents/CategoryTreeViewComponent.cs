using Microsoft.AspNetCore.Mvc;
using E_commerceEntity.Entity;
using System.Collections.Generic;
using System.Linq;

namespace E_commerce.ViewComponents
{
    [ViewComponent(Name = "CategoryTree")]
    public class CategoryTreeViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<Category> categories, int? currentCategoryId, int level = 0)
        {
            ViewBag.CurrentCategoryId = currentCategoryId;
            ViewBag.Level = level;
            return View(categories);
        }
    }
}