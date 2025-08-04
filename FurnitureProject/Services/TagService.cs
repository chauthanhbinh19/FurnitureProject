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

        public async Task<(bool Success, string? Message)> CreateAsync(Tag tag)
        {
            try
            {
                tag.CreatedAt = DateTime.UtcNow;
                await _tagRepo.CreateAsync(tag);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(Tag tag)
        {
            try
            {
                tag.UpdatedAt = DateTime.UtcNow;
                await _tagRepo.UpdateAsync(tag);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                await _tagRepo.DeleteAsync(id);
                return (true, null);
            }
            catch (Exception) {
                return (false, null);
            }
        }

        public async Task<List<Tag>> GetTagsByProductIdAsync(Guid productId)
        {
            return await _productTagRepo.GetTagsByProductIdAsync(productId);
        }

        public async Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds)
        {
            await _productTagRepo.AddTagsToProductAsync(productId, tagIds);
        }
    }
}
