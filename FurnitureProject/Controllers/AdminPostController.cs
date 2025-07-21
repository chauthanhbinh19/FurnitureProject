using FurnitureProject.Helper;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/post")]
    public class AdminPostController : Controller
    {
        private readonly ICartService _cartService;
        public AdminPostController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            return View();
        }
    }
}
