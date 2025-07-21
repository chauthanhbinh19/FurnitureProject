using FurnitureProject.Helper;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("not-found")]
        public IActionResult NotFound()
        {
            LayoutHelper.SetViewBagForLayout(this, false, "admin");
            return View("NotFound");
        }

        [Route("server-error")]
        public IActionResult ServerError()
        {
            LayoutHelper.SetViewBagForLayout(this, false, "admin");
            return View("ServerError");
        }

        [Route("maintenance")]
        public IActionResult Maintenance()
        {
            LayoutHelper.SetViewBagForLayout(this, false, "admin");
            return View("ServerError");
        }

        [Route("forbidden")]
        public IActionResult Forbidden()
        {
            LayoutHelper.SetViewBagForLayout(this, false, "admin");
            return View("Forbidden");
        }
    }
}
