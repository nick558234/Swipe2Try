using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System; // For Guid

namespace Swipe2Try.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly UserManager _userManager;

        public SignUpModel(UserManager userManager)
        {
            _userManager = userManager;
        }

        [BindProperty] public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public List<SelectListItem> AvailableRoles { get; set; } = new();

        // Populate AvailableRoles on GET
        public async Task OnGetAsync()
        {
            var roles = await _userManager.GetAllRolesAsync();
            AvailableRoles = roles
                .Select(r => new SelectListItem { Value = r.RoleID, Text = r.RoleName })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate AvailableRoles if returning to the page
            await OnGetAsync();

            // Use UserManager to handle all registration and login logic
            var result = await _userManager.RegisterAndLoginUserAsync(
                Input.Name,
                Input.Email,
                Input.Password,
                Input.Role);

            if (result.Success && result.Principal != null)
            {
                var authProperties = new AuthenticationProperties
                {
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
                ErrorMessages = result.Errors ?? new List<string> { "Registration failed." };
                return Page();
            }
        }
    }

    public class RegisterInputModel
    {
        // Remove validation attributes as we'll use the UserValidator instead
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}