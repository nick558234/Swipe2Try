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
    private readonly IUserDishPreferenceRepository _userDishPreferenceRepository;
    private readonly IDishRepository _dishRepository;
    private readonly ILogger<PreferencesModel> _logger;

    public List<DishPreferenceViewModel> LikedDishes { get; set; } = new();
    public List<DishPreferenceViewModel> DislikedDishes { get; set; } = new();

    public PreferencesModel(
        IUserDishPreferenceRepository userDishPreferenceRepository,
        IDishRepository dishRepository,
        ILogger<PreferencesModel> logger)
    {
        _userDishPreferenceRepository = userDishPreferenceRepository;
        _dishRepository = dishRepository;
        _logger = logger;
    }

    public async Task OnGetAsync()
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

            // Get all user preferences
            var userPreferences = await _userDishPreferenceRepository.GetUserPreferencesAsync(userId);
            _logger.LogInformation("Found {Count} preferences for user", userPreferences.Count);

            // Get all dish IDs to fetch dish details
            var dishIds = userPreferences.Select(p => p.DishId).Distinct().ToList();
            
            // Create a dictionary to store dish details for quick lookup
            var dishDetails = new Dictionary<int, Dish>();
            
            foreach (var dishId in dishIds)
            {
                var dish = await _dishRepository.GetDishByIdAsync(dishId);
                if (dish != null)
                {
                    dishDetails[dishId] = dish;
                }
            }

            // Separate liked and disliked dishes
            LikedDishes = userPreferences
                .Where(p => p.IsLiked && dishDetails.ContainsKey(p.DishId))
                .Select(p => new DishPreferenceViewModel
                {
                    Preference = p,
                    Dish = dishDetails[p.DishId]
                })
                .OrderByDescending(d => d.Preference.UpdatedAt ?? d.Preference.CreatedAt)
                .ToList();

            DislikedDishes = userPreferences
                .Where(p => !p.IsLiked && dishDetails.ContainsKey(p.DishId))
                .Select(p => new DishPreferenceViewModel
                {
                    Preference = p,
                    Dish = dishDetails[p.DishId]
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
    }

    public async Task<IActionResult> OnPostRemovePreferenceAsync(int dishId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new JsonResult(new { success = false, message = "User not authenticated" });
            }

            await _userDishPreferenceRepository.DeleteUserDishPreferenceAsync(userId, dishId);
            _logger.LogInformation("Removed preference for user {UserId} and dish {DishId}", userId, dishId);

            return new JsonResult(new { success = true, message = "Preference removed successfully" });
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
