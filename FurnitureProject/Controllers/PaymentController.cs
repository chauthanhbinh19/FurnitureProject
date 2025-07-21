using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("payment")]
    public class PaymentController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public PaymentController(ICategoryService categoryService, ICartService cartService, IOrderService orderService)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var paymentViewModel = new PaymentViewModel();

            var userId = HttpContext.Session.GetString("UserID");
            var cart = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId));

            //cartViewModel.Cart = cart;

            paymentViewModel.ProductsInCart = cart.CartItems.Select(ci => new ProductDTO
            {
                Id = ci.ProductId,
                Name = ci.Product?.Name,
                Price = ci.UnitPrice,
                Quantity = ci.Quantity,
                ImageUrls = ci.Product?.ProductImages
                                ?.Select(img => img.ImageUrl)
                                .ToList() ?? new List<string>()
            }).ToList();

            return View(paymentViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel paymentViewModel)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("SignIn", "User");
            }

            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId));

                var orderDto = new OrderDTO
                {
                    UserId = Guid.Parse(userId),
                    ReceiverName = paymentViewModel.Order.ReceiverName,
                    ReceiverEmail = paymentViewModel.Order.ReceiverEmail,
                    ReceiverPhone = paymentViewModel.Order.ReceiverPhone,
                    ShippingAddress = paymentViewModel.Order.ShippingAddress,
                    PaymentMethod = paymentViewModel.Order.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    TotalAmount = cart.CartItems.Sum(p => p.UnitPrice * p.Quantity),
                    TotalItems = cart.CartItems.Sum(p => p.Quantity),
                    Products = cart.CartItems.Select(ci => new ProductDTO
                    {
                        Id = ci.ProductId,
                        Name = ci.Product?.Name,
                        Price = ci.UnitPrice,
                        Quantity = ci.Quantity
                    }).ToList()
                };

                var (success, message) = await _orderService.CreateAsync(orderDto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.OrderPaymentFailed;
                    return RedirectToAction("Index", "Home");
                }

                foreach(var product in orderDto.Products)
                {
                    await _cartService.RemoveItemAsync(Guid.Parse(userId), product.Id);
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.OrderPaymentSuccessfully;
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.OrderPaymentFailed;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
