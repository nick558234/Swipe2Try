using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces;

public interface IRoleManager
{
    Task<List<Role>> GetAllRolesAsync();
    Task<Role?> GetRoleByIdAsync(string roleId);
    Task<bool> AssignRoleAsync(User user, Role role);
    Task<bool> UpdateRoleAsync(User user, Role newRole);
    Task<bool> DeleteRoleAsync(string roleId);
    Task<List<User>> GetUsersByRoleAsync(string roleId);
}