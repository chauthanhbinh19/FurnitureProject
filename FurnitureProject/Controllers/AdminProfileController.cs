using FurnitureProject.Helper;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/profile")]
    public class AdminProfileController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        public AdminProfileController(ICategoryService categoryService, ICartService cartService, IOrderService orderService, IUserService userService)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            return View();
        }
    }
}
