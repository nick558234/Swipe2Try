using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces
{
    public interface ICategoryValidator
    {
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Category category);
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Category category);
    }
}
