using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(PromotionDTO dto);
        Task<(bool Success, string? Message)> UpdateAsync(PromotionDTO dto);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }

}
