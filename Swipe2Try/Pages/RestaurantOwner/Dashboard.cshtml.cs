using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Managers;
using Swipe2Try.Core.Models;
using Swipe2Try.Core.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Swipe2Try.Pages.RestaurantOwner
{
    [Authorize(Roles = "Restaurant Owner")]
    public class DashboardModel : PageModel
    {
        private readonly DishManager _dishManager;
        private readonly RestaurantManager _restaurantManager;
        private readonly CategoryManager _categoryManager;
        private readonly IUserPreferenceManager _preferenceManager;

        public DashboardModel(
            DishManager dishManager, 
            RestaurantManager restaurantManager,
            CategoryManager categoryManager,
            IUserPreferenceManager preferenceManager)
        {
            _dishManager = dishManager;
            _restaurantManager = restaurantManager;
            _categoryManager = categoryManager;
            _preferenceManager = preferenceManager;
        }

        // KPI Properties
        public int TotalDishes { get; set; }
        public int TotalRestaurants { get; set; }
        public int TotalCategories { get; set; }
        public int TotalLikes { get; set; }
        public int TotalDislikes { get; set; }
        public double AverageHealthFactor { get; set; }
        public double LikePercentage { get; set; }
        
        // Data for charts and tables
        public List<Dish> RecentDishes { get; set; } = new();
        public List<Restaurant> UserRestaurants { get; set; } = new();
        public List<CategoryStat> CategoryStats { get; set; } = new();
        public List<DishPerformance> TopPerformingDishes { get; set; } = new();
        public List<MonthlyStats> MonthlyData { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            await LoadDashboardDataAsync(userId);
        }

        private async Task LoadDashboardDataAsync(string userId)
        {
            try
            {
                // Load user's dishes with categories
                var userDishes = await _dishManager.GetDishesByUserIdWithCategoriesAsync(userId);
                
                // Load user's restaurants with categories
                var userRestaurants = await _restaurantManager.GetRestaurantsByUserIdWithCategoriesAsync(userId);
                
                // Calculate KPIs
                TotalDishes = userDishes.Count;
                TotalRestaurants = userRestaurants.Count;
                
                // Get unique categories from dishes and restaurants
                var dishCategories = userDishes.SelectMany(d => d.Categories ?? new List<Category>()).Select(c => c.Id).Distinct();
                var restaurantCategories = userRestaurants.SelectMany(r => r.Categories ?? new List<Category>()).Select(c => c.Id).Distinct();
                TotalCategories = dishCategories.Union(restaurantCategories).Count();                // Calculate average health factor
                if (userDishes.Any())
                {
                    var dishesWithHealthFactor = userDishes.Where(d => d.HealthFactor.HasValue);
                    if (dishesWithHealthFactor.Any())
                    {
                        AverageHealthFactor = Math.Round(dishesWithHealthFactor.Average(d => (double)d.HealthFactor!.Value), 1);
                    }
                }
                
                // TODO: Get actual preference data when available
                // For now, simulate some data
                TotalLikes = userDishes.Count * 3; // Simulate 3 likes per dish on average
                TotalDislikes = userDishes.Count * 1; // Simulate 1 dislike per dish on average
                var totalInteractions = TotalLikes + TotalDislikes;
                LikePercentage = totalInteractions > 0 ? Math.Round((double)TotalLikes / totalInteractions * 100, 1) : 0;
                
                // Get recent dishes (last 5)
                RecentDishes = userDishes.OrderByDescending(d => d.Id).Take(5).ToList();
                
                // Set restaurants
                UserRestaurants = userRestaurants;
                
                // Calculate category statistics
                CalculateCategoryStats(userDishes);
                
                // Calculate dish performance
                CalculateDishPerformance(userDishes);
                
                // Generate monthly stats (simulated for now)
                GenerateMonthlyStats();
            }
            catch (Exception)
            {
                // Handle errors gracefully
                TotalDishes = 0;
                TotalRestaurants = 0;
                TotalCategories = 0;
            }
        }

        private void CalculateCategoryStats(List<Dish> dishes)
        {
            var categoryGroups = dishes
                .SelectMany(d => d.Categories ?? new List<Category>())
                .GroupBy(c => c.Name)
                .Select(g => new CategoryStat
                {
                    CategoryName = g.Key ?? "Unknown",
                    DishCount = g.Count(),
                    Percentage = Math.Round((double)g.Count() / dishes.Count * 100, 1)
                })
                .OrderByDescending(c => c.DishCount)
                .ToList();

            CategoryStats = categoryGroups;
        }

        private void CalculateDishPerformance(List<Dish> dishes)
        {
            // Simulate performance data - in real implementation, this would come from user preferences
            var random = new Random(42); // Use seed for consistent results
            
            TopPerformingDishes = dishes
                .Take(10)
                .Select(d => new DishPerformance                {
                    DishName = d.Name ?? "Unknown",
                    Likes = random.Next(1, 20),
                    Dislikes = random.Next(0, 5),
                    HealthFactor = d.HealthFactor ?? 1
                })
                .OrderByDescending(d => d.LikeRatio)
                .Take(5)
                .ToList();
        }

        private void GenerateMonthlyStats()
        {
            // Generate last 6 months of simulated data
            var random = new Random(42);
            MonthlyData = new List<MonthlyStats>();
            
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                MonthlyData.Add(new MonthlyStats
                {
                    Month = date.ToString("MMM yyyy"),
                    DishesAdded = random.Next(1, 8),
                    TotalLikes = random.Next(10, 50),
                    TotalDislikes = random.Next(2, 15)
                });
            }
        }
    }

    public class CategoryStat
    {
        public string CategoryName { get; set; } = string.Empty;
        public int DishCount { get; set; }
        public double Percentage { get; set; }
    }

    public class DishPerformance
    {
        public string DishName { get; set; } = string.Empty;
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int HealthFactor { get; set; }
        public double LikeRatio => Likes + Dislikes > 0 ? (double)Likes / (Likes + Dislikes) * 100 : 0;
    }

    public class MonthlyStats
    {
        public string Month { get; set; } = string.Empty;
        public int DishesAdded { get; set; }
        public int TotalLikes { get; set; }
        public int TotalDislikes { get; set; }
    }
}
