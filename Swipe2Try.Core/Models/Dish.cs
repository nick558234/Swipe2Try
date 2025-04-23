using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

public class Dish
{
    public string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Restaurant { get; set; }
}