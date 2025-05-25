using Swipe2Try.Core.Models;
using System.Collections.Generic;

namespace Swipe2Try.Core.Validation
{
    public class DishValidator
    {
        public (bool IsValid, List<string> Errors) ValidateForCreation(Dish dish)
        {
            var errors = new List<string>();

            if (dish == null)
            {
                errors.Add("Dish cannot be null");
                return (false, errors);
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(dish.Name))
                errors.Add("Dish name is required");
            else if (dish.Name.Length > 100)
                errors.Add("Dish name cannot exceed 100 characters");

            // Validate Description
            if (string.IsNullOrWhiteSpace(dish.Description))
                errors.Add("Dish description is required");
            else if (dish.Description.Length > 500)
                errors.Add("Dish description cannot exceed 500 characters");

            // Validate Health Factor (optional but if provided, should be valid)
            if (!string.IsNullOrEmpty(dish.HealthFactor) && dish.HealthFactor.Length > 50)
                errors.Add("Health factor cannot exceed 50 characters");

            // Validate Photo URL (optional but if provided, should be reasonable length)
            if (!string.IsNullOrEmpty(dish.Photo) && dish.Photo.Length > 500)
                errors.Add("Photo URL cannot exceed 500 characters");

            return (errors.Count == 0, errors);
        }

        public (bool IsValid, List<string> Errors) ValidateForUpdate(Dish dish)
        {
            var errors = new List<string>();

            if (dish == null)
            {
                errors.Add("Dish cannot be null");
                return (false, errors);
            }

            // Validate ID is provided for update
            if (string.IsNullOrWhiteSpace(dish.Id))
                errors.Add("Dish ID is required for update");

            // Use same validation rules as creation
            var creationValidation = ValidateForCreation(dish);
            if (!creationValidation.IsValid)
            {
                errors.AddRange(creationValidation.Errors);
            }

            return (errors.Count == 0, errors);
        }
    }
}
