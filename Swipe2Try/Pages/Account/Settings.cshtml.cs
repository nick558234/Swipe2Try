using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages.Account
{
    [Authorize(Roles = "Admin, Restaurant Owner")] // Multiple roles can access this page
    public class SettingsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
