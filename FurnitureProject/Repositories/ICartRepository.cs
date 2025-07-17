using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<bool> RemoveItemAsync(Guid cartItemId);
        Task CreateAsync(Cart cart);
        Task UpdateAsync(Cart cart);
    }
}
