using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class FavouriteRepository : IFavouriteRepository
    {
        private readonly AppDbContext _context;

        public FavouriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Favourite>> GetFavouritesByUserIdAsync(Guid userId)
        {
            return await _context.Favourites
                .Include(f => f.Product)
                .Where(f => f.userId == userId)
                .ToListAsync();
        }

        public async Task<Favourite?> GetByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.Favourites
                .FirstOrDefaultAsync(f => f.userId == userId && f.productId == productId);
        }

        public async Task AddAsync(Favourite favourite)
        {
            await _context.Favourites.AddAsync(favourite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Favourite favourite)
        {
            _context.Favourites.Remove(favourite);
            await _context.SaveChangesAsync();
        }
    }

}
