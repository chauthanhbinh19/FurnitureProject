using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Constants;
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
        private readonly ICartService _cartService;
        private readonly IPromotionService _promotionService;

        public AdminProductController(IProductService productService, ICategoryService categoryService, 
            ITagService tagService, ICartService cartService, IPromotionService promotionService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _tagService = tagService;
            _cartService = cartService;
            _promotionService = promotionService;
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
                    new { Value = AppConstants.Status.Active, Text = AppConstants.Display.Active },
                    new { Value = AppConstants.Status.Inactive, Text = AppConstants.Display.Inactive }
                },
                "Value", "Text", status
            );
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(ProductFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var tags = await _tagService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;

            var productDtos = products.Select(product =>
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
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                productDtos = productDtos
                    .Where(u => (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(u.Description) && u.Description.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)))
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
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                productDtos = filter.SortColumn switch
                {
                    "Name" => isAscending
                        ? productDtos.OrderBy(p => p.Name).ToList()
                        : productDtos.OrderByDescending(p => p.Name).ToList(),

                    "Price" => isAscending
                        ? productDtos.OrderBy(p => p.Price).ToList()
                        : productDtos.OrderByDescending(p => p.Price).ToList(),

                    "Stock" => isAscending
                        ? productDtos.OrderBy(p => p.Stock).ToList()
                        : productDtos.OrderByDescending(p => p.Stock).ToList(),

                    "CategoryName" => isAscending
                        ? productDtos.OrderBy(p => p.Category.Name).ToList()
                        : productDtos.OrderByDescending(p => p.Category.Name).ToList(),

                    "CreatedAt" => isAscending
                        ? productDtos.OrderBy(p => p.CreatedAt).ToList()
                        : productDtos.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? productDtos.OrderBy(p => p.Status).ToList()
                        : productDtos.OrderByDescending(p => p.Status).ToList(),

                    _ => productDtos
                };
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
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            await SetCategoryViewBag();
            await SetTagViewBag();

            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] ProductDTO dto)
        {
            try
            {
                var(success, message) = await _productService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateProductError;
                    return RedirectToAction("Create", "AdminProduct");
                }
                
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateProductSuccess;
                return RedirectToAction("Index", "AdminProduct");
            }
            catch (Exception) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateProductError;
                return RedirectToAction("Create", "AdminProduct");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
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
            try
            {
                var(success, message) = await _productService.UpdateAsync(dto);
                if (!success) {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateProductError;
                    return RedirectToAction("Index", "AdminProduct");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateProductSuccess;
                return RedirectToAction("Index", "AdminProduct");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateProductError;
                return RedirectToAction("Update", "AdminProduct");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var(success, message) = await _productService.DeleteAsync(id);
                if (!success) {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteProductError;
                    return RedirectToAction("Index", "AdminProduct");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteProductSuccess;
                return RedirectToAction("Index", "AdminProduct");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteProductError;
                return RedirectToAction("Index", "AdminProduct");
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
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
        [HttpGet("product-popup")]
        public async Task<IActionResult> ProductPopup(ProductFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var products = await _productService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();
            var tags = await _tagService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;

            var productDtos = products.Select(product =>
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
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                productDtos = productDtos
                    .Where(u => u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                                (u.Description != null &&
                                u.Description.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)))
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
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                productDtos = filter.SortColumn switch
                {
                    "Name" => isAscending
                        ? productDtos.OrderBy(p => p.Name).ToList()
                        : productDtos.OrderByDescending(p => p.Name).ToList(),

                    "Price" => isAscending
                        ? productDtos.OrderBy(p => p.Price).ToList()
                        : productDtos.OrderByDescending(p => p.Price).ToList(),

                    "Stock" => isAscending
                        ? productDtos.OrderBy(p => p.Stock).ToList()
                        : productDtos.OrderByDescending(p => p.Stock).ToList(),

                    "CategoryName" => isAscending
                        ? productDtos.OrderBy(p => p.Category.Name).ToList()
                        : productDtos.OrderByDescending(p => p.Category.Name).ToList(),

                    "CreatedAt" => isAscending
                        ? productDtos.OrderBy(p => p.CreatedAt).ToList()
                        : productDtos.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? productDtos.OrderBy(p => p.Status).ToList()
                        : productDtos.OrderByDescending(p => p.Status).ToList(),

                    _ => productDtos
                };
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

            ViewBag.Status = "active";
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Keyword = filter.SearchKeyWord;
            ViewBag.TotalProducts = totalProducts;
            return PartialView("_ProductPopup", productViewModel);
        }
    }
}
