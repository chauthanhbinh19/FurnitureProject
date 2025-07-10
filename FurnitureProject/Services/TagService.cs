using FurnitureProject.Models;
using FurnitureProject.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepo;
        private readonly IProductTagRepository _productTagRepo;

        public TagService(ITagRepository tagRepo, IProductTagRepository productTagRepo)
        {
            _tagRepo = tagRepo;
            _productTagRepo = productTagRepo;
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _tagRepo.GetAllAsync();
        }

        public async Task<Tag>? GetByIdAsync(Guid id)
        {
            return await _tagRepo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Tag tag)
        {
            await _tagRepo.CreateAsync(tag);
        }

        public async Task UpdateAsync(Tag tag)
        {
            await _tagRepo.UpdateAsync(tag);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _tagRepo.DeleteAsync(id);
        }

        public async Task<List<Tag>> GetTagsByProductIdAsync(Guid productId)
        {
            return await _productTagRepo.GetTagsByProductIdAsync(productId);
        }

        public async Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds) =>
            _productTagRepo.AddTagsToProductAsync(productId, tagIds);
    }
}
