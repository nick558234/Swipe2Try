using Swipe2Try.Core.Models;
using System.Collections.Generic;

namespace Swipe2Try.Core.Validation
{
    public class RestaurantValidator
    {
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new List<string>();
        }

        public ValidationResult ValidateForCreation(Restaurant restaurant)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(restaurant.Name))
            {
                result.Errors.Add("Restaurant name is required");
                result.IsValid = false;
            }
            else if (restaurant.Name.Length > 100)
            {
                result.Errors.Add("Restaurant name cannot exceed 100 characters");
                result.IsValid = false;
            }

            if (string.IsNullOrWhiteSpace(restaurant.Location))
            {
                result.Errors.Add("Restaurant location is required");
                result.IsValid = false;
            }
            else if (restaurant.Location.Length > 200)
            {
                result.Errors.Add("Restaurant location cannot exceed 200 characters");
                result.IsValid = false;
            }

            if (string.IsNullOrWhiteSpace(restaurant.UserId))
            {
                result.Errors.Add("User ID is required");
                result.IsValid = false;
            }

            return result;
        }

        public ValidationResult ValidateForUpdate(Restaurant restaurant)
        {
            var result = ValidateForCreation(restaurant);

            if (string.IsNullOrWhiteSpace(restaurant.Id))
            {
                result.Errors.Add("Restaurant ID is required for update");
                result.IsValid = false;
            }

            return result;
        }
    }
}
