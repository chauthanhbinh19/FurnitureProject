using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/promotion")]
    public class AdminPromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public AdminPromotionController(IPromotionService promotionService, IProductService productService, ICartService cartService)
        {
            _promotionService = promotionService;
            _productService = productService;
            _cartService = cartService;
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
        public async Task<IActionResult> Index(PromotionFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var promotions = await _promotionService.GetAllAsync();

            var promotionDTOs = promotions.Select(promotion => new PromotionDTO
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                Status = promotion.Status,
                CreatedAt = promotion.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                promotionDTOs = promotionDTOs
                    .Where(u => u.Title.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                promotionDTOs = promotionDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                promotionDTOs = filter.SortColumn switch
                {
                    "Title" => isAscending
                        ? promotionDTOs.OrderBy(p => p.Title).ToList()
                        : promotionDTOs.OrderByDescending(p => p.Title).ToList(),

                    "Description" => isAscending
                        ? promotionDTOs.OrderBy(p => p.Description).ToList()
                        : promotionDTOs.OrderByDescending(p => p.Description).ToList(),

                    "DiscountPercent" => isAscending
                        ? promotionDTOs.OrderBy(p => p.DiscountPercent).ToList()
                        : promotionDTOs.OrderByDescending(p => p.DiscountPercent).ToList(),

                    "CreatedAt" => isAscending
                        ? promotionDTOs.OrderBy(p => p.CreatedAt).ToList()
                        : promotionDTOs.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? promotionDTOs.OrderBy(p => p.Status).ToList()
                        : promotionDTOs.OrderByDescending(p => p.Status).ToList(),

                    _ => promotionDTOs
                };
            }

            int totalPromotions = promotionDTOs.Count();
            var pagedPromotions = promotionDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var promotionViewModel = new PromotionViewModel
            {
                Promotions = pagedPromotions,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalPromotions = totalPromotions;
            return View(promotionViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            SetStatusViewBag();
            var products = await _productService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var promotionDto = new PromotionDTO
            {
                Products = products.Select(product => 
                {
                    var isInActivePromotion = promotions.Any(p =>
                        p.EndDate >= DateTime.Today &&
                        p.ProductPromotions.Any(pp => pp.ProductId == product.Id));

                    return new ProductDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        Status = product.Status,
                        //Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                        CreatedAt = product.CreatedAt,
                        ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                        TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                        PromotionStatus = isInActivePromotion ? "In Promotion" : "Available"
                    };
                }).ToList()
            };
            return View(promotionDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(PromotionDTO dto)
        {
            if (dto.DiscountPercent <= 0)
            {
                ModelState.AddModelError(nameof(dto.DiscountPercent), AppConstants.LogMessages.PromotionDiscountPercentCannotBeEmpty);
            }
            if (dto.DiscountPercent > 100)
            {
                ModelState.AddModelError(nameof(dto.DiscountPercent), AppConstants.LogMessages.PromotionDiscountPercentCannotBeEmpty);
            }

            if (dto.StartDate == default)
            {
                ModelState.AddModelError(nameof(dto.StartDate), AppConstants.LogMessages.PromotionStartDateCannotBeEmpty);
            }
            if (dto.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(dto.StartDate), AppConstants.LogMessages.PromotionStartDateCannotBeInPast);
            }

            if (dto.EndDate == default)
            {
                ModelState.AddModelError(nameof(dto.EndDate), AppConstants.LogMessages.PromotionEndDateCannotBeEmpty);
            }
            if (dto.EndDate.Date < dto.StartDate.Date)
            {
                ModelState.AddModelError(nameof(dto.EndDate), AppConstants.LogMessages.PromotionEndDateCannotBeBeforeStart);
            }

            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                LayoutHelper.SetViewBagForLayout(this, true, "admin");
                var products = await _productService.GetAllAsync();
                var promotions = await _promotionService.GetAllAsync();
                var promotionDTO = new PromotionDTO
                {
                    Products = products.Select(product =>
                    {
                        var isInActivePromotion = promotions.Any(p =>
                            p.EndDate >= DateTime.Today &&
                            p.ProductPromotions.Any(pp => pp.ProductId == product.Id));

                        return new ProductDTO
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Stock = product.Stock,
                            Status = product.Status,
                            //Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                            CreatedAt = product.CreatedAt,
                            ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                            TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                            PromotionStatus = isInActivePromotion ? "In Promotion" : "Available"
                        };
                    }).ToList()
                };
                return View(promotionDTO);
            }

            try
            {
                var (success, message) = await _promotionService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreatePromotionError;
                    return RedirectToAction("Create", "AdminPromotion");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreatePromotionSuccess;
                return RedirectToAction("Index", "AdminPromotion");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreatePromotionError;
                return RedirectToAction("Create", "AdminPromotion");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var promotion = await _promotionService.GetByIdAsync(id);
            var promotions = await _promotionService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var promotionDTO = new PromotionDTO
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DiscountPercent = promotion.DiscountPercent,
                Status = promotion.Status,
                Products = products.Select(product =>
                {
                    var isInActivePromotion = promotions.Any(p =>
                        p.EndDate >= DateTime.Today &&
                        p.ProductPromotions.Any(pp => pp.ProductId == product.Id));

                    return new ProductDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Price = product.Price,
                        Stock = product.Stock,
                        Status = product.Status,
                        //Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                        CreatedAt = product.CreatedAt,
                        ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                        TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                        PromotionStatus = isInActivePromotion ? "In Promotion" : "Available"
                    };
                }).ToList(),
                SelectedProductIds = promotion.ProductPromotions?.Select(pp => pp.ProductId).ToList() ?? new List<Guid>()
            };
            SetStatusViewBag(promotion.Status);

            return View(promotionDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(PromotionDTO dto)
        {
            if (dto.DiscountPercent <= 0)
            {
                ModelState.AddModelError(nameof(dto.DiscountPercent), AppConstants.LogMessages.PromotionDiscountPercentCannotBeEmpty);
            }
            if (dto.DiscountPercent > 100)
            {
                ModelState.AddModelError(nameof(dto.DiscountPercent), AppConstants.LogMessages.PromotionDiscountPercentCannotBeEmpty);
            }

            if (dto.StartDate == default)
            {
                ModelState.AddModelError(nameof(dto.StartDate), AppConstants.LogMessages.PromotionStartDateCannotBeEmpty);
            }
            if (dto.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(dto.StartDate), AppConstants.LogMessages.PromotionStartDateCannotBeInPast);
            }

            if (dto.EndDate == default)
            {
                ModelState.AddModelError(nameof(dto.EndDate), AppConstants.LogMessages.PromotionEndDateCannotBeEmpty);
            }
            if (dto.EndDate.Date < dto.StartDate.Date)
            {
                ModelState.AddModelError(nameof(dto.EndDate), AppConstants.LogMessages.PromotionEndDateCannotBeBeforeStart);
            }

            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                LayoutHelper.SetViewBagForLayout(this, true, "admin");
                var promotion = await _promotionService.GetByIdAsync(dto.Id);
                var promotions = await _promotionService.GetAllAsync();
                var products = await _productService.GetAllAsync();
                var promotionDTO = new PromotionDTO
                {
                    Id = promotion.Id,
                    Title = promotion.Title,
                    Description = promotion.Description,
                    StartDate = promotion.StartDate,
                    EndDate = promotion.EndDate,
                    DiscountPercent = promotion.DiscountPercent,
                    Status = promotion.Status,
                    Products = products.Select(product =>
                    {
                        var isInActivePromotion = promotions.Any(p =>
                            p.EndDate >= DateTime.Today &&
                            p.ProductPromotions.Any(pp => pp.ProductId == product.Id));

                        return new ProductDTO
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Stock = product.Stock,
                            Status = product.Status,
                            //Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                            CreatedAt = product.CreatedAt,
                            ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                            TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                            PromotionStatus = isInActivePromotion ? "In Promotion" : "Available"
                        };
                    }).ToList(),
                    SelectedProductIds = promotion.ProductPromotions?.Select(pp => pp.ProductId).ToList() ?? new List<Guid>()
                };
                SetStatusViewBag(dto.Status);
                return View(promotionDTO);
            }

            try
            {
                var (success, message) = await _promotionService.UpdateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdatePromotionError;
                    return RedirectToAction("Index", "AdminPromotion");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdatePromotionSuccess;
                return RedirectToAction("Index", "AdminPromotion");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdatePromotionError;
                return RedirectToAction("Update", "AdminPromotion");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (success, message) = await _promotionService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeletePromotionError;
                    return RedirectToAction("Index", "AdminPromotion");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeletePromotionSuccess;
                return RedirectToAction("Index", "AdminPromotion");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeletePromotionError;
                return RedirectToAction("Index", "AdminPromotion");
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var promotion = await _promotionService.GetByIdAsync(id);
            var promotions = await _promotionService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var selectedProductIds = promotion.ProductPromotions?
                .Select(pp => pp.ProductId)
                .ToList() ?? new List<Guid>();

            var promotionDTO = new PromotionDTO
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                DiscountPercent = promotion.DiscountPercent,
                Status = promotion.Status,
                Products = products
                    .Where(product => selectedProductIds.Contains(product.Id))
                    .Select(product =>
                    {
                        var isInActivePromotion = promotions.Any(p =>
                            p.EndDate >= DateTime.Today &&
                            p.ProductPromotions.Any(pp => pp.ProductId == product.Id));

                        return new ProductDTO
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Stock = product.Stock,
                            Status = product.Status,
                            //Category = categories.FirstOrDefault(c => c.Id == product.CategoryId),
                            CreatedAt = product.CreatedAt,
                            ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                            TagIds = product.ProductTags?.Select(pt => pt.TagId).ToList() ?? new(),
                            PromotionStatus = isInActivePromotion ? "In Promotion" : "Available"
                        };
                    }).ToList(),
                SelectedProductIds = promotion.ProductPromotions?.Select(pp => pp.ProductId).ToList() ?? new List<Guid>()
            };
            SetStatusViewBag(promotion.Status);

            return View(promotionDTO);
        }
    }

}
