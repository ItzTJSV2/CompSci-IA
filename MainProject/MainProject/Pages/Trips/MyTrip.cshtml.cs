using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class MyTripModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? TripIDSubmit { get; set; }
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
        [BindProperty(SupportsGet = true)]
        public string? DateTimeSubmit { get; set; }

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
            CheckDB checkDB = new CheckDB();
            StringCreation stringCreation = new StringCreation();

            List<string> TripInformation = tripDB.GetTrip(tripID);
            TripIDSubmit = tripID.ToString();
            TripNameSubmit = TripInformation[1];
            OriginSubmit = TripInformation[2];

            ViewData["TripStopString"] = TripInformation[3];

            DestinationSubmit = TripInformation[4];
            DateTimeSubmit = TripInformation[5];

            string OriginalMemberString = TripInformation[6];
            string MemberString = "";
            if (OriginalMemberString.Length >= 2)
            {
                string[] IndividualIDs = OriginalMemberString.Split('-');
                List<string> IndividualNames = new List<string>();
                foreach (string IndividualID in IndividualIDs)
                {
                    string curName = checkDB.GetName(int.Parse(IndividualID));
                    if (curName != User.Identity.Name)
                    {
                        IndividualNames.Add(curName);
                    }
                }
                MemberString = string.Join("-", IndividualNames);
            }
            else // Get that one user ID
            {
                string curName = checkDB.GetName(int.Parse(OriginalMemberString));
                if (curName != User.Identity.Name)
                {
                    MemberString = curName;
                }
            }
            PeopleIDSubmit = MemberString;

            return Page();
        }
    }
}
