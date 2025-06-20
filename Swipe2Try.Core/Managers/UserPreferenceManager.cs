using Microsoft.Extensions.Logging;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Managers;

public class UserPreferenceManager : IUserPreferenceManager
{
    private readonly IUserDishPreferenceRepository _preferenceRepository;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<UserPreferenceManager> _logger;

    public UserPreferenceManager(
        IUserDishPreferenceRepository preferenceRepository,
        IDishRepository dishRepository,
        ILogger<UserPreferenceManager> logger)
    {
        _preferenceRepository = preferenceRepository ?? throw new ArgumentNullException(nameof(preferenceRepository));
        _dishRepository = dishRepository ?? throw new ArgumentNullException(nameof(dishRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(bool Success, string Message)> SavePreferenceAsync(string userId, int dishId, bool isLiked)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("SavePreferenceAsync called with null or empty userId");
                return (false, "User ID is required");
            }

            if (dishId <= 0)
            {
                _logger.LogWarning("SavePreferenceAsync called with invalid dishId: {DishId}", dishId);
                return (false, "Invalid dish ID");
            }

            // Check if dish exists
            var dish = await _dishRepository.GetDishByIdAsync(dishId);
            if (dish == null)
            {
                _logger.LogWarning("SavePreferenceAsync: Dish with ID {DishId} not found", dishId);
                return (false, "Dish not found");
            }            // Check if preference already exists
            var existingPreference = await _preferenceRepository.GetUserDishPreferenceAsync(userId, dishId);
            
            if (existingPreference != null)
            {
                // Update existing preference
                existingPreference.IsLiked = isLiked;
                existingPreference.UpdatedAt = DateTime.UtcNow;
                
                await _preferenceRepository.UpdateUserDishPreferenceAsync(existingPreference);
                
                _logger.LogInformation("Updated preference for user {UserId}, dish {DishId}, liked: {IsLiked}", 
                    userId, dishId, isLiked);
                
                return (true, "Preference updated successfully");
            }            else
            {
                // Create new preference
                var newPreference = new UserDishPreference
                {
                    UserId = userId,
                    DishId = dishId,
                    IsLiked = isLiked,
                    CreatedAt = DateTime.UtcNow
                };
                await _preferenceRepository.AddUserDishPreferenceAsync(newPreference);
                
                _logger.LogInformation("Created new preference for user {UserId}, dish {DishId}, liked: {IsLiked}", 
                    userId, dishId, isLiked);
                
                return (true, "Preference saved successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving preference for user {UserId}, dish {DishId}", userId, dishId);
            return (false, "An error occurred while saving the preference");
        }
    }

    public async Task<List<Dish>> GetLikedDishesAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetLikedDishesAsync called with null or empty userId");
                return new List<Dish>();
            }

            var preferences = await _preferenceRepository.GetUserPreferencesAsync(userId);
            var likedPreferences = preferences.Where(p => p.IsLiked).ToList();

            var likedDishes = new List<Dish>();
            foreach (var preference in likedPreferences)
            {
                var dish = await _dishRepository.GetDishByIdAsync(preference.DishId);
                if (dish != null)
                {
                    likedDishes.Add(dish);
                }
            }

