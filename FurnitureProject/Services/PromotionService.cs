using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepo;

        public PromotionService(IPromotionRepository promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }

        public async Task<IEnumerable<Promotion>> GetAllAsync() => await _promotionRepo.GetAllAsync();

        public async Task<Promotion?> GetByIdAsync(int id) => await _promotionRepo.GetByIdAsync(id);

        public async Task CreateAsync(Promotion promotion)
        {
            promotion.CreatedAt = DateTime.UtcNow;
            await _promotionRepo.AddAsync(promotion);
        }

        public async Task UpdateAsync(Promotion promotion)
        {
            promotion.UpdatedAt = DateTime.UtcNow;
            await _promotionRepo.UpdateAsync(promotion);
        }

        public async Task DeleteAsync(int id)
        {
            var promo = await _promotionRepo.GetByIdAsync(id);
            if (promo != null)
            {
                await _promotionRepo.DeleteAsync(promo);
            }
        }
    }

}
