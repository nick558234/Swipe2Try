using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Security.Claims;

namespace Swipe2Try.Pages
{
    [Authorize]
    public class swipeModel : PageModel
    {
        private readonly ILogger<swipeModel> _logger;
        private readonly DishManager _dishManager;
        private readonly IUserDishPreferenceRepository _preferenceRepository;

        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public swipeModel(ILogger<swipeModel> logger, DishManager dishManager, IUserDishPreferenceRepository preferenceRepository)
        {
            _logger = logger;
            _dishManager = dishManager;
            _preferenceRepository = preferenceRepository;
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Debug: Check authentication
                _logger.LogInformation($"User authenticated: {User.Identity?.IsAuthenticated}");
                if (User.Identity?.IsAuthenticated == true)
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                    _logger.LogInformation($"User ID: {userId}, User Name: {userName}");
                }

                Dishes = await _dishManager.GetAllDishesAsync();

                // If no dishes from database, add some sample dishes for demonstration
                if (!Dishes.Any())
                {
                    Dishes = GetSampleDishes();
                    _logger.LogInformation("No dishes found in database, using sample data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dishes from database, using sample data");
                Dishes = GetSampleDishes();
            }
        }        public async Task<IActionResult> OnPostSavePreferenceAsync(int dishId, bool isLiked)
        {
            try
            {
                _logger.LogInformation($"SavePreference called with dishId: {dishId}, isLiked: {isLiked}");
                
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User not authenticated");
                    return new JsonResult(new { success = false, message = "User not authenticated" });
                }

                _logger.LogInformation($"User ID: {userId}");

                // Check if preference already exists
                var existingPreference = await _preferenceRepository.GetUserDishPreferenceAsync(userId, dishId);
                
                if (existingPreference != null)
                {
                    // Update existing preference
                    existingPreference.IsLiked = isLiked;
                    existingPreference.UpdatedAt = DateTime.UtcNow;
                    await _preferenceRepository.UpdateUserDishPreferenceAsync(existingPreference);
                    _logger.LogInformation($"Updated preference for user {userId}, dish {dishId}, liked: {isLiked}");
                }
                else
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
                    _logger.LogInformation($"Created new preference for user {userId}, dish {dishId}, liked: {isLiked}");
                }

                return new JsonResult(new { success = true, message = "Preference saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving preference for dish {dishId}");
                return new JsonResult(new { success = false, message = "Error saving preference" });
            }
        }

        private List<Dish> GetSampleDishes()
        {
            return new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Grilled Salmon",
                    Description = "Fresh Atlantic salmon with herbs and lemon",
                    UserId = "sample_user",
                    HealthFactor = 5,
                    Photo = "/images/salmon.jpg",
                    Restaurant = "Healthy Eats"
                },
                new Dish
                {
                    Id = 2,
                    Name = "Caesar Salad",
                    Description = "Crisp romaine lettuce with parmesan and croutons",
                    UserId = "sample_user",
                    HealthFactor = 3,
                    Photo = "/images/caesar.jpg",
                    Restaurant = "Green Garden"
                },
                new Dish
                {
                    Id = 3,
                    Name = "Cheeseburger",
                    Description = "Juicy beef patty with cheese and fries",
                    UserId = "sample_user",
                    HealthFactor = 1,
                    Photo = "/images/burger.jpg",
                    Restaurant = "Burger Palace"
                },
                new Dish
                {
                    Id = 4,
                    Name = "Quinoa Bowl",
                    Description = "Nutritious quinoa with vegetables and avocado",
                    UserId = "sample_user",
                    HealthFactor = 5,
                    Photo = "/images/quinoa.jpg",
                    Restaurant = "Health Hub"
                },
                new Dish
                {
                    Id = 5,
                    Name = "Pepperoni Pizza",
                    Description = "Classic pizza with pepperoni and mozzarella",
                    UserId = "sample_user",
                    HealthFactor = 2,
                    Photo = "/images/pizza.jpg",
                    Restaurant = "Tony's Pizzeria"
                },
                new Dish
                {
                    Id = 6,
                    Name = "Chicken Stir Fry",
                    Description = "Tender chicken with mixed vegetables",
                    UserId = "sample_user",
                    HealthFactor = 4,
                    Photo = "/images/stirfry.jpg",
                    Restaurant = "Asian Kitchen"
                },
                new Dish
                {
                    Id = 7,
                    Name = "Chocolate Cake",
                    Description = "Rich chocolate cake with vanilla frosting", UserId = "sample_user",
                    HealthFactor = 1,
                    Photo = "/images/cake.jpg",
                    Restaurant = "Sweet Treats"
                }
            };
        }
    }
}