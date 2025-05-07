using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Swipe2Try.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public IndexModel(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public List<User> Users { get; set; }
        public Dictionary<string, string> RoleNames { get; set; } = new Dictionary<string, string>();
        public string CurrentUserName { get; set; }
        public string CurrentUserRole { get; set; }

        public async Task OnGetAsync()
        {
            // Get logged in user information from session
            CurrentUserName = HttpContext.Session.GetString("UserName") ?? "Guest";
            CurrentUserRole = HttpContext.Session.GetString("UserRole") ?? "Unknown";

            // Get all users
            Users = await _userRepository.GetAllUsersAsync();

            // Get all roles for displaying role names
            var roles = await _roleRepository.GetAllRolesAsync();
            foreach (var role in roles)
            {
                RoleNames[role.RoleID] = role.RoleName;
            }
        }
    }
}