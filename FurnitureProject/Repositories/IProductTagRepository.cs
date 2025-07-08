using FurnitureProject.Models;

namespace FurnitureProject.Repositories
{
    public interface IProductTagRepository
    {
        Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds);
        Task<List<Tag>> GetTagsByProductIdAsync(Guid productId);
    }
}
