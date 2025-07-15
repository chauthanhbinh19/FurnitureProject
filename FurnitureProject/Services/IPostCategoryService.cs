using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IPostCategoryService
    {
        Task<IEnumerable<PostCategory>> GetAllAsync();
        Task<PostCategory?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(PostCategory category);
        Task<(bool Success, string? Message)> UpdateAsync(PostCategory category);
        Task<bool> DeleteAsync(Guid id);
    }
}
