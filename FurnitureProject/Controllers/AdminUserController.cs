using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.DTO;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;
using FurnitureProject.Constants;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FurnitureProject.Controllers
{
    [Route("admin/user")]
    public class AdminUserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IAddressService _addressService;

        public AdminUserController(IUserService userService, ICartService cartService, IAddressService addressService)
        {
            _userService = userService;
            _cartService = cartService;
            _addressService = addressService;
        }
        private void SetGenderViewBag(string? gender = null)
        {
            ViewBag.GenderList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Male, Text = AppConstants.Display.Male },
                    new { Value = AppConstants.Status.Female, Text = AppConstants.Display.Female },
                    new { Value = AppConstants.Status.Other, Text = AppConstants.Display.Other }
                },
                "Value", "Text", gender
            );
        }
        private void SetStatusViewBag(string? status = null)
        {
            ViewBag.StatusList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Active, Text = AppConstants.Display.Active },
                    new { Value = AppConstants.Status.Inactive, Text = AppConstants.Display.Inactive }
                },
                "Value", "Text", status
            );
        }

        private void SetRoleViewBag(string? status = null)
        {
            ViewBag.RoleList = new SelectList(
                new[] {
                    new { Value = AppConstants.Status.Admin.ToLower(), Text = AppConstants.Display.Admin },
                    new { Value = AppConstants.Status.User.ToLower(), Text = AppConstants.Display.User }
                },
                "Value", "Text", status
            );
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(UserFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var users = await _userService.GetAllAsync();

            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Password = u.Password,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                userDtos = userDtos
                    .Where(u => u.FullName.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                            u.Email.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                userDtos = userDtos
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Filter by role
            if (filter.FilterByRole != null && filter.FilterByRole.Any())
            {
                userDtos = userDtos
                   .Where(p => !string.IsNullOrEmpty(p.Role) && filter.FilterByRole.Equals(p.Role))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                userDtos = filter.SortColumn switch
                {
                    "FullName" => isAscending
                        ? userDtos.OrderBy(p => p.FullName).ToList()
                        : userDtos.OrderByDescending(p => p.FullName).ToList(),

                    "Email" => isAscending
                        ? userDtos.OrderBy(p => p.Email).ToList()
                        : userDtos.OrderByDescending(p => p.Email).ToList(),

                    "PhoneNumber" => isAscending
                        ? userDtos.OrderBy(p => p.PhoneNumber).ToList()
                        : userDtos.OrderByDescending(p => p.PhoneNumber).ToList(),

                    "Username" => isAscending
                        ? userDtos.OrderBy(p => p.Username).ToList()
                        : userDtos.OrderByDescending(p => p.Username).ToList(),

                    "Role" => isAscending
                        ? userDtos.OrderBy(p => p.Role).ToList()
                        : userDtos.OrderByDescending(p => p.Role).ToList(),

                    "CreatedAt" => isAscending
                        ? userDtos.OrderBy(p => p.CreatedAt).ToList()
                        : userDtos.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? userDtos.OrderBy(p => p.Status).ToList()
                        : userDtos.OrderByDescending(p => p.Status).ToList(),

                    _ => userDtos
                };
            }

            int totalUsers = userDtos.Count();
            var pagedUsers = userDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userViewModel = new UserViewModel
            {
                Users = pagedUsers,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);
            SetRoleViewBag(filter.FilterByRole);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = filter.SearchKeyWord;
            ViewBag.TotalUsers = totalUsers;
            return View(userViewModel);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            SetRoleViewBag();
            SetStatusViewBag();
            SetGenderViewBag();

            var provinces = await _addressService.GetProvincesAsync();
            ViewBag.Provinces = provinces;

            return View();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(UserDTO dto)
        {
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                SetRoleViewBag();
                SetStatusViewBag();
                SetGenderViewBag();
                return View(dto);
            }

            try
            {
                var (success, message) = await _userService.CreateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateUserError;
                    return RedirectToAction("Create", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.CreateUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.CreateUserError;
                return RedirectToAction("Create", "AdminUser");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Update(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            TempData["UserPassword"] = user.Password;
            SetRoleViewBag(user.Role);
            SetStatusViewBag(user.Status);
            SetGenderViewBag(user.Gender);

            var provinces = await _addressService.GetProvincesAsync();
            ViewBag.Provinces = provinces;

            var userDTO = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password, // Ensure password is hashed in the service
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Addresses = user.Addresses.Select(a => new AddressDTO
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

            ViewBag.AddressesCount = userDTO.Addresses.Count;
            return View(userDTO);
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(UserDTO dto)
        {
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
                var tempUser = await _userService.GetByIdAsync(dto.Id);
                if (tempUser == null) return NotFound();

                TempData["UserPassword"] = tempUser.Password;
                SetRoleViewBag(tempUser.Role);
                SetStatusViewBag(tempUser.Status);
                SetGenderViewBag(tempUser.Gender);
                return View(dto);
            }

            try
            {
                if (dto.Password == null)
                {
                    dto.Password = TempData["UserPassword"]?.ToString();
                }

                var (success, message) = await _userService.UpdateAsync(dto);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateUserError;
                    return RedirectToAction("Index", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.UpdateUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception)
            {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.UpdateUserError;
                return RedirectToAction("Update", "AdminUser");
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var (success, message) = await _userService.DeleteAsync(id);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteUserError;
                    return RedirectToAction("Index", "AdminUser");
                }
                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.DeleteUserSuccess;
                return RedirectToAction("Index", "AdminUser");
            }
            catch (Exception) {
                TempData[AppConstants.Status.Error] = AppConstants.LogMessages.DeleteUserError;
                return RedirectToAction("Index", "AdminUser");
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(Guid id)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            TempData["UserPassword"] = user.Password;
            SetRoleViewBag(user.Role);
            SetStatusViewBag(user.Status);
            SetGenderViewBag(user.Gender);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Password = user.Password, // Ensure password is hashed in the service
                Role = user.Role,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                Addresses = user.Addresses.Select(a => new AddressDTO
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

            ViewBag.AddressesCount = userDTO.Addresses.Count;
            return View(userDTO);
        }
        [HttpGet("user-popup")]
        public async Task<IActionResult> UserPopup(UserFilterDTO filter, int page = 1)
        {
            await UserSessionHelper.SetUserInfoAndCartAsync(this, _cartService);
            LayoutHelper.SetViewBagForLayout(this, true, "admin");

            int pageSize = 10;
            var users = await _userService.GetAllAsync();

            var userDtos = users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Password = u.Password,
                Role = u.Role,
                Status = u.Status,
                CreatedAt = u.CreatedAt
            }).ToList();

            // Search by key word
            if (!string.IsNullOrEmpty(filter.SearchKeyWord))
            {
                userDtos = userDtos
                    .Where(u => u.FullName.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase) ||
                            u.Email.Contains(filter.SearchKeyWord, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Filter by status
            if (filter.FilterByStatus != null && filter.FilterByStatus.Any())
            {
                userDtos = userDtos
                   .Where(p => !string.IsNullOrEmpty(p.Status) && filter.FilterByStatus.Equals(p.Status))
                   .ToList();
            }

            // Filter by role
            if (filter.FilterByRole != null && filter.FilterByRole.Any())
            {
                userDtos = userDtos
                   .Where(p => !string.IsNullOrEmpty(p.Role) && filter.FilterByRole.Equals(p.Role))
                   .ToList();
            }

            // Sort Order
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                bool isAscending = filter.SortDirection?.ToLower() == "asc";

                userDtos = filter.SortColumn switch
                {
                    "FullName" => isAscending
                        ? userDtos.OrderBy(p => p.FullName).ToList()
                        : userDtos.OrderByDescending(p => p.FullName).ToList(),

                    "Email" => isAscending
                        ? userDtos.OrderBy(p => p.Email).ToList()
                        : userDtos.OrderByDescending(p => p.Email).ToList(),

                    "PhoneNumber" => isAscending
                        ? userDtos.OrderBy(p => p.PhoneNumber).ToList()
                        : userDtos.OrderByDescending(p => p.PhoneNumber).ToList(),

                    "Username" => isAscending
                        ? userDtos.OrderBy(p => p.Username).ToList()
                        : userDtos.OrderByDescending(p => p.Username).ToList(),

                    "Role" => isAscending
                        ? userDtos.OrderBy(p => p.Role).ToList()
                        : userDtos.OrderByDescending(p => p.Role).ToList(),

                    "CreatedAt" => isAscending
                        ? userDtos.OrderBy(p => p.CreatedAt).ToList()
                        : userDtos.OrderByDescending(p => p.CreatedAt).ToList(),

                    "Status" => isAscending
                        ? userDtos.OrderBy(p => p.Status).ToList()
                        : userDtos.OrderByDescending(p => p.Status).ToList(),

                    _ => userDtos
                };
            }

            int totalUsers = userDtos.Count();
            var pagedUsers = userDtos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var userViewModel = new UserViewModel
            {
                Users = pagedUsers,
                Filter = filter
            };

            SetStatusViewBag(filter.FilterByStatus);
            SetRoleViewBag(filter.FilterByRole);

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.Keyword = filter.SearchKeyWord;
            ViewBag.TotalUsers = totalUsers;
            return PartialView("_UserPopup", userViewModel);
        }
    }
}
