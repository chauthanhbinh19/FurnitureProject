using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task CreateAsync(ProductDTO dto);
        Task UpdateAsync(ProductDTO dto);
        Task DeleteAsync(Guid id);
    }
}
