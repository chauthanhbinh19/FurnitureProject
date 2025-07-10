using CloudinaryDotNet;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/tag")]
    public class AdminTagController : Controller
    {
        private readonly ITagService _tagService;

        public AdminTagController(ITagService tagService)
        {
            _tagService = tagService;
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
        [Route("")]
        public async Task<IActionResult> Index(TagFilterDTO filter, int page = 1)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var tags = await _tagService.GetAllAsync();

            var tagDTOs = tags.Select(tag => new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name,
                Status = tag.Status,
                CreatedAt = tag.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                tagDTOs = tagDTOs
                    .Where(u => u.Name.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                tagDTOs = tagDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Sort Order
            switch (filter.SortOrder)
            {
                case "newest":
                    tagDTOs = tagDTOs.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "oldest":
                    tagDTOs = tagDTOs.OrderBy(p => p.CreatedAt).ToList();
                    break;
            }

            int totalTags = tagDTOs.Count();
            var pagedTags = tagDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var tagViewModel = new TagViewModel
            {
                Tags = pagedTags,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);
            SetSortOptions(filter.SortOrder);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalTags = totalTags;
            return View(tagViewModel);
        }
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(Tag tag)
        {
            await _tagService.CreateAsync(tag);
            return RedirectToAction("Index", "AdminTag");
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var tag = await _tagService.GetByIdAsync(id);
            return View(tag);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(Tag tag)
        {
            await _tagService.UpdateAsync(tag);
            return RedirectToAction("Index", "AdminTag");
        }
    }
}
