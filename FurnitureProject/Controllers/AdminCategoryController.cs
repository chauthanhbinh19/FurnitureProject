using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using FurnitureProject.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/category")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;

        public AdminCategoryController(ICategoryService categoryService, ICartService cartService)
        {
            _categoryService = categoryService;
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

        [HttpGet("")]
        public async Task<IActionResult> Index(CategoryFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var categories = await _categoryService.GetAllAsync();

            var categoryDTOs = categories.Select(category => new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status,
                CreatedAt = category.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                categoryDTOs = categoryDTOs
                    .Where(u => (!string.IsNullOrEmpty(u.Name) && u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                                (!string.IsNullOrEmpty(u.Description) && u.Description.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                categoryDTOs = categoryDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                categoryDTOs = filter.SortColumn switch
                {
                    "Name" => isAscending
                        ? categoryDTOs.OrderBy(p => p.Name).ToList()
                        : categoryDTOs.OrderByDescending(p => p.Name).ToList(),

                    "Description" => isAscending
                        ? categoryDTOs.OrderBy(p => p.Description).ToList()
                        : categoryDTOs.OrderByDescending(p => p.Description).ToList(),

                    "CreatedAt" => isAscending
                        ? categoryDTOs.OrderBy(p => p.CreatedAt).ToList()
                        : categoryDTOs.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? categoryDTOs.OrderBy(p => p.Status).ToList()
                        : categoryDTOs.OrderByDescending(p => p.Status).ToList(),

                    _ => categoryDTOs
                };
            }


            int totalCategories = categoryDTOs.Count();
            var pagedCategories = categoryDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var categoryViewModel = new CategoryViewModel
            {
                Categories = pagedCategories,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);
            //SetSortOptions(filter.SortOrder);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalCategories = totalCategories;
            return View(categoryViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            SetStatusViewBag(result.Status);
            return Ok(result);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            SetStatusViewBag();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                SetStatusViewBag(dto.Status);
                return View(dto);
            }

            var category = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt // Ensure CreatedAt is set if not provided
            };

            try
            {
                var(success, message) = await _categoryService.CreateAsync(category);
                if (!success)
                {
                    TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateCategoryError;
                    return RedirectToAction("Create", "AdminCategory");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateCategorySuccess;
                return RedirectToAction("Index", "AdminCategory");
            }
            catch (Exception) {
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateCategoryError;
                return RedirectToAction("Create", "AdminCategory");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var category = await _categoryService.GetByIdAsync(id);
            SetStatusViewBag(category.Status);
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status,
                CreatedAt = category.CreatedAt,
            };
            return View(categoryDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                var tempCategory = await _categoryService.GetByIdAsync(dto.Id);
                SetStatusViewBag(tempCategory.Status);
                return View(dto);
            }

            var category = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt ?? DateTime.UtcNow // Ensure CreatedAt is set if not provided
            };

            try
            {
                var(success, message) = await _categoryService.UpdateAsync(category);
                if (!success) {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateCategoryError;
                    return RedirectToAction("Index", "AdminCategory");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateCategorySuccess;
                return RedirectToAction("Index", "AdminCategory");
            }
            catch (Exception) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateCategoryError;
                return RedirectToAction("Update", "AdminCategory");
            }
            
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var(success, message) = await _categoryService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteCategoryError;
                    return RedirectToAction("Index", "AdminCategory");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteCategorySuccess;
                return RedirectToAction("Index", "AdminCategory");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteCategoryError;
                return RedirectToAction("Index", "AdminCategory");
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var category = await _categoryService.GetByIdAsync(id);
            SetStatusViewBag(category.Status);
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status,
                CreatedAt = category.CreatedAt,
            };
            return View(categoryDTO);
        }
    }

}
