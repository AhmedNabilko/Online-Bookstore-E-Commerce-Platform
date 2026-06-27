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
    public class OrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int? pageNumber)
        {
            int pageSize = 20;
            int pageIndex = pageNumber ?? 1;

            var orders = _unitOfWork.Order.GetPaginated(
                pageNumber: pageIndex,
                pageSize: pageSize,
                condition: o => true,
                preloads: o => o.User
            );


            var orderVMs = orders.Select(o => new AdminOrderListVM(o)).ToList();

            var totalCount = _unitOfWork.Order.GetTotalCount(o => true);

            ViewData["PageNumber"] = pageIndex;
            ViewData["TotalCount"] = totalCount;
            ViewData["PageSize"] = pageSize;

            ViewBag.StatusList = Enum.GetValues<OrderStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                });
            return View(orderVMs);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var order = _unitOfWork.Order.FindAll(
                o => o.OrderId == id.Value,
                o => o.User,
                o => o.Address
            ).FirstOrDefault();

            if (order == null) return NotFound();

            var orderItems = _unitOfWork.Order_Item.FindAll(
                oi => oi.OrderId == id.Value,
                oi => oi.Product
            ).ToList();

            var viewModel = new OrderDetailsVM(order, orderItems);


            ViewBag.StatusList = Enum.GetValues<OrderStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                });

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int orderId, OrderStatus status)
        {
            var existingOrder = _unitOfWork.Order.GetById(orderId);

            if (existingOrder == null) return NotFound();

            existingOrder.Status = status;
            _unitOfWork.Order.Update(existingOrder);
            _unitOfWork.Save();

            TempData["success"] = "Order status updated successfully!";


            return RedirectToAction(nameof(Details), new { id = orderId });
        }
    }
}