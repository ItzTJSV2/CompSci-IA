using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace MainProject.Pages.Trips
{
    public class CreateTripModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? DestinationSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OriginSubmit { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? StopLatSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? StopLongSubmit { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? PeopleIDSubmit { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TripNameSubmit { get; set; }
        public IActionResult OnGet()
        {
            if (User.Identity.Name != null)
            {
                ViewData["Error"] = "";
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
        public async Task<IActionResult> OnPost()
        {
            string Destination = DestinationSubmit;
            string Origin = OriginSubmit;
            string TripName = TripNameSubmit;
            string PeopleIDs;
            List<string> PeopleIDArray = new List<string>();
            List<string> StopLat = new List<string>();
            List<string> StopLong = new List<string>();

            TripDBInteract tripDBInteract = new TripDBInteract();
            CheckDB database = new CheckDB();
            List<string> AccountDetails = database.getAccount(User.Identity.Name);
            string UserID = AccountDetails[0];

            if (TripName == "")
            {
                TripName = "Default Trip Name";
            }
            if (PeopleIDSubmit == "")
            {
                PeopleIDs = UserID;
            } else
            {
                PeopleIDs = $"{PeopleIDSubmit}, {UserID}";
            }
            for (int x = 0; x < StopLongSubmit.Count(c => c == ','); x++)
            {

            }
            return RedirectToPage("MyTrips");
        }

    }
}
