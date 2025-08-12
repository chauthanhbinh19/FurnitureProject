using FurnitureProject.Helper;
using FurnitureProject.Constants;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace FurnitureProject.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IAddressService _addressService;
        public AccountController(ICategoryService categoryService, ICartService cartService, 
            IOrderService orderService, IUserService userService, IAddressService addressService)
        {
            _categoryService = categoryService;
            _cartService = cartService;
            _orderService = orderService;
            _userService = userService;
            _addressService = addressService;
        }
        private void SetStatusViewBag(string? status = null)
        {
            var orderedStatuses = new[]
            {
                new { Value = AppConstants.Status.Pending, Text = AppConstants.Display.Pending },
                new { Value = AppConstants.Status.Confirmed, Text = AppConstants.Display.Confirmed },
                new { Value = AppConstants.Status.Processing, Text = AppConstants.Display.Processing },
                new { Value = AppConstants.Status.Shipping, Text = AppConstants.Display.Shipping },
                new { Value = AppConstants.Status.Completed, Text = AppConstants.Display.Completed },
                new { Value = AppConstants.Status.Cancelled, Text = AppConstants.Display.Cancelled }
            };

            if (status == AppConstants.Status.Cancelled || status == AppConstants.Status.Completed)
            {
                var selected = orderedStatuses.FirstOrDefault(s => s.Value == status);
                if (selected != null)
                {
                    ViewBag.StatusList = new SelectList(new[] { selected }, "Value", "Text", status);
                }
                else
                {
                    ViewBag.StatusList = null;
                }
                return;
            }

            if (string.IsNullOrEmpty(status))
            {
                ViewBag.StatusList = new SelectList(orderedStatuses, "Value", "Text");
                return;
            }

            int currentIndex = Array.FindIndex(orderedStatuses, s => s.Value == status);

            if (currentIndex == -1)
            {
                ViewBag.StatusList = null;
                return;
            }

            var availableStatuses = orderedStatuses.Skip(currentIndex);
            ViewBag.StatusList = new SelectList(availableStatuses, "Value", "Text", status);
        }
        [HttpGet("")]
        public async Task<IActionResult> IndexAsync()
        {
            //await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            //LayoutHelper.SetViewBagForLayout(this, true, "user");

            //var categories = await _categoryService.GetAllAsync();
            //ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();
            return RedirectToAction("Profile");
        }
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var user = await _userService.GetByIdAsync(Guid.Parse(userId));

            var model = new AccountViewModel
            {
                CurrentSection = "Profile",
                Profile = new UserDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                }
            };

            return View("Index", model);
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(OrderFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            int pageSize = 10;

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var orders = await _orderService.GetAllByUserIdAsync(Guid.Parse(userId));

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
                IsPaid = order.IsPaid,
                TotalAmount = order.TotalAmount,
                TotalItems = order.OrderItems.Sum(item => item.Quantity),
                CreatedAt = order.CreatedAt,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                orderDTOs = orderDTOs
                    .Where(u => u.ReceiverName.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                                u.ReceiverEmail.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                                u.ReceiverPhone.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                orderDTOs = orderDTOs
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Contains(p.Status))
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

                    "IsPaid" => isAscending
                        ? orderDTOs.OrderBy(p => p.IsPaid).ToList()
                        : orderDTOs.OrderByDescending(p => p.IsPaid).ToList(),

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

            var model = new AccountViewModel
            {
                CurrentSection = "Orders",
                OrderViewModel = new OrderViewModel
                {
                    Orders = pagedOrders,
                    Filter = filter
                }
            };

            SetStatusViewBag(filter.FilterByStatus);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalOrders = totalOrders;
            return View("Index", model);
        }

        [HttpGet("order-detail")]
        public async Task<IActionResult> OrderDetails(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

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

            var model = new AccountViewModel
            {
                CurrentSection = "OrderDetails",
                OrderViewModel = new OrderViewModel
                {
                    Order = orderDTO,
                }
            };
            return View("Index", model);
        }

        [HttpGet("vouchers")]
        public async Task<IActionResult> Vouchers()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var model = new AccountViewModel
            {
                CurrentSection = "Vouchers",
                //Vouchers = _voucherService.GetByUserId(Guid.Parse(userId))
            };

            return View("Index", model);
        }
        
        [HttpGet("addresses")]
        public async Task<IActionResult> Addresses()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var addresses = await _addressService.GetUserAddressesAsync(Guid.Parse(userId));
            var model = new AccountViewModel
            {
                CurrentSection = "Addresses",
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
                }).ToList(),
            };
            return View("Index", model);
        }
        
        [HttpGet("create-address")]
        public async Task<IActionResult> CreateAddress()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            var provinces = await _addressService.GetProvincesAsync();

            var model = new AccountViewModel
            {
                CurrentSection = "CreateAddress",
                Address = new AddressDTO
                {
                    Provinces = provinces.ToList()
                },
            };
            return View("Index", model);
        }
        
        [HttpPost("create-address")]
        public async Task<IActionResult> CreateAddress(AddressDTO addressDTO)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "user");

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.OrderBy(c => c.Name).ToList();

            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null) return RedirectToAction("Login", "Account");

            try
            {
                var address = new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = Guid.Parse(userId),
                    Street = addressDTO.Street,
                    Ward = addressDTO.Ward,
                    District = addressDTO.District,
                    City = addressDTO.City,
                    Country = addressDTO.Country,
                    PostalCode = addressDTO.PostalCode,
                    IsDefault = addressDTO.IsDefault
                };
                await _addressService.AddAddressAsync(address);
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateAddressSuccess;
                return RedirectToAction("CreateAddress", "Account");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateAddressError;
                return RedirectToAction("CreateAddress", "Account");
            }
        }
        [HttpGet("ajax/district")]
        public async Task<JsonResult> GetDistricts(int provinceCode)
        {
            var districts = await _addressService.GetDistrictsAsync(provinceCode);
            return Json(districts);
        }
        [HttpGet("ajax/ward")]
        public async Task<JsonResult> GetWards(int districtCode)
        {
            var wards = await _addressService.GetWardsAsync(districtCode);
            return Json(wards);
        }
    }
}
