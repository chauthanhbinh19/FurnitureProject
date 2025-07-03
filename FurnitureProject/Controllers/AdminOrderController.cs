using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    public class AdminOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
