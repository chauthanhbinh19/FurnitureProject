using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> GetByNameAsync(string name);
        Task<(bool Success, string? Message)> CreateAsync(Category category);
        Task<(bool Success, string? Message)> UpdateAsync(Category category);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
    }

}
