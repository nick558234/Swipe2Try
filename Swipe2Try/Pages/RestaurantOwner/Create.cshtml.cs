using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages.RestaurantOwner
{
    [Authorize(Roles = "Admin, Restaurant Owner")]
    public class CreateModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}