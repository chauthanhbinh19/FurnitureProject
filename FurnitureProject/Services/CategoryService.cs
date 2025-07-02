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

        public async Task<Category?> GetByIdAsync(int id) => await _categoryRepo.GetByIdAsync(id);

        public async Task<Category?> GetByNameAsync(string name) => await _categoryRepo.GetByNameAsync(name);

        public async Task CreateAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            await _categoryRepo.AddAsync(category);
        }

        public async Task UpdateAsync(Category category)
        {
            category.UpdatedAt = DateTime.UtcNow;
            await _categoryRepo.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepo.DeleteAsync(category);
            }
        }
    }

}
