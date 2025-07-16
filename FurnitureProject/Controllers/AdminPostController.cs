using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/post")]
    public class AdminPostController : Controller
    {
        private void GetUserInformationFromSession()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserFullName = HttpContext.Session.GetString("UserFullName");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
        }
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = true;
            ViewBag.LayoutType = "admin";
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();
            return View();
        }
    }
}
