using Swipe2Try.Core.Models;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> EmailExistsAsync(string email);
        Task<List<User>> GetAllUsersAsync();
    }
} 