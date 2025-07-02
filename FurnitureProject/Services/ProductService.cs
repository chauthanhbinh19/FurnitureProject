using FurnitureProject.Models;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _productRepo.GetAllAsync();

        public async Task<Product?> GetByIdAsync(int id) => await _productRepo.GetByIdAsync(id);

        public async Task CreateAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            await _productRepo.AddAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepo.UpdateAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepo.DeleteAsync(product);
            }
        }
    }

}
