using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages.RestaurantOwner
{
    [Authorize(Roles = "Restaurant Owner")]
    public class UpdateModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
