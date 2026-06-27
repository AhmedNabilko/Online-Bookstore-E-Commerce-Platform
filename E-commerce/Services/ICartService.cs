using E_commerce.Models.ViewModels;
using System.Threading.Tasks;

namespace E_commerce.Services
{
    public interface ICartService
    {
        Task<CartVM> GetCartViewModelAsync();
        Task AddToCartAsync(int productId, int count);
        Task UpdateItemQuantityAsync(int cartItemId, int newCount);
        Task RemoveItemAsync(int cartItemId);
        Task MergeGuestCartAsync(string userId);
    }
}
