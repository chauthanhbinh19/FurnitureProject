using FurnitureProject.Helper;
using FurnitureProject.Models;
using FurnitureProject.Models.ViewModels;
using FurnitureProject.Services;
using FurnitureProject.Services.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> SignIn()
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

                    switch (existingUsername.Role)
                    {
                        case "admin":
                            return RedirectToAction("Index", "AdminHome");
                        case "user":
                            return RedirectToAction("Index", "Home");
                        default:
                            return View(user);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(user.Username), AppConstants.LogMessages.WrongPassword);
                    return View(user);
                }
            }

        }
        [HttpGet("sign-up")]

        public async Task<IActionResult> SignUp()
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
        [HttpGet("verifycode")]
        public IActionResult VerifyCode()
        {
            return View(new EmailVerificationViewModel
            {
                Email = TempData["PendingEmail"]?.ToString()
                //Email = "thanhbinhfa999@gmail.com"
            });
        }
        [HttpPost("verifycode")]
        public async Task<IActionResult> VerifyCode(EmailVerificationViewModel model)
        {
            var expectedCode = TempData["VerificationCode"]?.ToString();
            var email = TempData["PendingEmail"]?.ToString();
            var username = TempData["PendingUsername"]?.ToString();
            var password = TempData["PendingPassword"]?.ToString();

            if (model.Code != expectedCode || model.Email != email)
            {
                ModelState.AddModelError(nameof(model.Code), AppConstants.LogMessages.InvalidCode);
                return View(model);
            }

            //Verfify code success -> save user
           var newUser = new User
           {
               Email = email,
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

    }

}
