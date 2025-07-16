using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("user")]
    public class AccountController : Controller
    {
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = true;
            ViewBag.LayoutType = "user";
        }
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpPost("profile")]
        public IActionResult UpdateProfile()
        {
            return View();
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpGet("orders/{id}")]
        public IActionResult OrderDetail(int id)
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpGet("wishlist")]
        public IActionResult Wishlist()
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpGet("vouchers")]
        public IActionResult Vouchers()
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpGet("addresses")]
        public IActionResult Addresses()
        {
            SetViewBagForLayout();
            return View();
        }

        [HttpPost("addresses")]
        public IActionResult UpdateAddresses()
        {
            return View();
        }

        [HttpGet("notifications")]
        public IActionResult Notifications()
        {
            SetViewBagForLayout();
            return View();
        }
    }
}
