using FurnitureProject.Models;
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
                new SelectListItem {Text = "Unactive", Value = "unactive", Selected = selectedStatus == "unactive"},
            };
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");

            int pageSize = 10;
            var users = await _userService.GetAllAsync();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                users = users
                    .Where(u => u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalUsers = users.Count();
            var pagedUsers = users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalUsers = totalUsers;
            return View(pagedUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Update(int id)
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
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
