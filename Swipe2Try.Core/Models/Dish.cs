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
    public string? Photo { get; set; }

    public string? Restaurant { get; set; }
}