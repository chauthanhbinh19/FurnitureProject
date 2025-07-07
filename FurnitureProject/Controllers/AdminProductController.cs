using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/product")]
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly Cloudinary _cloudinary;

        public AdminProductController(IProductService productService, ICategoryService categoryService, Cloudinary cloudinary)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cloudinary = cloudinary;
        }

        private async Task SetCategoryViewBag(string? categoryId = null)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            var productDtos = products.Select(product => new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                CreatedAt = product.CreatedAt,
                ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
            }).ToList();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                productDtos = productDtos
                    .Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                u.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalProducts = productDtos.Count();
            var pagedProducts = productDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalProducts = totalProducts;
            return View(pagedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            await SetCategoryViewBag();

            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto)
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

            // Chuyển từ DTO -> Entity
            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                ProductImages = imageList
            };
            await _productService.CreateAsync(product);
            //return CreatedAtAction(nameof(GetById), new { id = product.Id });
            return RedirectToAction("Index","AdminProduct");
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var product = await _productService.GetByIdAsync(id);
            return View(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product product)
        {
            if (id != product.Id) return BadRequest();
            await _productService.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
