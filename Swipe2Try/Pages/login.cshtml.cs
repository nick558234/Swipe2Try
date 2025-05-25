using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages
{    public class loginModel : PageModel
    {
        private readonly UserManager _userManager;

        public loginModel(UserManager userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Use UserManager to handle all authentication logic
            var result = await _userManager.LoginUserAsync(Input.Email, Input.Password);

            if (result.Success && result.Principal != null)
            {
                var authProperties = new AuthenticationProperties
                {
                    // Set cookie to expire after 30 minutes
                    // Make cookie persistent across browser sessions
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    result.Principal,
                    authProperties);

                // Get role from claims and redirect accordingly
                var roleName = result.Principal.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
                var redirectPage = _userManager.GetRedirectPageForRole(roleName);
                return RedirectToPage(redirectPage);
            }
            else
            {
                // Show error messages if authentication fails
                ErrorMessages = result.Errors ?? new List<string> { "Invalid login attempt." };
                return Page();
            }
        }
    }
    public class LoginInputModel
    {
        // Removed validation attributes as we'll use the UserValidator instead
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}