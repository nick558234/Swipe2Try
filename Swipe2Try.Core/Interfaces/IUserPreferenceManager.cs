using Swipe2Try.Core.Models;

namespace Swipe2Try.Core.Interfaces;

public interface IUserPreferenceManager
{
    /// <summary>
    /// Save or update a user's preference for a dish
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="dishId">The dish ID</param>
    /// <param name="isLiked">Whether the dish is liked (true) or disliked (false)</param>
    /// <returns>A result indicating success or failure with a message</returns>
    Task<(bool Success, string Message)> SavePreferenceAsync(string userId, int dishId, bool isLiked);

    /// <summary>
    /// Get all dishes that a user has liked
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>List of liked dishes with their details</returns>
    Task<List<Dish>> GetLikedDishesAsync(string userId);

    /// <summary>
    /// Get all dishes that a user has disliked
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>List of disliked dishes with their details</returns>
    Task<List<Dish>> GetDislikedDishesAsync(string userId);

    /// <summary>
    /// Get all user preferences with dish details
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>List of preferences with associated dish details</returns>
    Task<List<UserPreferenceWithDish>> GetAllPreferencesWithDishesAsync(string userId);

    /// <summary>
    /// Get user's preference for a specific dish
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="dishId">The dish ID</param>
    /// <returns>The user's preference or null if no preference exists</returns>
    Task<UserDishPreference?> GetUserPreferenceAsync(string userId, int dishId);

    /// <summary>
    /// Remove a user's preference for a dish
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="dishId">The dish ID</param>
    /// <returns>A result indicating success or failure with a message</returns>
    Task<(bool Success, string Message)> RemovePreferenceAsync(string userId, int dishId);

    /// <summary>
    /// Get user preference statistics
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>Statistics about the user's preferences</returns>
    Task<UserPreferenceStats> GetUserPreferenceStatsAsync(string userId);
}

/// <summary>
/// Data transfer object combining user preference with dish details
/// </summary>
public class UserPreferenceWithDish
{
    public UserDishPreference Preference { get; set; } = null!;
    public Dish Dish { get; set; } = null!;
}

/// <summary>
/// Statistics about a user's preferences
/// </summary>
public class UserPreferenceStats
{
    public int TotalPreferences { get; set; }
    public int LikedCount { get; set; }
    public int DislikedCount { get; set; }
    public double LikePercentage => TotalPreferences > 0 ? (double)LikedCount / TotalPreferences * 100 : 0;
    public double DislikePercentage => TotalPreferences > 0 ? (double)DislikedCount / TotalPreferences * 100 : 0;
}
