using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<Promotion>> GetAllAsync();
        Task<Promotion?> GetByIdAsync(Guid id);
        Task CreateAsync(Promotion promotion);
        Task UpdateAsync(Promotion promotion);
        Task DeleteAsync(Guid id);
    }

}
