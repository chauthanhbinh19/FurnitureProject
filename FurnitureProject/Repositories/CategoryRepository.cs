using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name && !c.IsDeleted);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
            }
        }
    }

}
