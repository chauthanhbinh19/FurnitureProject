using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using FurnitureProject.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FurnitureProject.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;

        public UserController(IUserService userService, IEmailSender emailSender)
        {
            _userService = userService;
            _emailSender = emailSender;
        }
        [HttpGet("sign-in")]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost("sign-in")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(User user)
        {
            ModelState.Remove("Id");
            ModelState.Remove("FullName");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Email");

            if (!ModelState.IsValid)
                return View(user);

            var existingUsername = await _userService.GetByUsernameAsync(user.Username);

            if (existingUsername == null)
            {
                ModelState.AddModelError(nameof(user.Username), AppConstants.LogMessages.UsernameIsNotExists);
                return View(user);
            }
            else
            {
                var hasher = new PasswordHasher<User>();
                var result = hasher.VerifyHashedPassword(user, existingUsername.Password, user.Password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("UserID", existingUsername.Id.ToString());
                    HttpContext.Session.SetString("UserRole", existingUsername.Role);
                    HttpContext.Session.SetString("UserFullName", existingUsername.FullName);
                    HttpContext.Session.SetString("UserEmail", existingUsername.Email);

                    switch (existingUsername.Role)
                    {
                        case AppConstants.Status.Admin:
                            return RedirectToAction("Index", "AdminHome");
                        case AppConstants.Status.User:
                            return RedirectToAction("Index", "Home");
                        default:
                            return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(user.Password), AppConstants.LogMessages.WrongPassword);
                    return View(user);
                }
            }

        }
        [HttpGet("sign-up")]

        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost("sign-up")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(User user)
        {
            ModelState.Remove("Id");
            ModelState.Remove("FullName");
            ModelState.Remove("PhoneNumber");
            if (!ModelState.IsValid)
                return View(user);

            var existingUsername = await _userService.GetByUsernameAsync(user.Username);
            var existingEmail = await _userService.GetByEmailAsync(user.Email);

            if (existingUsername != null)
                ModelState.AddModelError(nameof(user.Username), AppConstants.LogMessages.UsernameAlreadyExists);

            if (existingEmail != null)
                ModelState.AddModelError(nameof(user.Email), AppConstants.LogMessages.EmailAlreadyExists);

            if (!ModelState.IsValid)
                return View(user);

            // Verify code
            var code = new Random().Next(100000, 999999).ToString();

            // Send email
            await _emailSender.SendEmailAsync(user.Email, "Mã xác thực",
                $"<p>Mã xác nhận của bạn là: <b>{code}</b></p>");

            // Save temp data
            TempData["VerificationCode"] = code;
            TempData["PendingEmail"] = user.Email;
            TempData["PendingUsername"] = user.Username;
            TempData["PendingPassword"] = user.Password;

            return RedirectToAction("VerifyCode", "User");
        }
        [HttpGet("verify-code")]
        public IActionResult VerifyCode()
        {
            return View(new EmailVerificationViewModel
            {
                Email = TempData["PendingEmail"]?.ToString()
                //Email = "thanhbinhfa999@gmail.com"
            });
        }
        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode(EmailVerificationViewModel model)
        {
            var expectedCode = TempData["VerificationCode"]?.ToString();
            //var email = TempData["PendingEmail"]?.ToString();
            var username = TempData["PendingUsername"]?.ToString();
            var password = TempData["PendingPassword"]?.ToString();

            if (model.Code != expectedCode)
            {
                ModelState.AddModelError(nameof(model.Code), AppConstants.LogMessages.InvalidCode);
                return View(model);
            }

            //Verfify code success -> save user
           var newUser = new User
           {
               Email = model.Email,
               Username = username,
               Password = password,
           };

            try
            {
                var (success, message) = await _userService.SignUpAsync(newUser);
                if (!success)
                {
                    TempData[AppConstants.Status.Error] = message ?? AppConstants.LogMessages.SignInUserError;
                    return View(newUser);
                }

                TempData[AppConstants.Status.Success] = AppConstants.LogMessages.SignUpSuccess;
                return RedirectToAction("SignIn", "User");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(newUser);
            }
        }

        [HttpGet("forgot-password")]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var existingEmail = await _userService.GetByEmailAsync(model.Email);
            if (existingEmail == null)
                ModelState.AddModelError(nameof(model.Email), AppConstants.LogMessages.EmailDoesNotExists);

            if (!ModelState.IsValid)
                return View(model);

            var code = new Random().Next(100000, 999999).ToString();

            // Send email
            await _emailSender.SendEmailAsync(model.Email, "Mã xác thực đổi mật khẩu",
                $"<p>Mã xác nhận của bạn là: <b>{code}</b></p>");

            TempData["Message"] = "Vui lòng kiểm tra email để đặt lại mật khẩu.";
            TempData["VerificationCode"] = code;
            TempData["VerificationEmail"] = model.Email;
            return RedirectToAction("VerifyCodeForgotPassword", "User");
        }

        [HttpGet("verify-code-forgot-password")]
        public IActionResult VerifyCodeForgotPassword()
        {
            string? email = TempData["VerificationEmail"]?.ToString();
            ViewBag.Email = email;
            TempData["VerificationEmail"] = email;
            return View();
        }

        [HttpPost("verify-code-forgot-password")]
        public IActionResult VerifyCodeForgotPassword(EmailVerificationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Giả sử bạn lưu code trong TempData hoặc DB
            string? expectedCode = TempData["VerificationCode"]?.ToString();
            string? email = TempData["VerificationEmail"]?.ToString();

            if (model.Code != expectedCode || model.Email != email)
            {
                ModelState.AddModelError(string.Empty, "Mã xác thực không đúng.");
                return View(model);
            }

            TempData["VerificationEmail"] = model.Email;
            // Mã đúng => chuyển sang form đổi mật khẩu
            return RedirectToAction("ResetPassword", "User");
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            string? email = TempData["VerificationEmail"]?.ToString();
            ViewBag.Email = email;
            return View();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.GetByEmailAsync(model.Email);

            try
            {
                if(user != null)
                {
                    var hasher = new PasswordHasher<User>();
                    var result = hasher.VerifyHashedPassword(user, user.Password, model.NewPassword);
                    if (result == PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(nameof(model.NewPassword), AppConstants.LogMessages.PasswordSameAsOld);
                        if (!ModelState.IsValid)
                            return View(model);

                        return View(model);
                    }
                    else
                    {
                        user.Password = model.NewPassword;
                        user.Password = hasher.HashPassword(user, user.Password);
                        await _userService.UpdateAsync(user);

                        TempData[AppConstants.Status.Success] = AppConstants.LogMessages.ChangePasswordSuccessful;
                        return RedirectToAction("SignIn", "User");
                    }  
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.SetString("UserID", "");
            HttpContext.Session.SetString("UserRole", "");
            HttpContext.Session.SetString("UserFullName", "");
            HttpContext.Session.SetString("UserEmail", "");
            return RedirectToAction("SignIn", "User");
        }
    }

}
