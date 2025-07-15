using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync()
        {
            return await _context.Promotions
                .Where(p => !p.IsDeleted)
                .Include(p => p.ProductPromotions)
                    .ThenInclude(pp => pp.Product)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByIdAsync(Guid id)
        {
            return await _context.Promotions
                .Include(p => p.ProductPromotions)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task AddAsync(Promotion promotion)
        {
            promotion.StartDate = DateTime.SpecifyKind(promotion.StartDate, DateTimeKind.Utc);
            promotion.EndDate = DateTime.SpecifyKind(promotion.EndDate, DateTimeKind.Utc);
            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            promotion.StartDate = DateTime.SpecifyKind(promotion.StartDate, DateTimeKind.Utc);
            promotion.EndDate = DateTime.SpecifyKind(promotion.EndDate, DateTimeKind.Utc);
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Promotion promotion)
        {
            promotion.IsDeleted = true;
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }
    }

}
