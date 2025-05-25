using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Validation
{
    public class RestaurantValidator : IRestaurantValidator
    {
        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Restaurant restaurant)
        {
            var errors = new List<string>();

            if (restaurant == null)
            {
                errors.Add("Restaurant cannot be null");
                return (false, errors);
            }
            
            if (string.IsNullOrWhiteSpace(restaurant.Name))
            {
                errors.Add("Restaurant name is required");
            }
            else if (restaurant.Name.Length > 100)
            {
                errors.Add("Restaurant name cannot exceed 100 characters");
            }

            if (string.IsNullOrWhiteSpace(restaurant.Location))
            {
                errors.Add("Restaurant location is required");
            }
            else if (restaurant.Location.Length > 200)
            {
                errors.Add("Restaurant location cannot exceed 200 characters");
            }

            if (string.IsNullOrWhiteSpace(restaurant.UserId))
            {
                errors.Add("User ID is required");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Restaurant restaurant)
        {
            var errors = new List<string>();

            if (restaurant == null)
            {
                errors.Add("Restaurant cannot be null");
                return (false, errors);
            }

            // First run the same validations as creation
            var creationValidation = await ValidateForCreationAsync(restaurant);
            if (!creationValidation.IsValid)
            {
                return creationValidation;
            }

            if (string.IsNullOrWhiteSpace(restaurant.Id))
            {
                errors.Add("Restaurant ID is required for update");
            }

            return (errors.Count == 0, errors);
        }
    }
}
