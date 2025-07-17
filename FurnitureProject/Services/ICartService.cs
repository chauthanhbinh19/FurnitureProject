using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface ICartService
    {
        Task<(bool Success, string? Message)> CreateCartAsync(Guid userId, Guid productId, int quantity);
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<bool> RemoveItemAsync(Guid cartItemId);
    }
}