            _logger.LogInformation("Retrieved {Count} liked dishes for user {UserId}", likedDishes.Count, userId);
            return likedDishes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting liked dishes for user {UserId}", userId);
            return new List<Dish>();
        }
    }

    public async Task<List<Dish>> GetDislikedDishesAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetDislikedDishesAsync called with null or empty userId");
                return new List<Dish>();
            }

            var preferences = await _preferenceRepository.GetUserPreferencesAsync(userId);
            var dislikedPreferences = preferences.Where(p => !p.IsLiked).ToList();

            var dislikedDishes = new List<Dish>();
            foreach (var preference in dislikedPreferences)
            {
                var dish = await _dishRepository.GetDishByIdAsync(preference.DishId);
                if (dish != null)
                {
                    dislikedDishes.Add(dish);
                }
            }

            _logger.LogInformation("Retrieved {Count} disliked dishes for user {UserId}", dislikedDishes.Count, userId);
            return dislikedDishes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting disliked dishes for user {UserId}", userId);
            return new List<Dish>();
        }
    }

    public async Task<List<UserPreferenceWithDish>> GetAllPreferencesWithDishesAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetAllPreferencesWithDishesAsync called with null or empty userId");
                return new List<UserPreferenceWithDish>();
            }

            var preferences = await _preferenceRepository.GetUserPreferencesAsync(userId);
            var preferencesWithDishes = new List<UserPreferenceWithDish>();

            foreach (var preference in preferences)
            {
                var dish = await _dishRepository.GetDishByIdAsync(preference.DishId);
                if (dish != null)
                {
                    preferencesWithDishes.Add(new UserPreferenceWithDish
                    {
                        Preference = preference,
                        Dish = dish
                    });
                }
            }

            // Sort by most recent first
            preferencesWithDishes = preferencesWithDishes
                .OrderByDescending(p => p.Preference.UpdatedAt ?? p.Preference.CreatedAt)
                .ToList();

            _logger.LogInformation("Retrieved {Count} preferences with dishes for user {UserId}", 
                preferencesWithDishes.Count, userId);
            
            return preferencesWithDishes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preferences with dishes for user {UserId}", userId);
            return new List<UserPreferenceWithDish>();
        }
    }    public async Task<UserDishPreference?> GetUserPreferenceAsync(string userId, int dishId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetUserPreferenceAsync called with null or empty userId");
                return null;
            }

            if (dishId <= 0)
            {
                _logger.LogWarning("GetUserPreferenceAsync called with invalid dishId: {DishId}", dishId);
                return null;
            }

            var preference = await _preferenceRepository.GetUserDishPreferenceAsync(userId, dishId);
            
            _logger.LogDebug("Retrieved preference for user {UserId}, dish {DishId}: {Found}", 
                userId, dishId, preference != null);
            
            return preference;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preference for user {UserId}, dish {DishId}", userId, dishId);
            return null;
        }
    }    public async Task<(bool Success, string Message)> RemovePreferenceAsync(string userId, int dishId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("RemovePreferenceAsync called with null or empty userId");
                return (false, "User ID is required");
            }

            if (dishId <= 0)
            {
                _logger.LogWarning("RemovePreferenceAsync called with invalid dishId: {DishId}", dishId);
                return (false, "Invalid dish ID");
            }

            var existingPreference = await _preferenceRepository.GetUserDishPreferenceAsync(userId, dishId);
            if (existingPreference == null)
            {
                _logger.LogWarning("RemovePreferenceAsync: No preference found for user {UserId}, dish {DishId}", 
                    userId, dishId);
                return (false, "Preference not found");
            }

            await _preferenceRepository.DeleteUserDishPreferenceAsync(userId, dishId);
            
            _logger.LogInformation("Removed preference for user {UserId}, dish {DishId}", userId, dishId);
            return (true, "Preference removed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing preference for user {UserId}, dish {DishId}", userId, dishId);
            return (false, "An error occurred while removing the preference");
        }
    }

    public async Task<UserPreferenceStats> GetUserPreferenceStatsAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetUserPreferenceStatsAsync called with null or empty userId");
                return new UserPreferenceStats();
            }

            var preferences = await _preferenceRepository.GetUserPreferencesAsync(userId);
            
            var stats = new UserPreferenceStats
            {
                TotalPreferences = preferences.Count,
                LikedCount = preferences.Count(p => p.IsLiked),
                DislikedCount = preferences.Count(p => !p.IsLiked)
            };

            _logger.LogInformation("Retrieved preference stats for user {UserId}: {Total} total, {Liked} liked, {Disliked} disliked", 
                userId, stats.TotalPreferences, stats.LikedCount, stats.DislikedCount);

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting preference stats for user {UserId}", userId);
            return new UserPreferenceStats();
        }
    }
}
