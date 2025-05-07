using Swipe2Try.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Swipe2Try.Core.Interfaces
{
    public interface IUserManager
    {
        Task<(bool Success, List<string> Errors)> RegisterUserAsync(User user);
        Task<(bool Success, User User, List<string> Errors)> AuthenticateUserAsync(string email, string password);
    }
} 