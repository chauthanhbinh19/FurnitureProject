using FurnitureProject.Helper;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/order")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public AdminOrderController(IOrderService orderService, IUserService userService, 
            IProductService productService, ICartService cartService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
            _cartService = cartService;
        }
        private void SetStatusViewBag(string? status = null)
        {
            ViewBag.StatusList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Active, Text = AppConstants.LogMessages.Active },
                    new { Value = AppConstants.Status.Inactive, Text = AppConstants.LogMessages.Inactive }
                },
                "Value", "Text", status
            );
        }
        private void SetSortOptions(string? selectedSort = null)
        {
            var sortOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = AppConstants.LogMessages.Newest, Value = AppConstants.Status.Newest },
                new SelectListItem { Text = AppConstants.LogMessages.Oldest, Value = AppConstants.Status.Oldest },
                //new SelectListItem { Text = AppConstants.LogMessages.PriceAscending, Value = AppConstants.Status.PriceAscending },
                //new SelectListItem { Text = AppConstants.LogMessages.PriceDescending, Value = AppConstants.Status.PriceDescending }
            };

            ViewBag.SortOptions = new SelectList(sortOptions, "Value", "Text", selectedSort);
        }
        [Route("")]
        public async Task<IActionResult> Index(OrderFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var orders = await _orderService.GetAllAsync();

            var orderDTOs = orders.Select(order => new OrderDTO
            {
                Id = order.Id,
                User = order.User,
                ReceiverName = order.ReceiverName,
                ReceiverEmail = order.ReceiverEmail,
                ReceiverPhone = order.ReceiverPhone,
                //ShippingAddress = order.ShippingAddress,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                TotalItems = order.OrderItems.Count(),
                CreatedAt = order.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                orderDTOs = orderDTOs
                    .Where(u => u.User.FullName.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                orderDTOs = orderDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                orderDTOs = filter.SortColumn switch
                {
                    "ReceiverName" => isAscending
                        ? orderDTOs.OrderBy(p => p.ReceiverName).ToList()
                        : orderDTOs.OrderByDescending(p => p.ReceiverName).ToList(),

                    "ReceiverEmail" => isAscending
                        ? orderDTOs.OrderBy(p => p.ReceiverEmail).ToList()
                        : orderDTOs.OrderByDescending(p => p.ReceiverEmail).ToList(),

                    "ReceiverPhone" => isAscending
                        ? orderDTOs.OrderBy(p => p.ReceiverPhone).ToList()
                        : orderDTOs.OrderByDescending(p => p.ReceiverPhone).ToList(),

                    "OrderDate" => isAscending
                        ? orderDTOs.OrderBy(p => p.OrderDate).ToList()
                        : orderDTOs.OrderByDescending(p => p.OrderDate).ToList(),

                    "Status" => isAscending
                        ? orderDTOs.OrderBy(p => p.Status).ToList()
                        : orderDTOs.OrderByDescending(p => p.Status).ToList(),

                    "TotalAmount" => isAscending
                        ? orderDTOs.OrderBy(p => p.TotalAmount).ToList()
                        : orderDTOs.OrderByDescending(p => p.TotalAmount).ToList(),

                    "TotalItems" => isAscending
                        ? orderDTOs.OrderBy(p => p.TotalItems).ToList()
                        : orderDTOs.OrderByDescending(p => p.TotalItems).ToList(),

                    _ => orderDTOs
                };
            }

            int totalOrders = orderDTOs.Count();
            var pagedOrders = orderDTOs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var orderViewModel = new OrderViewModel
            {
                Orders = pagedOrders,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalOrders = totalOrders;
            return View(orderViewModel);
        }
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            var users = await _userService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            ViewBag.Users = new SelectList(users, "Id", "FullName");
            ViewBag.Products = products;

            //await SetCategoryViewBag();
            //await SetTagViewBag();

            return View();
            //return View(new OrderDTO
            //{
            //    Or = new List<OrderItemInputModel> { new OrderItemInputModel() } // khởi tạo 1 dòng mặc định
            //});
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(OrderDTO dto)
        {
            try
            {
                var (success, message) = await _orderService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateProductError;
                    return RedirectToAction("Create", "AdminProduct");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateProductSuccess;
                return RedirectToAction("Index", "AdminProduct");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateProductError;
                return RedirectToAction("Create", "AdminProduct");
            }
        }
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (success, message) = await _orderService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteTagError;
                    return RedirectToAction("Index", "AdminTag");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteTagSuccess;
                return RedirectToAction("Index", "AdminTag");
            }
            catch (Exception ex)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteTagError;
                return RedirectToAction("Index", "AdminTag");
            }
        }
    }
}
