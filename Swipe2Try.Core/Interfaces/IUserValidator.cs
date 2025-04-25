using Swipe2Try.Core.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Swipe2Try.Core.Interfaces
{
    public interface IUserValidator
    {
        Task<(bool IsValid, List<string> Errors)> ValidateForRegistrationAsync(User user);
        (bool IsValid, List<string> Errors) ValidateForLogin(string email, string password);
    }
} 