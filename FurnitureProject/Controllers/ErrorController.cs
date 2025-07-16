using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("error")]
    public class ErrorController : Controller
    {
        [Route("not-found")]
        public IActionResult NotFound()
        {
            return View("NotFound");
        }

        [Route("server-error")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }

        [Route("maintenance")]
        public IActionResult Maintenance()
        {
            return View("ServerError");
        }

        [Route("forbidden")]
        public IActionResult Forbidden()
        {
            return View("Forbidden");
        }
    }
}
