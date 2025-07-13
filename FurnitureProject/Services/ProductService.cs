using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Repositories;

namespace FurnitureProject.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductImageService _productImageService;
        private readonly Cloudinary _cloudinary;

        public ProductService(IProductRepository productRepo, IProductImageService productImageService, Cloudinary cloudinary)
        {
            _productRepo = productRepo;
            _productImageService = productImageService;
            _cloudinary = cloudinary;
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _productRepo.GetAllAsync();

        public async Task<Product?> GetByIdAsync(Guid id) => await _productRepo.GetByIdAsync(id);

        public async Task<(bool Success, string? Message)> CreateAsync(ProductDTO dto)
        {
            var imageList = new List<ProductImage>();

            if (dto.Files != null)
            {
                foreach (var file in dto.Files)
                {
                    if (file.Length > 0)
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            Folder = "products"
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            imageList.Add(new ProductImage
                            {
                                ImageUrl = uploadResult.SecureUrl.ToString()
                            });
                        }
                    }
                }
            }

            if (imageList == null)
            {
                imageList.Add(new ProductImage
                {
                    ImageUrl = "https://res.cloudinary.com/dewf4msbe/image/upload/v1751963408/No_Image_Available_arromx.jpg"
                });
            }

            var productId = Guid.NewGuid();

            var productTags = dto.TagIds.Select(tagId => new ProductTag
            {
                ProductId = productId,
                TagId = tagId
            }).ToList();
            // Chuyển từ DTO -> Entity
            var product = new Product
            {
                Id = productId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ProductImages = imageList,
                ProductTags = productTags
            };
            product.CreatedAt = DateTime.UtcNow;

            try
            {
                await _productRepo.AddAsync(product);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, null);
            }
        }

        public async Task<(bool Success, string? Message)> UpdateAsync(ProductDTO dto)
        {
            var product = await _productRepo.GetByIdAsync(dto.Id);
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            var imageList = new List<ProductImage>();
            var oldImageList = await _productImageService.GetByProductIdAsync(dto.Id);

            var existingUrls = new HashSet<string>(dto.ExistingImageUrls ?? new List<string>());

            var removedImages = oldImageList
                .Where(img => !existingUrls.Contains(img.ImageUrl))
                .ToList();

            foreach (var img in removedImages)
            {
                var deletionParams = new DeletionParams($"products/{img.ImageUrl}");
                var result = await _cloudinary.DestroyAsync(deletionParams);

                var target = product.ProductImages.FirstOrDefault(p => p.ImageId == img.ImageId);
                if (target != null)
                {
                    product.ProductImages.Remove(target);
                }

                await _productImageService.DeleteAsync(img.ImageId);
            }

            if (dto.Files != null)
            {
                foreach (var file in dto.Files)
                {
                    if (file.Length > 0)
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream()),
                            Folder = "products"
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            string newImageUrl = uploadResult.SecureUrl.ToString();

                            if (!product.ProductImages.Any(x => x.ImageUrl == newImageUrl))
                            {
                                product.ProductImages.Add(new ProductImage
                                {
                                    //ProductId = product.Id,
                                    ImageUrl = newImageUrl
                                });
                            }
                        }
                    }
                }
            }

            product.ProductTags.Clear();
            foreach (var tagId in dto.TagIds)
            {
                product.ProductTags.Add(new ProductTag
                {
                    ProductId = product.Id,
                    TagId = tagId
                });
            }

            try
            {
                await _productRepo.UpdateAsync(product);
                return (true, null);
            }
            catch (Exception ex) {
                return (false, null);
            }
        }

        public async Task<(bool Success, string? Message)> DeleteAsync(Guid id)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);
                if (product != null)
                {
                    await _productRepo.DeleteAsync(product);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, null);
            }
        }
    }

}
