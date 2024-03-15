using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class MyTripsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? MemberCountPerTrip { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? TripIndexerString { get; set; }
        public IActionResult OnGet()
        {
            MemberCountPerTrip = "";
            TripIndexerString = "";

            TripDBInteract DB = new TripDBInteract();
            List<int> Trips = new List<int>();
            if (User.Identity.Name.ToLower() == "admin")
            {
                Trips = DB.GetAccessable("admin");
            }
            else
            {
                Trips = DB.GetAccessable(User.Identity.Name);
            }
            foreach (int i in Trips)
            {
                List<string> CardTrip = DB.GetTrip(i);
                List<string> Members = DB.GetMembersNames(int.Parse(CardTrip[0]));

            }
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
