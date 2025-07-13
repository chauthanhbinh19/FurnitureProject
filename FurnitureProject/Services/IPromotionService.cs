using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(Promotion promotion);
        Task<(bool Success, string? Message)> UpdateAsync(Promotion promotion);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }

}
