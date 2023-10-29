using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class MyTripModel : PageModel
    {
        public IActionResult OnGet()
        {
            int tripID = -1;
            string inValue = Request.Query["tripID"];
            if (User.Identity.Name != null && inValue != null && inValue.Length > 0)
            {
                tripID = int.Parse(inValue);
            } else
            {
                return Redirect("/Trips/MyTrips");
            }
            // Checking if User really has access to this trip.
            CheckDB UserDB = new CheckDB();
            TripDBInteract tripDB = new TripDBInteract();
            if (User.Identity.Name.ToLower() != "admin")
            {
                List<string> Account = UserDB.getAccount(User.Identity.Name);
                List<int> accountTrips = tripDB.GetAccessable(User.Identity.Name);

                bool flag = false;
                for (int i = 0; i < accountTrips.Count; i++)
                {
                    if (accountTrips[i] == tripID)
                    {
                        // IF FOUND
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return RedirectToPage("/Trips/NoAccess");
                }
            }
            ViewData["tripID"] = tripID;

            return Page();
        }
    }
}
