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
                //new SelectListItem { Text = "Giá tăng dần", Value = "price-asc" },
                //new SelectListItem { Text = "Giá giảm dần", Value = "price-desc" }
            };

            ViewBag.SortOptions = new SelectList(sortOptions, "Value", "Text", selectedSort);
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CategoryFilterDTO filter, int page = 1)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

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
            return Ok(result);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Category category)
        {
            await _categoryService.CreateAsync(category);
            return RedirectToAction("Index", "AdminCategory");
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var category = await _categoryService.GetByIdAsync(id);
            return View(category);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Category category)
        {
            await _categoryService.UpdateAsync(category);
            return RedirectToAction("Index", "AdminCategory");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
    }

}
