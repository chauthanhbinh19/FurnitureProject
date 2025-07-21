using FurnitureProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureProject.Helper
{
    public static class UserSessionHelper
    {
        public static async Task SetUserInfoAndCartAsync(Controller controller, ICartService cartService)
        {
            var httpContext = controller.HttpContext;
            var userIdStr = httpContext.Session.GetString("UserID");

            controller.ViewBag.UserId = userIdStr;
            controller.ViewBag.UserRole = httpContext.Session.GetString("UserRole");
            controller.ViewBag.UserFullName = httpContext.Session.GetString("UserFullName");
            controller.ViewBag.UserEmail = httpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out Guid userId))
            {
                var cart = await cartService.GetCartByUserIdAsync(userId);
                controller.ViewBag.CartCount = cart?.CartItems.Count ?? 0;
            }
            else
            {
                controller.ViewBag.CartCount = 0;
            }
        }
    }

}
