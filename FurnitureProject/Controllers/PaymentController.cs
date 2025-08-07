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
        private readonly IConfiguration _config;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IPromotionService _promotionService;
        private readonly IAddressService _addressService;
        public PaymentController(ICategoryService categoryService, ICartService cartService, 
            IOrderService orderService, IProductService productService, IPromotionService promotionService, 
            IAddressService addressService, IConfiguration config)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _productService = productService;
            _promotionService = promotionService;
            _addressService = addressService;
            _config = config;
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
            var products = await _productService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();
            var today = DateTime.UtcNow;
            var addresses = await _addressService.GetUserAddressesAsync(Guid.Parse(userId));

            paymentViewModel.ProductsInCart = cart.CartItems.Select(ci => 
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

            paymentViewModel.Order = new OrderDTO
            {
                Addresses = addresses.Select(a => new AddressDTO
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    Street = a.Street,
                    Ward = a.Ward,
                    District = a.District,
                    City = a.City,
                    Country = a.Country,
                    PostalCode = a.PostalCode,
                    IsDefault = a.IsDefault
                }).ToList()
            };

            return View(paymentViewModel);
        }
        [HttpPost("process-payment")]
        public async Task<IActionResult> ProcessPayment(PaymentViewModel paymentViewModel)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.MustSignIn;
                return RedirectToAction("SignIn", "User");
            }

            //if (!ModelState.IsValid)
            //{
            //    return RedirectToAction("Index", "Payment");
            //}

            try
            {
                var cart = await _cartService.GetCartByUserIdAsync(Guid.Parse(userId));
                var products = await _productService.GetAllAsync();
                var promotions = await _promotionService.GetAllAsync();
                var today = DateTime.UtcNow;

                var orderDto = new OrderDTO
                {
                    UserId = Guid.Parse(userId),
                    ReceiverName = paymentViewModel.Order.ReceiverName,
                    ReceiverEmail = paymentViewModel.Order.ReceiverEmail,
                    ReceiverPhone = paymentViewModel.Order.ReceiverPhone,
                    AddressId = paymentViewModel.Order.AddressId,
                    ShippingMethodId = paymentViewModel.Order.ShippingMethodId,
                    PaymentMethod = paymentViewModel.Order.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = "pending",
                    TotalItems = cart.CartItems.Sum(p => p.Quantity),
                    Products = cart.CartItems.Select(ci =>
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
                    }).ToList(),
                };

                orderDto.TotalAmount = orderDto.Products.Sum(p => 
                    (p.DiscountPrice > 0 ? p.DiscountPrice : p.Price) * p.Quantity
                );

                var (success, message) = await _orderService.PaymentAsync(orderDto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.OrderPaymentFailed;
                    return RedirectToAction("Index", "Payment");
                }

                foreach(var product in orderDto.Products)
                {
                    await _cartService.RemoveItemAsync(Guid.Parse(userId), product.Id);
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.OrderPaymentSuccessfully;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.OrderPaymentFailed;
                return RedirectToAction("Index", "Payment");
            }
        }
    }
}
