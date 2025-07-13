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
    [Route("admin/user")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }
        private void GetUserInformationFromSession()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
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
        private void SetRoleViewBag(string? status = null)
        {
            ViewBag.RoleList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Admin, Text = AppConstants.LogMessages.Admin },
                    new { Value = AppConstants.Status.User, Text = AppConstants.LogMessages.User }
                },
                "Value", "Text", status
            );
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(UserFilterDTO filter, int page = 1)
        {
            GetUserInformationFromSession();

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
            GetUserInformationFromSession();
            SetRoleViewBag();
            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(User user)
        {
            try
            {
                var (success, message) = await _userService.CreateAsync(user);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateUserError;
                    return RedirectToAction("Create", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateUserError;
                return RedirectToAction("Create", "AdminUser");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            GetUserInformationFromSession();
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            TempData["UserPassword"] = user.Password;
            SetRoleViewBag(user.Role);
            return View(user);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(User user)
        {
            
            try
            {
                if (user.Password == null)
                {
                    user.Password = TempData["UserPassword"]?.ToString();
                }

                var (success, message) = await _userService.UpdateAsync(user);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateUserError;
                    return RedirectToAction("Update", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateUserError;
                return RedirectToAction("Update", "AdminUser");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (success, message) = await _userService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteUserError;
                    return RedirectToAction("Index", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception ex) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteUserError;
                return RedirectToAction("Index", "AdminUser");
            }
        }
    }
}
