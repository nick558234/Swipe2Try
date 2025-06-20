using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;
using Swipe2Try.Core.Models;
using System.Security.Claims;

namespace Swipe2Try.Pages;

[Authorize]
public class PreferencesModel : PageModel
{
    private readonly IUserPreferenceManager _preferenceManager;
    private readonly ILogger<PreferencesModel> _logger;

    public List<DishPreferenceViewModel> LikedDishes { get; set; } = new();
    public List<DishPreferenceViewModel> DislikedDishes { get; set; } = new();
    public UserPreferenceStats Stats { get; set; } = new();

    public PreferencesModel(
        IUserPreferenceManager preferenceManager,
        ILogger<PreferencesModel> logger)
    {
        _preferenceManager = preferenceManager;
        _logger = logger;
    }    public async Task OnGetAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User not authenticated or user ID not found");
                return;
            }

            _logger.LogInformation("Loading preferences for user: {UserId}", userId);

            // Get user preference statistics
            Stats = await _preferenceManager.GetUserPreferenceStatsAsync(userId);

            // Get all preferences with dish details
            var preferencesWithDishes = await _preferenceManager.GetAllPreferencesWithDishesAsync(userId);
            _logger.LogInformation("Found {Count} preferences for user", preferencesWithDishes.Count);

            // Separate liked and disliked dishes
            LikedDishes = preferencesWithDishes
                .Where(p => p.Preference.IsLiked)
                .Select(p => new DishPreferenceViewModel
                {
                    Preference = p.Preference,
                    Dish = p.Dish
                })
                .OrderByDescending(d => d.Preference.UpdatedAt ?? d.Preference.CreatedAt)
                .ToList();

            DislikedDishes = preferencesWithDishes
                .Where(p => !p.Preference.IsLiked)
                .Select(p => new DishPreferenceViewModel
                {
                    Preference = p.Preference,
                    Dish = p.Dish
                })
                .OrderByDescending(d => d.Preference.UpdatedAt ?? d.Preference.CreatedAt)
                .ToList();

            _logger.LogInformation("Loaded {LikedCount} liked dishes and {DislikedCount} disliked dishes", 
                LikedDishes.Count, DislikedDishes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user preferences");
        }
    }    public async Task<IActionResult> OnPostRemovePreferenceAsync(int dishId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false, message = "User not authenticated" });
            }

            var result = await _preferenceManager.RemovePreferenceAsync(userId, dishId);
            _logger.LogInformation("Attempted to remove preference for user {UserId} and dish {DishId}: {Success}", 
                userId, dishId, result.Success);

            return new JsonResult(new { success = result.Success, message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing preference for dish {DishId}", dishId);
            return new JsonResult(new { success = false, message = "Error removing preference" });
        }
    }
}

public class DishPreferenceViewModel
{
    public UserDishPreference Preference { get; set; } = null!;
    public Dish Dish { get; set; } = null!;
}
