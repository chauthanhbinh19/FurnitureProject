using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _imageRepo;

        public ProductImageService(IProductImageRepository imageRepo)
        {
            _imageRepo = imageRepo;
        }

        public async Task<IEnumerable<ProductImage>> GetAllAsync() => await _imageRepo.GetAllAsync();

        public async Task<ProductImage?> GetByIdAsync(Guid id) => await _imageRepo.GetByIdAsync(id);

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId)
        {
            return await _imageRepo.GetByProductIdAsync(productId);
        }

        public async Task CreateAsync(ProductImage image)
        {
            image.CreatedAt = DateTime.UtcNow;
            await _imageRepo.AddAsync(image);
        }

        public async Task DeleteAsync(Guid id)
        {
            var image = await _imageRepo.GetByIdAsync(id);
            if (image != null)
            {
                await _imageRepo.DeleteAsync(image);
            }
        }
    }

}
