using FurnitureProject.Models;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/promotion")]
    public class AdminPromotionController : Controller
    {
        private readonly IPromotionService _promotionService;

        public AdminPromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserID");
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            var list = await _promotionService.GetAllAsync();
            return View(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            await _promotionService.CreateAsync(promotion);
            return CreatedAtAction(nameof(GetById), new { id = promotion.Id }, promotion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Promotion promotion)
        {
            if (id != promotion.Id) return BadRequest();
            await _promotionService.UpdateAsync(promotion);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _promotionService.DeleteAsync(id);
            return NoContent();
        }
    }

}
