using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<IEnumerable<CartItem>> GetByCartItemByIdAsync(Guid productId);
        Task CreateAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task DeleteAsync(CartItem item);
    }

}
