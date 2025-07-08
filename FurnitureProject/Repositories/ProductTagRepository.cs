using FurnitureProject.Data;
using FurnitureProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FurnitureProject.Repositories
{
    public class ProductTagRepository : IProductTagRepository
    {
        private readonly AppDbContext _context;
        public ProductTagRepository(AppDbContext context) => _context = context;

        public async Task AddTagsToProductAsync(Guid productId, List<Guid> tagIds)
        {
            var productTags = tagIds.Select(tagId => new ProductTag
            {
                ProductId = productId,
                TagId = tagId
            });

            _context.ProductTag.AddRange(productTags);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetTagsByProductIdAsync(Guid productId)
        {
            return await _context.ProductTag
                .Where(pt => pt.ProductId == productId)
                .Select(pt => pt.Tag)
                .ToListAsync();
        }
    }
}
