using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly AppDbContext _context;

        public ProductImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync()
        {
            return await _context.ProductImages
                .Where(x => !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetByIdAsync(int id)
        {
            return await _context.ProductImages
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.ImageId == id && !x.IsDeleted);
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductImages
                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(ProductImage image)
        {
            await _context.ProductImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProductImage image)
        {
            image.IsDeleted = true;
            _context.ProductImages.Update(image);
            await _context.SaveChangesAsync();
        }
    }

}
