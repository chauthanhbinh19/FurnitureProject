using FurnitureProject.Models;
using FurnitureProject.Models.DTO;

namespace FurnitureProject.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(ProductDTO dto);
        Task<(bool Success, string? Message)> UpdateAsync(ProductDTO dto);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }
}
