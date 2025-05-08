using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Swipe2Try.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var pathValue = context.Request.Path.Value;
            if (string.IsNullOrEmpty(pathValue))
            {
                await _next(context);
                return;
            }
            var path = pathValue.ToLower();

            // Specific page/area checks
            bool isAdminArea = path.StartsWith("/admin/");
            bool isRestaurantOwnerArea = path.StartsWith("/restaurantowner/");
            bool isProfileArea = path.StartsWith("/profile/");
            bool isAccountArea = path.StartsWith("/account/");
            // Check for /swipe or /swipe/...
            bool isSwipePage = (path == "/swipe" || path.StartsWith("/swipe/"));

            // Determine if the path requires authentication
            bool requiresAuth = isAdminArea || isRestaurantOwnerArea || isProfileArea || isAccountArea || isSwipePage;

            if (requiresAuth)
            {
                var userRole = context.Session.GetString("UserRole");

                // If not logged in, redirect to login
                if (string.IsNullOrEmpty(userRole))
                {
                    context.Response.Redirect("/login");
                    return;
                }

                // Role-specific access checks
                if (isAdminArea && !userRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.Redirect("/Error?code=403"); // Forbidden
                    return;
                }

                if (isRestaurantOwnerArea && !(userRole.Equals("OWNER", StringComparison.OrdinalIgnoreCase) || userRole.Equals("ADMIN", StringComparison.OrdinalIgnoreCase)))
                {
                    context.Response.Redirect("/Error?code=403"); // Forbidden
                    return;
                }
                // For isProfileArea, isAccountArea, and isSwipePage,
                // being logged in (checked above) is sufficient if not an admin/owner specific path.
            }

            await _next(context);
        }
    }
}