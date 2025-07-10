using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FurnitureProject.Controllers
{
    [Route("admin/product")]
    public class AdminProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;

        public AdminProductController(IProductService productService, ICategoryService categoryService, 
            ITagService tagService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        private async Task SetCategoryViewBag(Guid? categoryId = null)
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", categoryId);
        }
        private async Task SetTagViewBag(Guid? tagId = null)
        {
            var tags = await _tagService.GetAllAsync();
            ViewBag.Tags = new SelectList(tags, "Id", "Name", tagId);
        }
        private void SetStatusViewBag(string? status = null)
        {
            ViewBag.StatusList = new SelectList(
                new[] {
                    new { Value = "active", Text = "Đang hoạt động" },
                    new { Value = "inactive", Text = "Đã ẩn" }
                },
                "Value", "Text", status
            );
        }
        private void SetSortOptions(string? selectedSort = null)
        {
            var sortOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "Mới nhất", Value = "newest" },
                new SelectListItem { Text = "Cũ nhất", Value = "oldest" },
                new SelectListItem { Text = "Giá tăng dần", Value = "price-asc" },
                new SelectListItem { Text = "Giá giảm dần", Value = "price-desc" }
            };

            ViewBag.SortOptions = new SelectList(sortOptions, "Value", "Text", selectedSort);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(ProductFilterDTO filter, int page = 1)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var tags = await _tagService.GetAllAsync();

            var productDtos = products.Select(product => new ProductDTO
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

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                productDtos = productDtos
                    .Where(u => u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                                u.Description.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by category id
            if (filter.FilterCategoryId.HasValue)
            {
                productDtos = productDtos
                    .Where(p => p.Category?.Id == filter.FilterCategoryId.Value)
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                productDtos = productDtos
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Filter by tag id
            if (filter.FilterTagId.HasValue)
            {
                productDtos = productDtos
                    .Where(p => p.TagIds != null && p.TagIds.Any(id => id == filter.FilterTagId.Value))
                    .ToList();
            }

            // Sort Order
            switch (filter.SortOrder)
            {
                case "newest":
                    productDtos = productDtos.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "oldest":
                    productDtos = productDtos.OrderBy(p => p.CreatedAt).ToList();
                    break;
                case "price-asc":
                    productDtos = productDtos.OrderBy(p => p.Price).ToList();
                    break;
                case "price-desc":
                    productDtos = productDtos.OrderByDescending(p => p.Price).ToList();
                    break;
            }



            int totalProducts = productDtos.Count();
            var pagedProducts = productDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productViewModel = new ProductViewModel
            {
                Products = pagedProducts,
                Filter = filter
            };

            var tagMap = tags.ToDictionary(t => t.Id, t => t.Name);
            ViewBag.TagMap = tagMap;

            await SetCategoryViewBag(filter.FilterCategoryId);
            await SetTagViewBag(filter.FilterTagId);
            SetStatusViewBag(filter.FilterByStatus);
            SetSortOptions(filter.SortOrder);

            ViewBag.Status = "active";
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
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
            await SetTagViewBag();

            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto)
        {
            await _productService.CreateAsync(dto);
            return RedirectToAction("Index","AdminProduct");
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var product = await _productService.GetByIdAsync(id);

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrls = product.ProductImages?.Select(p => p.ImageUrl).ToList() ?? new List<string>(),
                TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new List<Guid>()
            };

            await SetCategoryViewBag();
            //await SetTagViewBag(productDTO.TagIds);
            var tags = await _tagService.GetAllAsync();
            ViewBag.Tags = tags;
            return View(productDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(ProductDTO dto)
        {
            await _productService.UpdateAsync(dto);
            return RedirectToAction("Index","AdminProduct");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}
