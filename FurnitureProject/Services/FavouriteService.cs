using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepository _favouriteRepository;

        public FavouriteService(IFavouriteRepository favouriteRepository)
        {
            _favouriteRepository = favouriteRepository;
        }

        public async Task<List<Favourite>> GetFavouritesByUserAsync(Guid userId)
        {
            return await _favouriteRepository.GetFavouritesByUserIdAsync(userId);
        }

        public async Task<bool> IsFavouritedAsync(Guid userId, Guid productId)
        {
            var favourite = await _favouriteRepository.GetByUserAndProductAsync(userId, productId);
            if(favourite == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }

        public async Task AddFavouriteAsync(Guid userId, Guid productId)
        {
            var exists = await _favouriteRepository.GetByUserAndProductAsync(userId, productId);
            if (exists == null)
            {
                var fav = new Favourite
                {
                    id = Guid.NewGuid(),
                    userId = userId,
                    productId = productId
                };
                await _favouriteRepository.AddAsync(fav);
            }
        }

        public async Task RemoveFavouriteAsync(Guid userId, Guid productId)
        {
            var favourite = await _favouriteRepository.GetByUserAndProductAsync(userId, productId);
            if (favourite != null)
            {
                await _favouriteRepository.RemoveAsync(favourite);
            }
        }
    }

}
