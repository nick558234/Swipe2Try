using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Swipe2Try.Pages.Shared
{
    public class _UserInfoPartialModel : PageModel
    {
        public string CurrentUserName { get; set; }
        public string CurrentUserRole { get; set; }

        public void OnGet()
        {
            // Use built-in User claims
            CurrentUserName = User.Identity?.Name ?? "Guest";
            CurrentUserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
        }
    }
}