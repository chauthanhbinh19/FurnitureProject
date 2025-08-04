using FurnitureProject.Helper;
using FurnitureProject.Models.DTO;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/home")]
    public class AdminHomeController : Controller
    {
        public readonly ICartService _cartService;
        public readonly IOrderService _orderService;
        public readonly IUserService _userService;
        public readonly IProductService _productService;
        public AdminHomeController(ICartService cartService, IOrderService orderService, 
            IUserService userService, IProductService productService)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
        }
        [Route("")]
        public async Task<IActionResult> IndexAsync(int range = 30)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            var toDate = DateTime.UtcNow;
            var fromDate = toDate.AddDays(-range);
            var startDate = DateTime.Today.AddDays(-6);

            ViewBag.SelectedRange = range;

            var totalRevenue = await _orderService.GetTotalRevenueAsync(fromDate, toDate);
            var totalTransactions = await _orderService.GetTotalTransactionsAsync(fromDate, toDate);
            var totalUsers = await _userService.GetTotalUsersAsync(fromDate, toDate);
            var totalProducts = await _productService.GetTotalProductsAsync(fromDate, toDate);
            var topCustomers = await _orderService.GetTopCustomersAsync(fromDate, toDate);
            var topProducts = await _orderService.GetTopProductsAsync(fromDate, toDate);
            var ordersByStatus = await _orderService.GetOrderCountByStatusAsync(fromDate, toDate);
            var revenueData = await _orderService.GetRevenueByDateAsync(startDate, toDate);

            var topFilteredCustomers = topCustomers
                .Select(t => new UserDTO
                {
                    Id = t.UserId,
                    FullName = t.FullName,
                    OrderCount = t.OrderCount
                }).ToList();

            while (topFilteredCustomers.Count < 6)
            {
                topFilteredCustomers.Add(new UserDTO
                {
                    Id = Guid.Empty,
                    FullName = "Chưa có",
                    OrderCount = 0
                });
            }

            var chartData = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i))
                .ToDictionary(
                    date => date.ToString("dd-MM-yyyy"),
                    date => revenueData.TryGetValue(date, out var value) ? value : 0m
                );

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalTransactions = totalTransactions;
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TopCustomers = topFilteredCustomers;
            ViewBag.TopProducts = topProducts;
            ViewBag.OrderStatusCounts = ordersByStatus;
            ViewBag.RevenueByDate = chartData;

            return View();
        }
    }
}
