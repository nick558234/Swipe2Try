using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
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
        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public List<SelectListItem> AvailableRoles { get; set; } = new();

        // Populate AvailableRoles on GET
        public async Task OnGetAsync()
        {
            var roleManager = HttpContext.RequestServices.GetService(typeof(IRoleManager)) as IRoleManager;
            if (roleManager != null)
            {
                var roles = await roleManager.GetAllRolesAsync();
                AvailableRoles = roles
                    .Select(r => new SelectListItem { Value = r.RoleID, Text = r.RoleName })
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate AvailableRoles if returning to the page
            await OnGetAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Create user object from input and generate a unique UserID
            var user = new User
            {
                UserID = Guid.NewGuid().ToString(),
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                RoleID = Input.Role
            };

            // Register user using IUserManager
            var userManager = HttpContext.RequestServices.GetService(typeof(IUserManager)) as IUserManager;
            if (userManager != null)
            {
                var result = await userManager.RegisterUserAsync(user);
                if (!result.Success)
                {
                    ErrorMessages.AddRange(result.Errors);
                    return Page();
                }
            }
            else
            {
                ErrorMessages.Add("User manager service is not available.");
                return Page();
            }

            // Fetch the role name for claims
            var roleManager = HttpContext.RequestServices.GetService(typeof(IRoleManager)) as IRoleManager;
            var role = roleManager != null ? await roleManager.GetRoleByIdAsync(user.RoleID) : null;
            var roleName = role?.RoleName ?? "Unknown";

            // If sign up is successful and you want to log in the user immediately:
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Redirect as appropriate
            return RedirectToPage("/Index");
        }
    }

    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
