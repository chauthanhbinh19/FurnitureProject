using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/user")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }
        private void SetViewBags(string? selectedRole = null, string? selectedStatus = null)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem {Text = "Admin", Value = "admin", Selected = selectedStatus == "admin"},
                new SelectListItem {Text = "User", Value = "user", Selected = selectedStatus == "user"},
            };

            ViewBag.Status = new List<SelectListItem>
            {
                new SelectListItem {Text = "Active", Value = "active", Selected = selectedStatus == "active"},
                new SelectListItem {Text = "Inactive", Value = "inactive", Selected = selectedStatus == "inactive"},
            };
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
        private void SetRoleViewBag(string? status = null)
        {
            ViewBag.RoleList = new SelectList(
                new[] {
                    new { Value = "admin", Text = "Admin" },
                    new { Value = "user", Text = "User" }
                },
                "Value", "Text", status
            );
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(UserFilterDTO filter, int page = 1)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var users = await _userService.GetAllAsync();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                users = users
                    .Where(u => u.FullName.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                            u.Email.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                users = users
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Filter by role
            if (filter.FilterByRole != null && filter.FilterByRole.Any())
            {
                users = users
                   .Where(p => !string.IsNullOrEmpty(p.Role) && filter.FilterByRole.Equals(p.Role))
                   .ToList();
            }

            // Sort Order
            switch (filter.SortOrder)
            {
                case "newest":
                    users = users.OrderByDescending(p => p.CreatedAt).ToList();
                    break;
                case "oldest":
                    users = users.OrderBy(p => p.CreatedAt).ToList();
                    break;
            }

            int totalUsers = users.Count();
            var pagedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userViewModel = new UserViewModel
            {
                Users = pagedUsers,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);
            SetSortOptions(filter.SortOrder);
            SetRoleViewBag(filter.FilterByRole);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalUsers = totalUsers;
            return View(userViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            SetViewBags();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(User user)
        {
            await _userService.CreateAsync(user);
            return RedirectToAction("Index","AdminUser");
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            TempData["UserPassword"] = user.Password;
            SetViewBags(user.Role, user.Status);
            return View(user);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(User user)
        {
            if(user.Password == null)
            {
                user.Password = TempData["UserPassword"]?.ToString();
            }
            
            await _userService.UpdateAsync(user);
            return RedirectToAction("Index", "AdminUser");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
