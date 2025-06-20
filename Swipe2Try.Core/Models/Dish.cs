using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

public class Dish
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string UserId { get; set; } = string.Empty;

    public int? HealthFactor { get; set; }
    public string? Photo { get; set; }    public string? Restaurant { get; set; }    // Navigation properties for many-to-many relationship
    public ICollection<DishRestaurant> DishRestaurants { get; set; } = new List<DishRestaurant>();
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    
    // Navigation properties for categories
    public ICollection<DishCategory> DishCategories { get; set; } = new List<DishCategory>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public Dish()
    {
    }

    public Dish(string name, string description, string userId = "", int? healthFactor = null, string? photo = null,
        string? restaurant = null)
    {
        Name = name;
        Description = description;
        UserId = userId;
        HealthFactor = healthFactor;
        Photo = photo;
        Restaurant = restaurant;
    }
}