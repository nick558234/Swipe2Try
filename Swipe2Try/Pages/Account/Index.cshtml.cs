using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Swipe2Try.Pages.Account
{
    [Authorize] // This requires the user to be authenticated but doesn't check role
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
