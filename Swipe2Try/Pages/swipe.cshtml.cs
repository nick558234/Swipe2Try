using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages
{
    public class swipeModel : PageModel
    {
        private readonly ILogger<swipeModel> _logger;

        public List<DishViewModel> Dishes { get; set; } = new List<DishViewModel>();

        public swipeModel(ILogger<swipeModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // In a real application, you would fetch this from a database or service
            Dishes = GetSampleDishes();
        }

        private List<DishViewModel> GetSampleDishes()
        {
            return new List<DishViewModel>
            {
                new DishViewModel
                {
                    Id = 1,
                    ImageUrl = "/images/pasta.jpg",
                    Title = "Spaghetti Carbonara",
                    Restaurant = "Pasta Paradise",
                    Description = "Creamy pasta with pancetta and parmesan cheese.",
                    Price = "$12.99",
                    HealthFactor = 3,
                    Tags = new List<string> { "Italian", "Pasta", "Creamy" }
                },
                new DishViewModel
                {
                    Id = 2,
                    ImageUrl = "/images/sushi.jpg",
                    Title = "Dragon Roll",
                    Restaurant = "Sushi Heaven",
                    Description = "Delicious sushi roll with avocado and eel.",
                    Price = "$15.99",
                    HealthFactor = 4,
                    Tags = new List<string> { "Japanese", "Seafood", "Fresh" }
                },
                new DishViewModel
                {
                    Id = 3,
                    ImageUrl = "/images/pizza.jpg",
                    Title = "Margherita Pizza",
                    Restaurant = "Pizza Palace",
                    Description = "Classic pizza with tomato, mozzarella, and basil.",
                    Price = "$10.99",
                    HealthFactor = 2,
                    Tags = new List<string> { "Italian", "Pizza", "Vegetarian" }
                }
            };
        }
    }

    public class DishViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Restaurant { get; set; } = string.Empty;
        public int HealthFactor { get; set; }
        public string Price { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }
}





