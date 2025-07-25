using FurnitureProject.Helper;
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
        private readonly ICartService _cartService;
        private readonly IPromotionService _promotionService;

        public ProductController(IProductService productService, ICategoryService categoryService,
            ITagService tagService, ICartService cartService, IPromotionService promotionService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
            _cartService = cartService;
            _promotionService = promotionService;
        }
        [HttpGet("category/{id}")]
        public async Task<IActionResult> ProductByCategory(Guid id, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            int pageSize = 10;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;

            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDtos = products
                .Where(p => p.CategoryId == id)
                .Select(product =>
                {
                    var activePromotion = promotions.FirstOrDefault(promo =>
                        promo.ProductPromotions.Any(pp => pp.ProductId == product.Id) &&
                        promo.EndDate >= today
                    );

                    decimal discountPrice = 0;
                    if (activePromotion != null)
                    {
                        var discount = activePromotion.DiscountPercent;
                        discountPrice = product.Price * (1 - discount / 100m);
                    }
                    return new ProductDTO
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
                        TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                        DiscountPrice = discountPrice,
                    };
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
        public async Task<IActionResult> AllProducts(string search = "", int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            int pageSize = 50;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;

            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var productDtos = products
                .Select(product =>
                {
                    var activePromotion = promotions.FirstOrDefault(promo =>
                        promo.ProductPromotions.Any(pp => pp.ProductId == product.Id) &&
                        promo.EndDate >= today
                    );

                    decimal discountPrice = 0;
                    if (activePromotion != null)
                    {
                        var discount = activePromotion.DiscountPercent;
                        discountPrice = product.Price * (1 - discount / 100m);
                    }
                    return new ProductDTO
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
                        TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                        DiscountPrice = discountPrice,
                    };
                }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(search))
            {
                productDtos = productDtos
                    .Where(u =>
                        (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Description) && u.Description.Contains(search, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

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
            ViewBag.Search = search;
            ViewBag.TotalProducts = totalProducts;
            return View(productViewModel);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> ProductDetail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");
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
