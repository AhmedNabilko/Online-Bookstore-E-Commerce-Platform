using Microsoft.AspNetCore.Mvc;
using E_commerce.Services;
using E_commerce.Models.ViewModels;
using System.Threading.Tasks;

namespace E_commerce.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        public async Task<IActionResult> Index()
        {
            var cartViewModel = await _cartService.GetCartViewModelAsync();
            return View(cartViewModel);
        }

        // POST: /Cart/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int count)
        {
            if (count <= 0)
            {
                TempData["Error"] = "Please specify a valid quantity.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _cartService.AddToCartAsync(productId, count);
                TempData["Success"] = "Item added to cart.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int count)
        {
            if (count <= 0)
            {

                await _cartService.RemoveItemAsync(cartItemId);
                TempData["Success"] = "Item removed from cart.";
            }
            else
            {
                try
                {
                    await _cartService.UpdateItemQuantityAsync(cartItemId, count);
                    TempData["Success"] = "Cart updated.";
                }
                catch (System.Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            try
            {
                await _cartService.RemoveItemAsync(cartItemId);
                TempData["Success"] = "Item removed from cart.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
