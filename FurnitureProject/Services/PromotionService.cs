using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;
using FurnitureProject.Constants;

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

        public async Task<(bool Success, string? Message)> CreateAsync(PromotionDTO dto)
        {
            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = AppConstants.Status.Active,
                CreatedAt = DateTime.UtcNow
            };

            promotion.ProductPromotions = dto.SelectedProductIds.Select(productId => new ProductPromotion
            {
                ProductId = productId,
                PromotionId = promotion.Id
            }).ToList();

            try
            {
                await _promotionRepo.AddAsync(promotion);
                return (true, null);
            }
            catch (Exception ex) { 
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(PromotionDTO dto)
        {
            var promotion = new Promotion
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = AppConstants.Status.Active, // Default status
                CreatedAt = dto.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                ProductPromotions = dto.SelectedProductIds.Select(id => new ProductPromotion
                {
                    ProductId = id,
                    PromotionId = dto.Id
                }).ToList()
            };
            try
            {
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
