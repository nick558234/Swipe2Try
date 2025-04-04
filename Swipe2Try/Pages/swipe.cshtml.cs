using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace Swipe2Try.Pages
{
    public class swipeModel : PageModel
    {
        public List<Dish> Dishes { get; set; }

        public void OnGet()
        {
            Dishes = new List<Dish>
               {
                   new Dish
                   {
                       ImageUrl = "https://images.unsplash.com/photo-1612874742237-6526221588bd?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1771&q=80",
                       Title = "Spaghetti Carbonara",
                       Description = "Creamy pasta with bacon and eggs. A classic Italian dish that's rich and satisfying.",
                       Restaurant = "Italian Delight",
                       HealthFactor = 6,
                       Price = "€12.95",
                       Tags = new List<string> { "Italian", "Pasta", "Creamy" }
                   },
                   new Dish
                   {
                       ImageUrl = "https://images.unsplash.com/photo-1519708227418-c8fd9a32b7a2?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1770&q=80",
                       Title = "Grilled Salmon",
                       Description = "Fresh salmon with lemon and herbs. A healthy and delicious seafood option.",
                       Restaurant = "Sea Breeze",
                       HealthFactor = 9,
                       Price = "€16.50",
                       Tags = new List<string> { "Seafood", "Healthy", "Grilled" }
                   },
                   new Dish
                   {
                       ImageUrl = "https://images.unsplash.com/photo-1520072959219-c595dc870360?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1773&q=80",
                       Title = "Veggie Burger",
                       Description = "Plant-based patty with all the fixings. Perfect for vegetarians and meat-lovers alike.",
                       Restaurant = "Green Bites",
                       HealthFactor = 8,
                       Price = "€10.95",
                       Tags = new List<string> { "Vegetarian", "Burger", "Plant-based" }
                   }
               };
        }
    }

    public class Dish
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Restaurant { get; set; }
        public int HealthFactor { get; set; }
        public string Price { get; set; }
        public List<string> Tags { get; set; }
    }
}





