using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using FurnitureProject.Services.Session;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FurnitureProject.Constants;
using System.Security.Claims;

namespace FurnitureProject.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IPromotionService _promotionService;
        public CartController(IProductService productService,ICategoryService categoryService, ICartService cartService, IPromotionService promotionService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
            _promotionService = promotionService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var cartViewModel = new CartViewModel();

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;

            if (userId != null)
            {
                var cart = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId));

                cartViewModel.Cart = cart;

                var products = await _productService.GetAllAsync();

                cartViewModel.ProductsInCart = cart.CartItems.Select(ci =>
                {
                    var product = products.FirstOrDefault(p => p.Id == ci.ProductId);

                    var activePromotion = promotions.FirstOrDefault(promo =>
                        promo.ProductPromotions.Any(pp => pp.ProductId == product.Id) &&
                        promo.EndDate >= today
                    );

                    decimal discountPrice = 0;
                    if (activePromotion != null)
                    {
                        var discount = activePromotion.DiscountPercent;
                        discountPrice = product.Price * (1 - discount / 100m);
                    }

                    return new ProductDTO
                    {
                        Id = ci.ProductId,
                        Name = product?.Name,
                        Price = product?.Price ?? 0,
                        Quantity = ci.Quantity,
                        ImageUrls = product?.ProductImages?
                                            .Select(img => img.ImageUrl)
                                            .ToList() ?? new List<string>(),
                        DiscountPrice = discountPrice,
                    };
                }).ToList();
            }
            return View(cartViewModel);
        }
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity, string returnUrl)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("SignIn", "User");
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
                return Redirect(returnUrl ?? "/");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemNotFound;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("ajax/add-to-cart")]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Vui lòng đăng nhập để sử dụng chức năng yêu thích.",
                    redirectUrl = Url.Action("Signin", "User") // --> /user/signin
                });
            }

            try
            {
                var (success, message) = await _cartService.CreateCartAsync(Guid.Parse(userId), productId, quantity);
                if (!success)
                {
                    return Json(new { success = false});
                }

                return Json(new { success = true});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, status = ex.Message });
            }
        }
        [HttpPost("update-quantity")]
        public async Task<IActionResult> UpdateQuantity(Guid id, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("SignIn", "User");
            }

            try
            {
                var (success, message) = await _cartService.UpdateItemQuantityAsync(Guid.Parse(userId), id, quantity);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemNotFound;
                    return RedirectToAction("Index", "Home");
                }

                //TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CartItemUpdated;
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemNotFound;
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id, int quantity)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("SignIn", "User");
            }

            try
            {
                var (success, message) = await _cartService.RemoveItemAsync(Guid.Parse(userId), id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemRemoveFailed;
                    return RedirectToAction("Index", "Home");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CartItemRemoved;
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CartItemRemoveFailed;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
