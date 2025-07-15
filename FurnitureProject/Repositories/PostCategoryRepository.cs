using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly AppDbContext _context;

        public PostCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostCategory>> GetAllAsync()
        {
            return await _context.PostCategories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<PostCategory?> GetByIdAsync(Guid id)
        {
            return await _context.PostCategories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task AddAsync(PostCategory category)
        {
            await _context.PostCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PostCategory category)
        {
            _context.PostCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.PostCategories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }

}
