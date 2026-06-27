// File: Controllers/OrdersController.cs
using E_commerce.Models.ViewModels;
using E_commerce.Services;
using E_commerceEntity.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace E_commerce.Controllers
{
    //[Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly UserManager<APP_User> _userManager;

        public OrdersController(IOrderService orderService, ICartService cartService, UserManager<APP_User> userManager)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cartVM = await _cartService.GetCartViewModelAsync();

            if (cartVM.CartItems == null || !cartVM.CartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var checkoutVM = new CheckoutVM(cartVM, userId);

            return View(checkoutVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutVM model)
        {
            if (!ModelState.IsValid)
            {
                var cartVM = await _cartService.GetCartViewModelAsync();
                model.RefreshFromCart(cartVM);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _orderService.PlaceOrderAsync(userId, model);

            if (!result.Success)
            {
                foreach (var error in result.AdjustmentMessages)
                {
                    ModelState.AddModelError("", error);
                }

                var cartVM = await _cartService.GetCartViewModelAsync();
                model.RefreshFromCart(cartVM);
                return View(model);
            }

            TempData["Success"] = $"Order {result.OrderNumber} placed successfully!";
            return RedirectToAction(nameof(Details), new { id = result.OrderId });
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var viewModel = _orderService.GetOrderDetails(id, userId);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }
    }
}