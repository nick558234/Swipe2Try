using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swipe2Try.Core.Interfaces;

namespace Swipe2Try.Pages.Admin;

public class LinkDishesModel : PageModel
{
    private readonly IDishRestaurantManager _dishRestaurantManager;
    private readonly IDishRepository _dishRepository;
    private readonly IRestaurantRepository _restaurantRepository;

    public LinkDishesModel(
        IDishRestaurantManager dishRestaurantManager,
        IDishRepository dishRepository,
        IRestaurantRepository restaurantRepository)
    {
        _dishRestaurantManager = dishRestaurantManager;
        _dishRepository = dishRepository;
        _restaurantRepository = restaurantRepository;
    }

    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public int? LinksCreated { get; set; }
    public int TotalDishes { get; set; }
    public int TotalRestaurants { get; set; }
    public int TotalLinks { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadStatistics();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string action)
    {
        try
        {
            if (action == "link")
            {
                var result = await _dishRestaurantManager.LinkDishesByOwnershipAsync();
                
                Message = result.Message;
                IsSuccess = result.Success;
                LinksCreated = result.LinksCreated;
            }
            else
            {
                Message = "Invalid action specified.";
                IsSuccess = false;
            }

            await LoadStatistics();
            return Page();
        }
        catch (Exception ex)
        {
            Message = $"Error processing request: {ex.Message}";
            IsSuccess = false;
            await LoadStatistics();
            return Page();
        }
    }

    private async Task LoadStatistics()
    {
        try
        {
            var dishes = await _dishRepository.GetAllDishesAsync();
            var restaurants = await _restaurantRepository.GetAllRestaurantsAsync();
            var links = await _dishRestaurantManager.GetAllDishesWithRestaurantsAsync();

            TotalDishes = dishes.Count;
            TotalRestaurants = restaurants.Count;
            TotalLinks = links.Sum(d => d.Restaurants.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading statistics: {ex.Message}");
            TotalDishes = 0;
            TotalRestaurants = 0;
            TotalLinks = 0;
        }
    }
}
