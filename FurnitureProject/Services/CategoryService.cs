using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IEnumerable<Category>> GetAllAsync() => await _categoryRepo.GetAllAsync();

        public async Task<Category?> GetByIdAsync(Guid id) => await _categoryRepo.GetByIdAsync(id);

        public async Task<Category?> GetByNameAsync(string name) => await _categoryRepo.GetByNameAsync(name);

        public async Task<(bool Success, string? Message)> CreateAsync(Category category)
        {
            try
            {
                category.CreatedAt = DateTime.UtcNow;
                await _categoryRepo.AddAsync(category);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, null);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(Category category)
        {
            try
            {
                category.UpdatedAt = DateTime.UtcNow;
                await _categoryRepo.UpdateAsync(category);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                await _categoryRepo.DeleteAsync(id);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}
