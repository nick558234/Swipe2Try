using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages.Profile
{
    [Authorize] // Requires authentication but no specific role
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // You can access the current user's information through User claims
            var userName = User.Identity?.Name;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            _logger.LogInformation($"User {userName} ({userEmail}) with role {userRole} accessed their profile");
        }
    }
}