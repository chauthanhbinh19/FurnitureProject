using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace FurnitureProject.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IPromotionService _promotionService;
        private readonly IAddressService _addressService;
        public OrderController(ICategoryService categoryService, ICartService cartService,
            IOrderService orderService, IProductService productService, IPromotionService promotionService,
            IAddressService addressService)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _productService = productService;
            _promotionService = promotionService;
            _addressService = addressService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("order-success")]
        public IActionResult OrderSuccess()
        {
            return View();
        }
        [HttpPost("store-temp-order")]
        public async Task<IActionResult> StoreTempOrder(PaymentViewModel paymentViewModel)
        {
            var userId = HttpContext.Session.GetString("UserID");
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

            TempData["TempOrder"] = JsonConvert.SerializeObject(orderDto);

            // Trả về tổng số tiền (hoặc thông tin cần thiết để thanh toán)
            return Json(new { amount = orderDto.TotalAmount });
        }
    }
}
