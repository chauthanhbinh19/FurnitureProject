using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FurnitureProject.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public ProductController(IProductService productService, ICategoryService categoryService,
            ITagService tagService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
        }
        private void GetUserInformationFromSession()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
        }
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = true;
            ViewBag.LayoutType = "user";
        }
        [HttpPost("category/{id}")]
        public async Task<IActionResult> ProductByCategory(Guid id, int page = 1)
        {
            SetViewBagForLayout();
            int pageSize = 10;
            GetUserInformationFromSession();
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDtos = products
                .Where(p => p.CategoryId == id)
                .Select(product => new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Status = product.Status,
                    Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                    CreatedAt = product.CreatedAt,
                    ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                    TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new()
                }).ToList();

            int totalProducts = productDtos.Count();
            var pagedProducts = productDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productViewModel = new ProductViewModel
            {
                Products = pagedProducts,
                //Filter = filter
            };

            //ViewBag.Status = "active";
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            //ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }
        [HttpGet("all")]
        public async Task<IActionResult> AllProducts(int page = 1)
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();
            int pageSize = 50;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDtos = products
                .Select(product => new ProductDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    Stock = product.Stock,
                    Status = product.Status,
                    Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                    CreatedAt = product.CreatedAt,
                    ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                    TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new()
                }).ToList();

            int totalProducts = productDtos.Count();
            var pagedProducts = productDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productViewModel = new ProductViewModel
            {
                Products = pagedProducts,
                //Filter = filter
            };

            //ViewBag.Status = "active";
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            //ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();
            var product = await _productService.GetByIdAsync(id);
            var tags = await _tagService.GetAllAsync();
            if (product == null)
            {
                return NotFound();
            }

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDto = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                Status = product.Status,
                Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                CreatedAt = product.CreatedAt,
                ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new()
            };

            var tagMap = tags.ToDictionary(t => t.Id, t => t.Name);
            ViewBag.TagMap = tagMap;
            return View(productDto);
        }
    }
}
