using FurnitureProject.Enums;
using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Constants;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Models.Vnpay;
using FurnitureProject.Services;
using FurnitureProject.Services.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VNPAY.NET.Utilities;

namespace FurnitureProject.Controllers
{
    [Route("Vnpay")]
    public class VnpayController : Controller
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        public VnpayController(IVnpay vnPayservice, IConfiguration configuration, IOrderService orderService, ICartService cartService)
        {
            _vnpay = vnPayservice;
            _configuration = configuration;
            _orderService = orderService;
            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            _cartService = cartService;
        }
        /// <summary>
        /// Tạo url thanh toán
        /// </summary>
        /// <param name="money">Số tiền phải thanh toán</param>
        /// <param name="description">Mô tả giao dịch</param>
        /// <returns></returns>
        [HttpGet("CreatePaymentUrl")]
        public ActionResult<string> CreatePaymentUrl(double money, string description)
        {
            description = "Thanh toán đơn hàng";
            try
            {
                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Thực hiện hành động sau khi thanh toán. URL này cần được khai báo với VNPAY để API này hoạt đồng (ví dụ: http://localhost:1234/api/Vnpay/IpnAction)
        /// </summary>
        /// <returns></returns>
        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
        /// <summary>
        /// Trả kết quả thanh toán về cho người dùng
        /// </summary>
        /// <returns></returns>
        [HttpGet("Callback")]
        public async Task<IActionResult> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var userId = HttpContext.Session.GetString("UserID");
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);

                    if (paymentResult.IsSuccess)
                    {
                        var tempOrderJson = TempData["TempOrder"]?.ToString();
                        if (!string.IsNullOrEmpty(tempOrderJson))
                        {
                            var orderDTO = JsonConvert.DeserializeObject<OrderDTO>(tempOrderJson);
                            orderDTO.IsPaid = true;
                            var (success, message) = await _orderService.PaymentAsync(orderDTO);
                            if (!success)
                            {
                                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.OrderPaymentFailed;
                                return RedirectToAction("Index", "Payment");
                            }

                            foreach (var product in orderDTO.Products)
                            {
                                await _cartService.RemoveItemAsync(Guid.Parse(userId), product.Id);
                            }

                            TempData[AppConstants.Status.Success] = AppConstants.LogMessages.OrderPaymentSuccessfully;
                            return RedirectToAction("Index", "Home");
                        }
                    }

                    return RedirectToAction("OrderSuccess", "Order");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Cart");
                }
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}
