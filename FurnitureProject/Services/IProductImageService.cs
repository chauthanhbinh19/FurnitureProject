using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImage>> GetAllAsync();
        Task<ProductImage?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task CreateAsync(ProductImage image);
        Task DeleteAsync(Guid id);
    }

}
