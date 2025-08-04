using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static FurnitureProject.Helper.AppConstants;

namespace FurnitureProject.Controllers
{
    [Route("admin/order")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly ICategoryService _categoryService;
        private readonly ITagService _tagService;
        private readonly IPromotionService _promotionService;
        private readonly IAddressService _addressService;

        public AdminOrderController(IOrderService orderService, IUserService userService, ICategoryService categoryService,
            IProductService productService, ICartService cartService, ITagService tagService, IPromotionService promotionService,
            IAddressService addressService)
        {
            _orderService = orderService;
            _userService = userService;
            _productService = productService;
            _cartService = cartService;
            _categoryService = categoryService;
            _tagService = tagService;
            _promotionService = promotionService;
            _addressService = addressService;
        }
        private void SetStatusViewBag(string? status = null)
        {
            ViewBag.StatusList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Pending, Text = AppConstants.Display.Pending },
                    new { Value = AppConstants.Status.Confirmed, Text = AppConstants.Display.Confirmed },
                    new { Value = AppConstants.Status.Processing, Text = AppConstants.Display.Processing },
                    new { Value = AppConstants.Status.Shipping, Text = AppConstants.Display.Shipping },
                    new { Value = AppConstants.Status.Completed, Text = AppConstants.Display.Completed },
                    new { Value = AppConstants.Status.Cancelled, Text = AppConstants.Display.Cancelled }
                },
                "Value", "Text", status
            );
        }
        private void SetPaymentMethodViewBag(string? paymentMethod = null)
        {
            ViewBag.PaymentMethodList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.COD, Text = AppConstants.Display.COD },
                    new { Value = AppConstants.Status.Confirmed, Text = AppConstants.Display.BankTransfer },
                    new { Value = AppConstants.Status.Processing, Text = AppConstants.Display.CreditCard }
                },
                "Value", "Text", paymentMethod
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

            if (string.IsNullOrEmpty(filter.SortColumn))
            {
                filter.SortColumn = "OrderDate"; // Default sort by OrderDate
                filter.SortDirection = "desc"; // Default sort direction
            }

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

            SetStatusViewBag();
            SetPaymentMethodViewBag();
            //await SetCategoryViewBag();
            //await SetTagViewBag();

            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(OrderDTO dto)
        {
            try
            {
                var (success, message) = await _orderService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateOrderError;
                    return RedirectToAction("Create", "AdminOrder");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateOrderSuccess;
                return RedirectToAction("Index", "AdminOrder");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateOrderError;
                return RedirectToAction("Create", "AdminOrder");
            }
        }
        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            var users = await _userService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            var order = await _orderService.GetByIdAsync(id);

            ViewBag.Users = new SelectList(users, "Id", "FullName");
            ViewBag.Products = products;

            //await SetCategoryViewBag();
            //await SetTagViewBag();
            var address = await _addressService.GetAddressByIdAsync(order.AddressId.Value);

            var orderDTO = new OrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                ReceiverName = order.ReceiverName,
                ReceiverEmail = order.ReceiverEmail,
                ReceiverPhone = order.ReceiverPhone,
                AddressId = order.AddressId,
                Address = new AddressDTO
                {
                    Street = address?.Street,
                    Ward = address?.Ward,
                    District = address?.District,
                    City = address?.City,
                    Country = address?.Country,
                    PostalCode = address?.PostalCode
                },
                ShippingMethodId = order.ShippingMethodId,
                PaymentMethod = order.PaymentMethod,
                ShippingFee = order.ShippingFee,
                OrderDate = order.OrderDate,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                Products = order.OrderItems.Select(item => new ProductDTO
                {
                    Id = item.ProductId,
                    ImageUrls = item.Product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                    Name = item.Product.Name,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };

            SetStatusViewBag(orderDTO.Status);
            SetPaymentMethodViewBag(orderDTO.PaymentMethod);

            return View(orderDTO);
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update(OrderDTO dto)
        {
            try
            {
                var (success, message) = await _orderService.UpdateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateOrderError;
                    return RedirectToAction("Index", "AdminOrder");
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateOrderSuccess;
                return RedirectToAction("Index", "AdminOrder");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateOrderError;
                return RedirectToAction("Update", "AdminOrder");
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
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteOrderError;
                    return RedirectToAction("Index", "AdminOrder");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteOrderSuccess;
                return RedirectToAction("Index", "AdminOrder");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteOrderError;
                return RedirectToAction("Index", "AdminOrder");
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            var order = await _orderService.GetByIdAsync(id);
            var orderDTO = new OrderDTO
            {
                ReceiverName = order.ReceiverName,
                ReceiverEmail = order.ReceiverEmail,
                ReceiverPhone = order.ReceiverPhone,
                AddressId = order.AddressId,
                Address = new AddressDTO
                {
                    Street = order.Address?.Street,
                    Ward = order.Address?.Ward,
                    District = order.Address?.District,
                    City = order.Address?.City,
                    Country = order.Address?.Country,
                    PostalCode = order.Address?.PostalCode
                },
                ShippingMethodId = order.ShippingMethodId,
                PaymentMethod = order.PaymentMethod,
                ShippingFee = order.ShippingFee,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Products = order.OrderItems.Select(item => new ProductDTO
                {
                    Id = item.ProductId,
                    ImageUrls = item.Product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>(),
                    Name = item.Product.Name,
                    Price = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            };
            return View(orderDTO);
        }
        
    }
}
