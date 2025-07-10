using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag>? GetByIdAsync(Guid id);
        Task CreateAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Guid id);
        Task<List<Tag>> GetTagsByProductIdAsync(Guid productId);
        Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds);
    }
}
