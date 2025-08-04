using FurnitureProject.Helper;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Controllers
{
    [Route("admin/address")]
    public class AddressController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IAddressService _addressService;
        public AddressController(IUserService userService, ICartService cartService, IAddressService addressService)
        {
            _userService = userService;
            _cartService = cartService;
            _addressService = addressService;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("address-popup")]
        public async Task<IActionResult> AddressPopup(AddressFilterDTO filter, Guid userId, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            if(userId == null || userId == Guid.Empty)
            {
                ViewBag.UserIdToCheckAddress = null;
            }
            else
            {
                ViewBag.UserIdToCheckAddress = userId;
            }

                int pageSize = 10;
            var addresses = await _addressService.GetUserAddressesAsync(userId);

            var addressDtos = addresses.Select(a => new AddressDTO
            {
                Id = a.Id,
                UserId = a.UserId,
                Street = a.Street,
                Ward = a.Ward,
                District = a.District,
                City = a.City,
                Country = a.Country,
                PostalCode = a.PostalCode,
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                addressDtos = addressDtos
                    .Where(u => (u.Street != null && u.Street.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Ward != null && u.Ward.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                            (u.District != null && u.District.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                            (u.City != null && u.City.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                            (u.Country != null && u.Country.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)) ||
                            (u.PostalCode != null && u.PostalCode.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                addressDtos = filter.SortColumn switch
                {
                    "Street" => isAscending
                        ? addressDtos.OrderBy(p => p.Street).ToList()
                        : addressDtos.OrderByDescending(p => p.Street).ToList(),

                    "Ward" => isAscending
                        ? addressDtos.OrderBy(p => p.Ward).ToList()
                        : addressDtos.OrderByDescending(p => p.Ward).ToList(),

                    "District" => isAscending
                        ? addressDtos.OrderBy(p => p.District).ToList()
                        : addressDtos.OrderByDescending(p => p.District).ToList(),

                    "City" => isAscending
                        ? addressDtos.OrderBy(p => p.City).ToList()
                        : addressDtos.OrderByDescending(p => p.City).ToList(),

                    "Country" => isAscending
                        ? addressDtos.OrderBy(p => p.Country).ToList()
                        : addressDtos.OrderByDescending(p => p.Country).ToList(),

                    "PostalCode" => isAscending
                        ? addressDtos.OrderBy(p => p.PostalCode).ToList()
                        : addressDtos.OrderByDescending(p => p.PostalCode).ToList(),

                    _ => addressDtos
                };
            }

            int totalAddresses = addressDtos.Count();
            var pagedAddresses = addressDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var addressViewModel = new AddressViewModel
            {
                Addresses = pagedAddresses,
                Filter = filter
            };

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Keyword = filter.SearchKeyWord;
            ViewBag.TotalAddresses = totalAddresses;
            return PartialView("_AddressPopup", addressViewModel);
        }
    }
}
