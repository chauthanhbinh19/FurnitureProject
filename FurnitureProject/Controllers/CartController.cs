using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using FurnitureProject.Services.Session;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FurnitureProject.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        public CartController(IProductService productService,ICategoryService categoryService, ICartService cartService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
        }
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
            ViewBag.LayoutType = "user";
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            GetUserInformationFromSession();
            SetViewBagForLayout();

            var cartViewModel = new CartViewModel();

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");

            // Lấy giỏ hàng
            if (userId != null)
            {
                var cart = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId));

                cartViewModel.ProductsInCart = cart.CartItems.Select(ci => new ProductDTO
                {
                    Id = ci.ProductId,
                    Name = ci.Product?.Name,
                    Price = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    ImageUrls = ci.Product?.ProductImages
                                    ?.Select(img => img.ImageUrl)
                                    .ToList() ?? new List<string>()
                }).ToList();
            }
            return View(cartViewModel);
        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("Index", "Home");
            }
            
            try
            {
                var (success, message) = await _cartService.CreateCartAsync(Guid.Parse(userId), productId, quantity);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemNotFound;
                    return RedirectToAction("Index", "Home");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CartItemAdded;
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemNotFound;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
