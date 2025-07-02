using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IProductImageRepository
    {
        Task<IEnumerable<ProductImage>> GetAllAsync();
        Task<ProductImage?> GetByIdAsync(int id);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
        Task AddAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
    }

}
