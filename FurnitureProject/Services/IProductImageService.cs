using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IProductImageService
    {
        Task<IEnumerable<ProductImage>> GetAllAsync();
        Task<ProductImage?> GetByIdAsync(int id);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
        Task CreateAsync(ProductImage image);
        Task DeleteAsync(int id);
    }

}
