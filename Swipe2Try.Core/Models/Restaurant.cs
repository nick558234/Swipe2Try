using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

public class Restaurant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }    public required string UserId { get; set; }

    // Navigation properties for many-to-many relationship
    public ICollection<DishRestaurant> DishRestaurants { get; set; } = new List<DishRestaurant>();
    public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}