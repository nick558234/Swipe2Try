using System.Security.Claims;

namespace Swipe2Try.Helpers
{
    public static class ClaimsHelper
    {
        public static string? GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string? GetName(this ClaimsPrincipal user)
        {
            return user.Identity?.Name;
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }

        public static bool IsOwner(this ClaimsPrincipal user)
        {
            return user.IsInRole("OWNER");
        }
    }
}
