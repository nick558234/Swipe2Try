using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces
{
    public interface IDishValidator
    {
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Dish dish);
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Dish dish);
    }
}
