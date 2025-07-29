using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IFavouriteService
    {
        Task<List<Favourite>> GetFavouritesByUserAsync(Guid userId);
        Task<bool> IsFavouritedAsync(Guid userId, Guid productId);
        Task AddFavouriteAsync(Guid userId, Guid productId);
        Task RemoveFavouriteAsync(Guid userId, Guid productId);
    }
}
