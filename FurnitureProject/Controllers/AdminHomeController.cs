using FurnitureProject.Helper;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/home")]
    public class AdminHomeController : Controller
    {
        public readonly ICartService _cartService;
        public AdminHomeController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Route("")]
        public async Task<IActionResult> IndexAsync()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            return View();
        }
    }
}
