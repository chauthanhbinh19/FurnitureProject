using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("support")]
    public class SupportController : Controller
    {
        private readonly ICategoryService _categoryService;

        public SupportController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        private void SetViewBagForLayout()
        {
            ViewBag.UseLayout = true;
            ViewBag.LayoutType = "user";
        }
        [HttpGet("about")]
        public async Task<IActionResult> About()
        {
            SetViewBagForLayout();
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("contact")]
        public async Task<IActionResult> Contact()
        {
            SetViewBagForLayout();
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("faq")]
        public async Task<IActionResult> Faq()
        {
            SetViewBagForLayout();
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("privacy-policy")]
        public async Task<IActionResult> PrivacyPolicy()
        {
            SetViewBagForLayout();
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("terms")]
        public async Task<IActionResult> Terms()
        {
            SetViewBagForLayout();
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }
    }
}
