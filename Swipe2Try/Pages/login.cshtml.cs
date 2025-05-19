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
{
    public class loginModel : PageModel
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IUserValidator _userValidator;

        public loginModel(UserManager userManager, RoleManager roleManager, IUserValidator userValidator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userValidator = userValidator;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public void OnGet()
        {
        }        public async Task<IActionResult> OnPostAsync()
        {
            // Skip automatic model validation as we'll use our custom validator
            ModelState.Clear();
            
            // Validate inputs using the UserValidator
            var validationResult = _userValidator.ValidateForLogin(Input.Email, Input.Password);
            if (!validationResult.IsValid)
            {
                ErrorMessages.AddRange(validationResult.Errors);
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
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);                var authProperties = new AuthenticationProperties
                {
                    // Set cookie to expire after 30 minutes
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5),
                    // Make cookie persistent across browser sessions
                    IsPersistent = true,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);                // Redirect based on role
                if (roleNameToStore == "Admin")
                    return RedirectToPage("/Admin/Index");
                else if (roleNameToStore == "OWNER") // Updated to use consistent role name
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
    }    public class LoginInputModel
    {
        // Removed validation attributes as we'll use the UserValidator instead
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}