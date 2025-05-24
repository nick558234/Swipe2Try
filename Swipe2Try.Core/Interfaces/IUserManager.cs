using Swipe2Try.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;

namespace Swipe2Try.Core.Interfaces
{
    public interface IUserManager
    {
        Task<(bool Success, List<string> Errors)> RegisterUserAsync(User user);
        Task<(bool Success, User User, List<string> Errors)> AuthenticateUserAsync(string email, string password);
        Task<(bool Success, List<string> Errors, ClaimsPrincipal Principal)> LoginUserAsync(string email, string password);
        Task<(bool Success, List<string> Errors, ClaimsPrincipal Principal)> RegisterAndLoginUserAsync(string name, string email, string password, string roleId);
        Task<List<User>> GetAllUsersAsync();
        Task<List<Role>> GetAllRolesAsync();
    }
}