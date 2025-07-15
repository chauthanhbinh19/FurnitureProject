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

            string[] staticExtensions = { ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".map", ".woff", ".woff2", ".ttf", ".eot" };

            if (staticExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Ignore page
            string[] bypassPaths = new[]
            {
                "/",
                "/user/sign-in",
                "/user/sign-up",
                "/user/verify-code",
                "/user/forgot-password",
                "/user/verify-code-forgot-password",
                "/user/reset-password",
                "/support/about",
                "/support/contact",
                "/support/faq",
                "/support/privacy-policy",
                "/support/terms",
                "/error/not-found",
                "/error/server-error",
                "/error/maintenance"
            };

            if (bypassPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
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
