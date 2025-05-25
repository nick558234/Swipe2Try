namespace Swipe2Try.Core.Models
{
    public class Category
    {
        public string Id { get; set; } = string.Empty;
        public required string Name { get; set; }
        public string? Photo { get; set; }
    }
}
