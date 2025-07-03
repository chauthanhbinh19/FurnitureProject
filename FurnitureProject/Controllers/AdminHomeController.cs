using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
