using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swipe2Try.Core.Models;

public class Restaurant
{
    public string Id { get; set; } = string.Empty;
    public required string Name { get; set; }
    public required string Location { get; set; }
    public string UserId { get; set; } = string.Empty;
}
