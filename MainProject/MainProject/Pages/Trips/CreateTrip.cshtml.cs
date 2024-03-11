using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class CreateTripModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string DestinationSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OriginSubmit { get; set; }
        [BindProperty(SupportsGet = true)]
        public string StopLatSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StopLongSubmit { get; set; }
        [BindProperty(SupportsGet = true)]
        public string PeopleIDSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TripNameSubmit { get; set; }
        public IActionResult OnGet()
        {
            if (User.Identity.Name != null)
            {
                ViewData["Error"] = "";
                ViewData["Destination"] = "";
                ViewData["Origin"] = "";
                ViewData["StopLat"] = "";
                ViewData["StopLong"] = "";
                ViewData["PeopleIDs"] = "";
                ViewData["TripName"] = "";
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (TripNameSubmit == "Testing Value")
            {
                return RedirectToPage("MyTrips");
            }
            return Page();
             //return RedirectToPage("MyTrips");
        }

    }
}
