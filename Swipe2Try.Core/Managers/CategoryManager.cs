using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers
{
    public class CategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly CategoryValidator _categoryValidator;

        public CategoryManager(ICategoryRepository categoryRepository, CategoryValidator categoryValidator)
        {
            _categoryRepository = categoryRepository;
            _categoryValidator = categoryValidator; // Use injected validator
        }

        public async Task<IList<Category>> GetAllCategoriesAsync() // Changed List to IList
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Category ID cannot be null or empty", nameof(id));

            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        // Modified to use string URL for photo
        public async Task AddCategoryAsync(Category category)
        {
            var validationResult = await _categoryValidator.ValidateForCreationAsync(category); // Changed to ValidateForCreationAsync and added await
            if (!validationResult.IsValid)
            {
                // Convert FluentValidation errors to a single string or handle as needed
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }

            category.Id = Guid.NewGuid().ToString("N").Substring(0, 10);
            // Photo URL is now expected to be set on the category object directly
            
            await _categoryRepository.AddCategoryAsync(category);
        }
        
        // Removed AddCategoryWithMessageAsync as UI will handle messages based on try-catch

        // Modified to use string URL for photo
        public async Task UpdateCategoryAsync(Category category)
        {
            var validationResult = await _categoryValidator.ValidateForUpdateAsync(category); // Changed to ValidateForUpdateAsync and added await
            if (!validationResult.IsValid)
            {
                throw new ValidationException(string.Join(", ", validationResult.Errors));
            }
            // Photo URL is now expected to be set on the category object directly

            await _categoryRepository.UpdateCategoryAsync(category);
        }

        // Removed UpdateCategoryWithMessageAsync

        // Modified to return Task for direct use by UI
        public async Task DeleteCategoryAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Invalid category ID", nameof(id));

            await _categoryRepository.DeleteCategoryAsync(id);
        }

        // Removed DeleteCategoryWithMessageAsync
    }

    // Custom ValidationException to carry validation messages
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}
