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
        private readonly CategoryManager _categoryManager;
        private readonly IUserPreferenceManager _preferenceManager;

        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public swipeModel(ILogger<swipeModel> logger, DishManager dishManager, CategoryManager categoryManager, IUserPreferenceManager preferenceManager)
        {
            _logger = logger;
            _dishManager = dishManager;
            _categoryManager = categoryManager;
            _preferenceManager = preferenceManager;
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

                // Get all dishes with their categories loaded
                Dishes = await _dishManager.GetAllDishesWithCategoriesAsync();                // If no dishes from database, add some sample dishes for demonstration
                if (!Dishes.Any())
                {
                    Dishes = await GetSampleDishesWithCategoriesAsync();
                    _logger.LogInformation("No dishes found in database, using sample data");
                }
                else
                {
                    _logger.LogInformation($"Loaded {Dishes.Count} dishes from database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dishes from database, using sample data");
                Dishes = await GetSampleDishesWithCategoriesAsync();
            }
        }public async Task<IActionResult> OnPostSavePreferenceAsync(int dishId, bool isLiked)
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

                // Use the preference manager to save the preference
                var result = await _preferenceManager.SavePreferenceAsync(userId, dishId, isLiked);
                
                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving preference for dish {dishId}");
                return new JsonResult(new { success = false, message = "Error saving preference" });
            }
        }        private Task<List<Dish>> GetSampleDishesWithCategoriesAsync()
        {
            // Get sample categories to assign to dishes
            var sampleCategories = new List<Category>
            {
                new Category { Id = 1, Name = "Healthy" },
                new Category { Id = 2, Name = "Italian" },
                new Category { Id = 3, Name = "Fast Food" },
                new Category { Id = 4, Name = "Vegetarian" },
                new Category { Id = 5, Name = "Dessert" },
                new Category { Id = 6, Name = "Asian" }
            };

            var dishes = new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Grilled Salmon",
                    Description = "Fresh Atlantic salmon with herbs and lemon",
                    UserId = "sample_user",
                    HealthFactor = 5,
                    Photo = "https://images.unsplash.com/photo-1485921325833-c519f76c4927?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Healthy Eats",
                    Categories = new List<Category> { sampleCategories[0] } // Healthy
                },
                new Dish
                {
                    Id = 2,
                    Name = "Caesar Salad",
                    Description = "Crisp romaine lettuce with parmesan and croutons",
                    UserId = "sample_user",
                    HealthFactor = 3,
                    Photo = "https://images.unsplash.com/photo-1546793665-c74683f339c1?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Green Garden",
                    Categories = new List<Category> { sampleCategories[0], sampleCategories[3] } // Healthy, Vegetarian
                },
                new Dish
                {
                    Id = 3,
                    Name = "Cheeseburger",
                    Description = "Juicy beef patty with cheese and fries",
                    UserId = "sample_user",
                    HealthFactor = 1,
                    Photo = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Burger Palace",
                    Categories = new List<Category> { sampleCategories[2] } // Fast Food
                },
                new Dish
                {
                    Id = 4,
                    Name = "Quinoa Bowl",
                    Description = "Nutritious quinoa with vegetables and avocado",
                    UserId = "sample_user",
                    HealthFactor = 5,
                    Photo = "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Health Hub",
                    Categories = new List<Category> { sampleCategories[0], sampleCategories[3] } // Healthy, Vegetarian
                },
                new Dish
                {
                    Id = 5,
                    Name = "Pepperoni Pizza",
                    Description = "Classic pizza with pepperoni and mozzarella",
                    UserId = "sample_user",
                    HealthFactor = 2,
                    Photo = "https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Tony's Pizzeria",
                    Categories = new List<Category> { sampleCategories[1], sampleCategories[2] } // Italian, Fast Food
                },
                new Dish
                {
                    Id = 6,
                    Name = "Chicken Stir Fry",
                    Description = "Tender chicken with mixed vegetables",
                    UserId = "sample_user",
                    HealthFactor = 4,
                    Photo = "https://images.unsplash.com/photo-1603133872878-684f208fb84b?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Asian Kitchen",
                    Categories = new List<Category> { sampleCategories[0], sampleCategories[5] } // Healthy, Asian
                },
                new Dish
                {
                    Id = 7,
                    Name = "Chocolate Cake",
                    Description = "Rich chocolate cake with vanilla frosting",
                    UserId = "sample_user",
                    HealthFactor = 1,
                    Photo = "https://images.unsplash.com/photo-1578985545062-69928b1d9587?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80",
                    Restaurant = "Sweet Treats",
                    Categories = new List<Category> { sampleCategories[4] } // Dessert
                }
            };

            return Task.FromResult(dishes);
        }
    }
}