using E_commerce.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public class CartService : ICartService
    {
        private readonly SessionCartService _sessionCart;
        private readonly DatabaseCartService _dbCart;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(SessionCartService sessionCart, DatabaseCartService dbCart, IHttpContextAccessor httpContextAccessor)
        {
            _sessionCart = sessionCart;
            _dbCart = dbCart;
            _httpContextAccessor = httpContextAccessor;
        }

        private bool IsAuthenticated() => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        private string GetUserId() => _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        public async Task<CartVM> GetCartViewModelAsync()
        {
            if (IsAuthenticated()) return await _dbCart.GetCartViewModelAsync(GetUserId());
            return await _sessionCart.GetCartViewModelAsync();
        }

        public async Task AddToCartAsync(int productId, int count)
        {
            if (IsAuthenticated()) await _dbCart.AddToCartAsync(GetUserId(), productId, count);
            else await _sessionCart.AddToCartAsync(productId, count);
        }

        public async Task UpdateItemQuantityAsync(int cartItemId, int newCount)
        {
            if (IsAuthenticated()) await _dbCart.UpdateItemQuantityAsync(GetUserId(), cartItemId, newCount);
            else await _sessionCart.UpdateItemQuantityAsync(cartItemId, newCount);
        }

        public async Task RemoveItemAsync(int cartItemId)
        {
            if (IsAuthenticated()) await _dbCart.RemoveItemAsync(GetUserId(), cartItemId);
            else await _sessionCart.RemoveItemAsync(cartItemId);
        }

        public async Task MergeGuestCartAsync(string userId)
        {
            var rawSessionCart = _sessionCart.GetRawCart();

            // If the guest had items, merge them into the database, then clear the session
            if (rawSessionCart != null && rawSessionCart.Items.Count > 0)
            {
                await _dbCart.MergeItemsAsync(userId, rawSessionCart.Items);
                _sessionCart.ClearCart();
            }
        }
    }
}