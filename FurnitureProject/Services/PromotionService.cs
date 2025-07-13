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

        public async Task<Promotion?> GetByIdAsync(Guid id) => await _promotionRepo.GetByIdAsync(id);

        public async Task<(bool Success, string? Message)> CreateAsync(Promotion promotion)
        {
            try
            {
                promotion.CreatedAt = DateTime.UtcNow;
                await _promotionRepo.AddAsync(promotion);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(Promotion promotion)
        { 
            try
            {
                promotion.UpdatedAt = DateTime.UtcNow;
                await _promotionRepo.UpdateAsync(promotion);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                var promo = await _promotionRepo.GetByIdAsync(id);
                if (promo != null)
                {
                    await _promotionRepo.DeleteAsync(promo);
                }
                return (true, null);
            }
            catch (Exception ex) { 
                return(false, ex.Message);
            }
        }
    }

}
