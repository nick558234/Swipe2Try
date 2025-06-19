using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;

namespace Swipe2Try.Pages
{
    [Authorize]
    public class swipeModel : PageModel
    {
        private readonly ILogger<swipeModel> _logger;
        private readonly DishManager _dishManager;

        public List<Dish> Dishes { get; set; } = new List<Dish>();

        public swipeModel(ILogger<swipeModel> logger, DishManager dishManager)
        {
            _logger = logger;
            _dishManager = dishManager;
        }

        public async Task OnGetAsync()
        {
            try
            {
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