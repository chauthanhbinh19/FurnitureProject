using FurnitureProject.Helper;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FurnitureProject.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public AccountController(ICategoryService categoryService, ICartService cartService, IOrderService orderService, IUserService userService)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
        }
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            //await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            //LayoutHelper.SetViewBagForLayout(this, true, "user");

            //var categories = await _categoryService.GetAllAsync();
            //ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return RedirectToAction("Profile");
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _userService.GetByIdAsync(Guid.Parse(userId));

            var model = new AccountViewModel
            {
                CurrentSection = "Profile",
                Profile = new UserDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                }
            };

            return View("Index", model);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            int pageSize = 10;

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetAllByUserIdAsync(Guid.Parse(userId));

            var model = new AccountViewModel
            {
                CurrentSection = "Orders",
                OrderViewModel = new OrderViewModel
                {
                    Orders = orders.Select(order => new OrderDTO
                    {
                        Id = order.Id,
                        User = order.User,
                        ReceiverName = order.ReceiverName,
                        ReceiverEmail = order.ReceiverEmail,
                        ReceiverPhone = order.ReceiverPhone,
                        ShippingAddress = order.ShippingAddress,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        TotalAmount = order.TotalAmount,
                        TotalItems = order.OrderItems.Count(),
                        CreatedAt = order.CreatedAt,
                    }).ToList()
                }
            };

            int totalOrders = model.OrderViewModel.Orders.Count();
            var pagedOrders = model.OrderViewModel.Orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            //ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalOrders = totalOrders;
            return View("Index", model);
        }

        [HttpGet("vouchers")]
        public async Task<IActionResult> Vouchers()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var model = new AccountViewModel
            {
                CurrentSection = "Vouchers",
                //Vouchers = _voucherService.GetByUserId(Guid.Parse(userId))
            };

            return View("Index", model);
        }

    }
}
