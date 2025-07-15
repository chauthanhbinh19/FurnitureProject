using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAllAsync();
        Task<Post?> GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(Post post);
        Task<(bool Success, string? Message)> UpdateAsync(Post post);
        Task<bool> DeleteAsync(Guid id);
    }
}
