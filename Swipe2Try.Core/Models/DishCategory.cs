using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

/// <summary>
/// Junction table for many-to-many relationship between Dish and Category
/// </summary>
public class DishCategory
{
    public int DishId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public Dish? Dish { get; set; }
    public Category? Category { get; set; }
}
