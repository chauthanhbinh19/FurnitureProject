using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("user")]
    public class AccountController : Controller
    {
        [HttpGet("profile")]
        public IActionResult Profile() => View();

        [HttpPost("profile")]
        public IActionResult UpdateProfile()
        {
            return View();
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            return View();
        }

        [HttpGet("orders/{id}")]
        public IActionResult OrderDetail(int id)
        {
            return View();
        }

        [HttpGet("wishlist")]
        public IActionResult Wishlist()
        {
            return View();
        }

        [HttpGet("vouchers")]
        public IActionResult Vouchers()
        {
            return View();
        }

        [HttpGet("addresses")]
        public IActionResult Addresses()
        {
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
            return View();
        }
    }
}
