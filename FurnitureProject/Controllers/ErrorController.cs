using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = false;
            ViewBag.LayoutType = "user";
        }
        [Route("not-found")]
        public IActionResult NotFound()
        {
            SetViewBagForLayout();
            return View("NotFound");
        }

        [Route("server-error")]
        public IActionResult ServerError()
        {
            SetViewBagForLayout();
            return View("ServerError");
        }

        [Route("maintenance")]
        public IActionResult Maintenance()
        {
            SetViewBagForLayout();
            return View("ServerError");
        }

        [Route("forbidden")]
        public IActionResult Forbidden()
        {
            SetViewBagForLayout();
            return View("Forbidden");
        }
    }
}
