using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class MyTripsModel : PageModel
    {
        public IActionResult OnGet()
        { 
            ViewData["Gallery"] = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), @"Mainproject", @"Pages", @"Trips", @"Gallery");
            if (User.Identity.Name != null)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
        public IActionResult OnPost(int Trip)
        {
            return null;
        }
    }
}
