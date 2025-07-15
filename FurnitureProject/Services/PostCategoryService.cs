using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class PostCategoryService : IPostCategoryService
    {
        private readonly IPostCategoryRepository _repository;

        public PostCategoryService(IPostCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PostCategory>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PostCategory?> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? Message)> CreateAsync(PostCategory category)
        {
            try
            {
                await _repository.AddAsync(category);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(PostCategory category)
        {
            try
            {
                await _repository.UpdateAsync(category);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }
    }
}
