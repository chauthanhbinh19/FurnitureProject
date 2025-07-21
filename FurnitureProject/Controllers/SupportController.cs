using FurnitureProject.Helper;
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
        [HttpGet("about")]
        public async Task<IActionResult> About()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("contact")]
        public async Task<IActionResult> Contact()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("faq")]
        public async Task<IActionResult> Faq()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("privacy-policy")]
        public async Task<IActionResult> PrivacyPolicy()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }

        [HttpGet("terms")]
        public async Task<IActionResult> Terms()
        {
            LayoutHelper.SetViewBagForLayout(this, true, "user");
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return View();
        }
    }
}
