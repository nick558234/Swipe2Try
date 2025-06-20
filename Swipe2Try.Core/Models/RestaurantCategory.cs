using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

/// <summary>
/// Junction table for many-to-many relationship between Restaurant and Category
/// </summary>
public class RestaurantCategory
{
    public int RestaurantId { get; set; }
    public int CategoryId { get; set; }

    // Navigation properties
    public Restaurant? Restaurant { get; set; }
    public Category? Category { get; set; }
}
