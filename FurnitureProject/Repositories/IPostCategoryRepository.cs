using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IPostCategoryRepository
    {
        Task<IEnumerable<PostCategory>> GetAllAsync();
        Task<PostCategory?> GetByIdAsync(Guid id);
        Task AddAsync(PostCategory category);
        Task UpdateAsync(PostCategory category);
        Task DeleteAsync(Guid id);
    }
}
