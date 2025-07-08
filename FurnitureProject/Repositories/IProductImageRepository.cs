using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IProductImageRepository
    {
        Task<IEnumerable<ProductImage>> GetAllAsync();
        Task<ProductImage?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);
        Task AddAsync(ProductImage image);
        Task DeleteAsync(ProductImage image);
    }

}
