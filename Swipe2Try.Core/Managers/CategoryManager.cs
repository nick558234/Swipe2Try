using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Swipe2Try.Core.Managers
{    public class CategoryManager
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryValidator _categoryValidator;

        public CategoryManager(ICategoryRepository categoryRepository, ICategoryValidator categoryValidator)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _categoryValidator = categoryValidator ?? throw new ArgumentNullException(nameof(categoryValidator));
        }        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync() as List<Category> ?? new List<Category>();
        }

        public async Task<Category?> GetCategoryByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Category ID cannot be null or empty", nameof(id));

            return await _categoryRepository.GetCategoryByIdAsync(id);
        }        public async Task<(bool Success, List<string> Errors)> AddCategoryAsync(Category category)
        {
            try
            {
                // Validate category
                var validationResult = await _categoryValidator.ValidateForCreationAsync(category);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                // Generate a short random string for CategoryID (length 10)
                category.Id = Guid.NewGuid().ToString("N").Substring(0, 10);
                
                await _categoryRepository.AddCategoryAsync(category);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to add category: {ex.Message}" });
            }
        }
        
        public async Task<(bool Success, string Message)> AddCategoryWithMessageAsync(Category category)
        {
            var result = await AddCategoryAsync(category);
            if (result.Success)
            {
                return (true, "Category created successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }        public async Task<(bool Success, List<string> Errors)> UpdateCategoryAsync(Category category)
        {
            try
            {
                // Validate category
                var validationResult = await _categoryValidator.ValidateForUpdateAsync(category);
                if (!validationResult.IsValid)
                    return (false, validationResult.Errors);

                await _categoryRepository.UpdateCategoryAsync(category);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to update category: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> UpdateCategoryWithMessageAsync(Category category)
        {
            var result = await UpdateCategoryAsync(category);
            if (result.Success)
            {
                return (true, "Category updated successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }
        }        public async Task<(bool Success, List<string> Errors)> DeleteCategoryAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return (false, new List<string> { "Invalid category ID" });

                await _categoryRepository.DeleteCategoryAsync(id);
                return (true, new List<string>());
            }
            catch (Exception ex)
            {
                return (false, new List<string> { $"Failed to delete category: {ex.Message}" });
            }
        }

        public async Task<(bool Success, string Message)> DeleteCategoryWithMessageAsync(string id)
        {
            var result = await DeleteCategoryAsync(id);
            if (result.Success)
            {
                return (true, "Category deleted successfully!");
            }
            else
            {
                return (false, string.Join(", ", result.Errors));
            }        }
    }
}
