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
{    public class SignUpModel : PageModel
    {
        private readonly IUserValidator _userValidator;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        
        public SignUpModel(IUserValidator userValidator, RoleManager roleManager, UserManager userManager)
        {
            _userValidator = userValidator;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public List<string> ErrorMessages { get; set; } = new();

        public List<SelectListItem> AvailableRoles { get; set; } = new();

        // Populate AvailableRoles on GET
        public async Task OnGetAsync()
        {
            var roles = await _roleManager.GetAllRolesAsync();
            AvailableRoles = roles
                .Select(r => new SelectListItem { Value = r.RoleID, Text = r.RoleName })
                .ToList();
        }        public async Task<IActionResult> OnPostAsync()
        {
            // Repopulate AvailableRoles if returning to the page
            await OnGetAsync();
            
            // Skip automatic model validation as we'll use our custom validator
            ModelState.Clear();
            
            // Create user object from input and generate a unique UserID
            var user = new User
            {
                UserID = Guid.NewGuid().ToString(),
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                RoleID = Input.Role
            };
            
            // Validate the user using the UserValidator
            var validationResult = await _userValidator.ValidateForRegistrationAsync(user);
            if (!validationResult.IsValid)
            {
                ErrorMessages.AddRange(validationResult.Errors);
                return Page();
            }            // Register user using the injected _userManager
            var result = await _userManager.RegisterUserAsync(user);
            if (!result.Success)
            {
                ErrorMessages.AddRange(result.Errors);
                return Page();
            }

            // Fetch the role name for claims
            var role = await _roleManager.GetRoleByIdAsync(user.RoleID);
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
    }    public class RegisterInputModel
    {
        // Remove validation attributes as we'll use the UserValidator instead
        [DataType(DataType.Text)]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
