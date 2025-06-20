using System;

namespace Swipe2Try.Core.Models;

public class UserDishPreference
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int DishId { get; set; }
    public bool IsLiked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public UserDishPreference()
    {
    }

    public UserDishPreference(string userId, int dishId, bool isLiked)
    {
        UserId = userId;
        DishId = dishId;
        IsLiked = isLiked;
        CreatedAt = DateTime.UtcNow;
    }
}
