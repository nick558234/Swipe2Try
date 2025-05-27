using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Interfaces
{
    public interface IRestaurantValidator
    {
        Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Restaurant restaurant);
        Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Restaurant restaurant);
    }
}