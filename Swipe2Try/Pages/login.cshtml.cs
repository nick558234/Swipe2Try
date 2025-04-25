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

        public loginModel(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

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
            
            if (result.Success)
            {
                // Store user info in session or cookie
                HttpContext.Session.SetString("UserID", result.User.UserID);
                HttpContext.Session.SetString("UserName", result.User.Name);
                HttpContext.Session.SetString("UserRole", result.User.RoleID);
                
                // Redirect to home page
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessages = result.Errors;
                return Page();
            }
        }
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
