namespace Swipe2Try.Core.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Photo { get; set; }

    // Navigation properties for many-to-many relationships
    public ICollection<DishCategory> DishCategories { get; set; } = new List<DishCategory>();
    public ICollection<RestaurantCategory> RestaurantCategories { get; set; } = new List<RestaurantCategory>();
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    public ICollection<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
}