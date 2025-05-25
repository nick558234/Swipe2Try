using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Validation
{
    public class CategoryValidator : ICategoryValidator
    {
        // If Category validation needs to check against existing data (e.g., unique name),
        // a repository can be injected here, similar to UserValidator.
        // For now, we'll assume simple validation based on model properties.
        // private readonly ICategoryRepository _categoryRepository; 

        // public CategoryValidator(ICategoryRepository categoryRepository) 
        // { 
        // _categoryRepository = categoryRepository; 
        // }

        public CategoryValidator() // Added parameterless constructor if no repository needed for now
        {
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(Category category)
        {
            var errors = new List<string>();

            if (category == null)
            {
                errors.Add("Category cannot be null");
                return (false, errors);
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                errors.Add("Category name is required.");
            }
            else if (category.Name.Length > 100)
            {
                errors.Add("Category name cannot exceed 100 characters");
            }

            // Validate Photo URL (optional but if provided, should be reasonable length)
            if (!string.IsNullOrEmpty(category.Photo) && category.Photo.Length > 500)
            {
                errors.Add("Photo URL cannot exceed 500 characters");
            }
            // Photo URL validation (optional, can be simple or regex-based)
            else if (!string.IsNullOrEmpty(category.Photo) && !Uri.IsWellFormedUriString(category.Photo, UriKind.Absolute))
            {
                errors.Add("Photo URL is not a valid URL.");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(Category category)
        {
            var errors = new List<string>();

            if (category == null)
            {
                errors.Add("Category cannot be null");
                return (false, errors);
            }

            // Validate ID is provided for update
            if (string.IsNullOrWhiteSpace(category.Id))
            {
                errors.Add("Category ID is required for an update.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                errors.Add("Category name is required.");
            }
            // Add other update-specific validation rules here
            // Example: Check for name uniqueness, excluding the current category, if repository is injected
            // if (await _categoryRepository.CategoryNameExistsAsync(category.Name, category.Id)) 
            // { 
            // errors.Add("A category with this name already exists."); 
            // }

            if (!string.IsNullOrEmpty(category.Photo) && !Uri.IsWellFormedUriString(category.Photo, UriKind.Absolute))
            {
                errors.Add("Photo URL is not a valid URL.");
            }

            return (errors.Count == 0, errors);
        }
    }
}
