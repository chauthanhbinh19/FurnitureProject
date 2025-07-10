using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag>? GetByIdAsync(Guid id);
        Task CreateAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
    }
}
