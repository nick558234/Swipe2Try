using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Swipe2Try.Pages
{
    public class loginModel : PageModel
    {
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager; // Add this

        public loginModel(IUserManager userManager, IRoleManager roleManager) // Modify constructor
        {
            _userManager = userManager;
            _roleManager = roleManager; // Add this
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public List<string> ErrorMessages { get; set; } = new List<string>();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _userManager.AuthenticateUserAsync(Input.Email, Input.Password);
            
            if (result.Success && result.User != null)
            {
                // Fetch the role name
                var role = await _roleManager.GetRoleByIdAsync(result.User.RoleID);
                var roleNameToStore = role?.RoleName ?? "Unknown";

                // Store user info in session
                HttpContext.Session.SetString("UserID", result.User.UserID);
                HttpContext.Session.SetString("UserName", result.User.Name);
                HttpContext.Session.SetString("UserRole", roleNameToStore); // Store RoleName
                
                // Redirect based on role name
                if (roleNameToStore == "ADMIN")
                    return RedirectToPage("/Admin/Index");
                else if (roleNameToStore == "OWNER")
                    return RedirectToPage("/RestaurantOwner/Index");
                else
                    return RedirectToPage("/swipe"); // Default page for regular users
            }
            else
            {
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