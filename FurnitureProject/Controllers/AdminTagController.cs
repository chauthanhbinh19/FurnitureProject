using CloudinaryDotNet;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

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
        [Route("")]
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var tags = await _tagService.GetAllAsync();

            var tagDTOs = tags.Select(tag => new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name,
                Status = tag.Status
            }).ToList();
            // Search
            if (!string.IsNullOrEmpty(search))
            {
                tagDTOs = tagDTOs
                    .Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalTags = tagDTOs.Count();
            var pagedTags = tagDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalTags = totalTags;
            return View(pagedTags);
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
