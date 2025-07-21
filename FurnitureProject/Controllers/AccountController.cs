using FurnitureProject.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("user")]
    public class AccountController : Controller
    {
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpPost("profile")]
        public IActionResult UpdateProfile()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("orders/{id}")]
        public IActionResult OrderDetail(int id)
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("wishlist")]
        public IActionResult Wishlist()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("vouchers")]
        public IActionResult Vouchers()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("addresses")]
        public IActionResult Addresses()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpPost("addresses")]
        public IActionResult UpdateAddresses()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }

        [HttpGet("notifications")]
        public IActionResult Notifications()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            return View();
        }
    }
}
