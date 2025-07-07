using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("support")]
    public class SupportController : Controller
    {
        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpGet("faq")]
        public IActionResult Faq()
        {
            return View();
        }

        [HttpGet("privacy-policy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [HttpGet("terms")]
        public IActionResult Terms()
        {
            return View();
        }
    }
}
