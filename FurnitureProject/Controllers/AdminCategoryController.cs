using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/category")]
    public class AdminCategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public AdminCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        private void GetUserInformationFromSession()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
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
        public async Task<IActionResult> Index(CategoryFilterDTO filter, int page = 1)
        {
            GetUserInformationFromSession();

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
                    .Where(u => u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                                u.Description.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
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
            switch (filter.SortOrder)
            {
                case "newest":
                    categoryDTOs = categoryDTOs.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "oldest":
                    categoryDTOs = categoryDTOs.OrderBy(p => p.CreatedAt).ToList();
                    break;
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
            SetSortOptions(filter.SortOrder);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalCategories = totalCategories;
            return View(categoryViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            if (result == null) return NotFound();
            SetStatusViewBag(result.Status);
            return Ok(result);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            GetUserInformationFromSession();
            SetStatusViewBag();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                GetUserInformationFromSession();
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
            catch (Exception ex) {
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateCategoryError;
                return RedirectToAction("Create", "AdminCategory");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            GetUserInformationFromSession();
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
                GetUserInformationFromSession();
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
                    return RedirectToAction("Update", "AdminCategory");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateCategorySuccess;
                return RedirectToAction("Index", "AdminCategory");
            }
            catch (Exception ex) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateCategoryError;
                return RedirectToAction("Update", "AdminCategory");
            }
            
        }

        [HttpDelete("{id}")]
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
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteCategoryError;
                return RedirectToAction("Index", "AdminCategory");
            }
        }
    }

}
