using FurnitureProject.Models;

namespace FurnitureProject.Services
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag>? GetByIdAsync(Guid id);
        Task<(bool Success, string? Message)> CreateAsync(Tag tag);
        Task<(bool Success, string? Message)> UpdateAsync(Tag tag);
        Task<(bool Success, string? Message)> DeleteAsync(Guid id);
        Task<List<Tag>> GetTagsByProductIdAsync(Guid productId);
        Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds);
    }
}
