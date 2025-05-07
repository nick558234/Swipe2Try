using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Swipe2Try.Pages
{
    public class SignUpModel : PageModel
    {
        private readonly IUserManager _userManager;
        private readonly IRoleRepository _roleRepository;

        public SignUpModel(IUserManager userManager, IRoleRepository roleRepository)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();
        
        public List<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>();

        public async Task OnGetAsync()
        {
            await LoadRolesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadRolesAsync();
                return Page();
            }

            var user = new User
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                RoleID = Input.Role
            };

            var result = await _userManager.RegisterUserAsync(user);
            
            if (result.Success)
            {
                return RedirectToPage("/Login");
            }
            else
            {
                ErrorMessages = result.Errors;
                await LoadRolesAsync();
                return Page();
            }
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            AvailableRoles = roles.Select(r => new SelectListItem
            {
                Value = r.RoleID,
                Text = r.RoleName
            }).ToList();
        }
    }

    public class RegisterInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }
    }
}
