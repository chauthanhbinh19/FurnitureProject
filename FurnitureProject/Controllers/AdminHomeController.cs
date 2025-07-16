using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/home")]
    public class AdminHomeController : Controller
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
        [Route("")]
        public IActionResult Index()
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();
            return View();
        }
    }
}
