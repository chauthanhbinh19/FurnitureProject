using Microsoft.AspNetCore.Http;
using System.Net.Http;
namespace FurnitureProject.Middleware
{
    public class RoleAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public RoleAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();


            if (path.Contains("."))
            {
                await _next(context);
                return;
            }

            // Ignore page
            if (path.StartsWith("/user/sign-in") || path.StartsWith("/user/sign-up") || path.StartsWith("/user/verifycode"))
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetString("UserID");
            var userRole = context.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId))
            {
                // Not sign in
                context.Response.Redirect("/user/sign-in");
                return;
            }

            // Only admin, employee can enter admin page
            if (path.StartsWith("/admin") && (!userRole.Equals("admin") && !userRole.Equals("employee")))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Bạn không có quyền truy cập.");
                return;
            }

            await _next(context);
        }
    }
}
