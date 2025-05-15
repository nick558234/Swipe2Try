using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Swipe2Try.Pages
{
    public class loginModel : PageModel
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;

        public loginModel(IUserManager userManager, IRoleManager roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Authenticate user using IUserManager (database-backed)
            var result = await _userManager.AuthenticateUserAsync(Input.Email, Input.Password);

            if (result.Success && result.User != null)
            {
                // Get role from database
                var role = await _roleManager.GetRoleByIdAsync(result.User.RoleID);
                var roleNameToStore = role?.RoleName ?? "Unknown";

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.User.Name),
                    new Claim(ClaimTypes.Email, result.User.Email),
                    new Claim(ClaimTypes.Role, roleNameToStore)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    // Optional: set properties like IsPersistent, ExpiresUtc, etc.
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Redirect based on role
                if (roleNameToStore.ToUpper() == "ADMIN")
                    return RedirectToPage("/Admin/Index");
                else if (roleNameToStore.ToUpper() == "Restaurant Owner")
                    return RedirectToPage("/RestaurantOwner/Index");
                else
                    return RedirectToPage("/swipe");
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
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}