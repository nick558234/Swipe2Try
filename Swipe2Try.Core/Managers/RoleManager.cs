using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRepository _userRepository;

        public RoleManager(IRoleRepository roleRepository, IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleRepository.GetAllRolesAsync();
        }        public async Task<Role?> GetRoleByIdAsync(string roleId)
        {
            return await _roleRepository.GetRoleByIdAsync(roleId);
        }

        public async Task<bool> AssignRoleAsync(User user, Role role)
        {
            // Check if role exists
            if (!await _roleRepository.RoleExistsAsync(role.RoleID))
                return false;

            // Update user with new role
            user.RoleID = role.RoleID;
            
            // In a real application, we would update the user in the database here
            // For this demo, we'll assume it happens elsewhere
            
            return true;
        }

        public async Task<bool> UpdateRoleAsync(User user, Role newRole)
        {
            // Same as assigning a role in this simple implementation
            return await AssignRoleAsync(user, newRole);
        }        public Task<bool> DeleteRoleAsync(string roleId)
        {
            // In a real application, we would delete the role from the database
            // This would require checking for users with this role first and perhaps reassigning them
            // For this simple demo, we'll return false as deletion isn't implemented
            return Task.FromResult(false);
        }

        public Task<List<User>> GetUsersByRoleAsync(string roleId)
        {
            // In a real application, we would query the database for all users with this role
            // For this simple demo, we'll return an empty list
            return Task.FromResult(new List<User>());
        }
    }
}