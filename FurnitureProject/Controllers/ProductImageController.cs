using FurnitureProject.Models;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/[controller]")]
    public class ProductImageController : Controller
    {
        private readonly IProductImageService _imageService;

        public ProductImageController(IProductImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _imageService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _imageService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var result = await _imageService.GetByProductIdAsync(productId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductImage image)
        {
            await _imageService.CreateAsync(image);
            return Ok(image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _imageService.DeleteAsync(id);
            return NoContent();
        }
    }

}
