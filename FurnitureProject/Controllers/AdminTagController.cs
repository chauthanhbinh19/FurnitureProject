using CloudinaryDotNet;
using FurnitureProject.Helper;
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
        private void GetUserInformationFromSession()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
        }
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = true;
            ViewBag.LayoutType = "admin";
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
        [Route("")]
        public async Task<IActionResult> Index(TagFilterDTO filter, int page = 1)
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();

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
            GetUserInformationFromSession();
            SetViewBagForLayout();
            SetStatusViewBag();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(TagDTO dto)
        {
            if (!ModelState.IsValid)
            {
                GetUserInformationFromSession();
                SetStatusViewBag(dto.Status);
                return View(dto);
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt
            };

            try
            {
                var(success, message) = await _tagService.CreateAsync(tag);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateTagError;
                    return RedirectToAction("Create", "AdminTag");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateTagSuccess;
                return RedirectToAction("Index", "AdminTag");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateTagError;
                return RedirectToAction("Create", "AdminTag");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();
            var tag = await _tagService.GetByIdAsync(id);
            SetStatusViewBag(tag.Status);
            var tagDTO = new TagDTO
            {
                Id = tag.Id,
                Name = tag.Name,
                Status = tag.Status,
                CreatedAt = tag.CreatedAt
            };
            return View(tagDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(TagDTO dto)
        {

            if (!ModelState.IsValid)
            {
                GetUserInformationFromSession();
                SetStatusViewBag(dto.Status);
                return View(dto);
            }

            var tag = new Tag
            {
                Id = dto.Id,
                Name = dto.Name,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var(success, message) = await _tagService.UpdateAsync(tag);
                if (!success) {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateTagError;
                    return RedirectToAction("Update", "AdminTag");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateTagSuccess;
                return RedirectToAction("Index", "AdminTag");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateTagError;
                return RedirectToAction("Update", "AdminTag");
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var(success, message) = await _tagService.DeleteAsync(id);
                if (!success){
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteTagError;
                    return RedirectToAction("Index", "AdminTag");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteTagSuccess;
                return RedirectToAction("Index", "AdminTag");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteTagError;
                return RedirectToAction("Index", "AdminTag");
            }
        }
    }
}
