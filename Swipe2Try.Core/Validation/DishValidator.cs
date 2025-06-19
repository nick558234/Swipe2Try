using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Validation;

public class DishValidator : IDishValidator
{
    public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Dish dish)
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
            errors.Add(
                "Dish description cannot exceed 500 characters"); // Validate Health Factor (optional but if provided, should be valid)
        if (dish.HealthFactor.HasValue && (dish.HealthFactor.Value < 1 || dish.HealthFactor.Value > 10))
            errors.Add("Health factor must be between 1 and 10");

        // Validate Photo URL (optional but if provided, should be reasonable length)
        if (!string.IsNullOrEmpty(dish.Photo) && dish.Photo.Length > 500)
            errors.Add("Photo URL cannot exceed 500 characters");

        return (errors.Count == 0, errors);
    }

    public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Dish dish)
    {
        var errors = new List<string>();

        if (dish == null)
        {
            errors.Add("Dish cannot be null");
            return (false, errors);
        } // Validate ID is provided for update

        if (dish.Id <= 0)
            errors.Add("Dish ID must be a positive integer for update");

        // Use same validation rules as creation
        var creationValidation = await ValidateForCreationAsync(dish);
        if (!creationValidation.IsValid) errors.AddRange(creationValidation.Errors);

        return (errors.Count == 0, errors);
    }
}