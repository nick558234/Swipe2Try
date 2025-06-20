using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

/// <summary>
/// Junction table model for the many-to-many relationship between Dishes and Restaurants
/// </summary>
public class DishRestaurant
{
    public int Id { get; set; }
    public int DishId { get; set; }
    public int RestaurantId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Dish? Dish { get; set; }
    public Restaurant? Restaurant { get; set; }

    public DishRestaurant()
    {
    }

    public DishRestaurant(int dishId, int restaurantId)
    {
        DishId = dishId;
        RestaurantId = restaurantId;
        CreatedAt = DateTime.UtcNow;
    }
}
