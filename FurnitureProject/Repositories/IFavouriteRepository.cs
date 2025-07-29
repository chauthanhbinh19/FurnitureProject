using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IFavouriteRepository
    {
        Task<List<Favourite>> GetFavouritesByUserIdAsync(Guid userId);
        Task<Favourite?> GetByUserAndProductAsync(Guid userId, Guid productId);
        Task AddAsync(Favourite favourite);
        Task RemoveAsync(Favourite favourite);
    }
}
