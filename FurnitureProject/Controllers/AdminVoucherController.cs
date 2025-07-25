using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/voucher")]
    public class AdminVoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public AdminVoucherController(IVoucherService voucherService, IProductService productService, ICartService cartService)
        {
            _voucherService = voucherService;
            _productService = productService;
            _cartService = cartService;
        }
        private void SetStatusViewBag(string? status = null)
        {
            ViewBag.StatusList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Active, Text = AppConstants.LogMessages.Active },
                    new { Value = AppConstants.Status.Inactive, Text = AppConstants.LogMessages.Inactive }
                },
                "Value", "Text", status
            );
        }
        private void SetSortOptions(string? selectedSort = null)
        {
            var sortOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = AppConstants.LogMessages.Newest, Value = AppConstants.Status.Newest },
                new SelectListItem { Text = AppConstants.LogMessages.Oldest, Value = AppConstants.Status.Oldest },
                //new SelectListItem { Text = "Giá tăng dần", Value = "price-asc" },
                //new SelectListItem { Text = "Giá giảm dần", Value = "price-desc" }
            };

            ViewBag.SortOptions = new SelectList(sortOptions, "Value", "Text", selectedSort);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(VoucherFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var vouchers = await _voucherService.GetAllAsync();

            var voucherDTOs = vouchers.Select(voucher => new VoucherDTO
            {
                Id = voucher.Id,
                Code = voucher.Code,
                DiscountPercent = voucher.DiscountPercent,
                DiscountAmount = voucher.DiscountAmount,
                ExpiryDate = voucher.ExpiryDate,
                UsageLimit = voucher.UsageLimit,
                TimeUsed = voucher.TimeUsed,
                IsValid = voucher.IsValid,
                Status = voucher.Status,
                CreatedAt = voucher.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                voucherDTOs = voucherDTOs
                    .Where(u => u.Code.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                voucherDTOs = voucherDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                voucherDTOs = filter.SortColumn switch
                {
                    "Code" => isAscending
                        ? voucherDTOs.OrderBy(p => p.Code).ToList()
                        : voucherDTOs.OrderByDescending(p => p.Code).ToList(),

                    "DiscountPercent" => isAscending
                        ? voucherDTOs.OrderBy(p => p.DiscountPercent).ToList()
                        : voucherDTOs.OrderByDescending(p => p.DiscountPercent).ToList(),

                    "DiscountAmount" => isAscending
                        ? voucherDTOs.OrderBy(p => p.DiscountAmount).ToList()
                        : voucherDTOs.OrderByDescending(p => p.DiscountAmount).ToList(),

                    "CreatedAt" => isAscending
                        ? voucherDTOs.OrderBy(p => p.CreatedAt).ToList()
                        : voucherDTOs.OrderByDescending(p => p.CreatedAt).ToList(),

                    "ExpiryDate" => isAscending
                        ? voucherDTOs.OrderBy(p => p.ExpiryDate).ToList()
                        : voucherDTOs.OrderByDescending(p => p.ExpiryDate).ToList(),

                    "Status" => isAscending
                        ? voucherDTOs.OrderBy(p => p.Status).ToList()
                        : voucherDTOs.OrderByDescending(p => p.Status).ToList(),

                    _ => voucherDTOs
                };
            }

            int totalvouchers = voucherDTOs.Count();
            var pagedvouchers = voucherDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var voucherViewModel = new VoucherViewModel
            {
                Vouchers = pagedvouchers,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalVouchers = totalvouchers;
            return View(voucherViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var promotion = await _voucherService.GetByIdAsync(id);
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
            var vouchers = await _voucherService.GetAllAsync();
            var voucherDto = new VoucherDTO
            {
                Products = products.Select(product =>
                {
                    var isInActiveVoucher = vouchers.Any(p =>
                        p.ExpiryDate >= DateTime.Today &&
                        p.ProductVouchers.Any(pp => pp.ProductId == product.Id));

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
                        VoucherStatus = isInActiveVoucher ? "In Voucher" : "Available"
                    };
                }).ToList()
            };
            return View(voucherDto);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(VoucherDTO dto)
        {
            if (dto.ExpiryDate == default)
            {
                ModelState.AddModelError(nameof(dto.ExpiryDate), AppConstants.LogMessages.VoucherExpiryDateCannotBeEmpty);
            }
            if (dto.ExpiryDate.Date == DateTime.Today)
            {
                ModelState.AddModelError(nameof(dto.ExpiryDate), AppConstants.LogMessages.VoucherExpiryDateCannotBeInPast);
            }

            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                LayoutHelper.SetViewBagForLayout(this, true, "admin");
                var products = await _productService.GetAllAsync();
                var vouchers = await _voucherService.GetAllAsync();
                var voucherDTO = new VoucherDTO
                {
                    Products = products.Select(product =>
                    {
                        var isInActiveVoucher = vouchers.Any(p =>
                            p.ExpiryDate >= DateTime.Today &&
                            p.ProductVouchers.Any(pp => pp.ProductId == product.Id));

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
                            VoucherStatus = isInActiveVoucher ? "In Voucher" : "Available"
                        };
                    }).ToList()
                };
                return View(voucherDTO);
            }

            try
            {
                var (success, message) = await _voucherService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateVoucherError;
                    return RedirectToAction("Create", "AdminVoucher");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateProductSuccess;
                return RedirectToAction("Index", "AdminVoucher");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateProductError;
                return RedirectToAction("Create", "AdminVoucher");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var voucher = await _voucherService.GetByIdAsync(id);
            var vouchers = await _voucherService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            var voucherDTO = new VoucherDTO
            {
                Id = voucher.Id,
                Code = voucher.Code,
                DiscountPercent = voucher.DiscountPercent,
                DiscountAmount = voucher.DiscountAmount,
                ExpiryDate = voucher.ExpiryDate,
                UsageLimit = voucher.UsageLimit,
                TimeUsed = voucher.TimeUsed,
                IsValid = voucher.IsValid,
                Status = voucher.Status,
                CreatedAt = voucher.CreatedAt,
                Products = products.Select(product =>
                {
                    var isInActiveVoucher = vouchers.Any(p =>
                        p.ExpiryDate >= DateTime.Today &&
                        p.ProductVouchers.Any(pp => pp.ProductId == product.Id));

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
                        VoucherStatus = isInActiveVoucher ? "In Voucher" : "Available"
                    };
                }).ToList(),
                SelectedProductIds = voucher.ProductVouchers?.Select(pp => pp.ProductId).ToList() ?? new List<Guid>()
            };
            SetStatusViewBag(voucher.Status);

            return View(voucherDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(VoucherDTO dto)
        {
            if (dto.ExpiryDate == default)
            {
                ModelState.AddModelError(nameof(dto.ExpiryDate), AppConstants.LogMessages.VoucherExpiryDateCannotBeEmpty);
            }
            if (dto.ExpiryDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(dto.ExpiryDate), AppConstants.LogMessages.VoucherExpiryDateCannotBeInPast);
            }

            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                LayoutHelper.SetViewBagForLayout(this, true, "admin");
                var voucher = await _voucherService.GetByIdAsync(dto.Id);
                var vouchers = await _voucherService.GetAllAsync();
                var products = await _productService.GetAllAsync();
                var voucherDTO = new VoucherDTO
                {
                    Id = voucher.Id,
                    Code = voucher.Code,
                    DiscountPercent = voucher.DiscountPercent,
                    DiscountAmount = voucher.DiscountAmount,
                    ExpiryDate = voucher.ExpiryDate,
                    UsageLimit = voucher.UsageLimit,
                    TimeUsed = voucher.TimeUsed,
                    IsValid = voucher.IsValid,
                    Status = voucher.Status,
                    CreatedAt = voucher.CreatedAt,
                    Products = products.Select(product =>
                    {
                        var isInActiveVoucher = vouchers.Any(p =>
                            p.ExpiryDate >= DateTime.Today &&
                            p.ProductVouchers.Any(pp => pp.ProductId == product.Id));

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
                            VoucherStatus = isInActiveVoucher ? "In Voucher" : "Available"
                        };
                    }).ToList(),
                    SelectedProductIds = voucher.ProductVouchers?.Select(pp => pp.ProductId).ToList() ?? new List<Guid>()
                };
                SetStatusViewBag(dto.Status);
                return View(voucherDTO);
            }

            try
            {
                var (success, message) = await _voucherService.UpdateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateVoucherError;
                    return RedirectToAction("Index", "AdminVoucher");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateVoucherSuccess;
                return RedirectToAction("Index", "AdminVoucher");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateVoucherError;
                return RedirectToAction("Update", "AdminVoucher");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (success, message) = await _voucherService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteVoucherError;
                    return RedirectToAction("Index", "AdminVoucher");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteVoucherSuccess;
                return RedirectToAction("Index", "AdminVoucher");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteVoucherError;
                return RedirectToAction("Index", "AdminVoucher");
            }
        }
    }
}
