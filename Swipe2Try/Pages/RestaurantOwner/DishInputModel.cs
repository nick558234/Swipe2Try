using System.ComponentModel.DataAnnotations;

namespace Swipe2Try.Pages.RestaurantOwner
{
    public class DishInputModel
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Dish name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters", MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description must be between 1 and 500 characters", MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Health factor must be less than 50 characters")]
        public string HealthFactor { get; set; } = string.Empty;

        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Photo { get; set; } = string.Empty;
    }
}
