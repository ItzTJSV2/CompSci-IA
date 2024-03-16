using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class MyTripsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? AmountOfPeopleString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? TripIndexerString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? IDListString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? NameListString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? EmailListString { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? PhoneListString { get; set; }

        public IActionResult OnGet()
        {
            if (!(User.Identity.Name != null))
            {
                return RedirectToPage("/Index");
            }
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

            string output = string.Join(", ", Trips);
            Console.WriteLine($"Trips: {output}");

            //  [Member][Trip]
            //  TypeOfData | Member(0)  Member(1)  
            //  Trip(0)
            //  Trip(1)
            //  ...

            List<int> AmountOfFields = new List<int>();
            List<int> TripIDIndexer = new List<int>();
            List<string> memberIDs = new List<string>();
            List<string> memberNames = new List<string>();
            List<string> memberEmails = new List<string>();
            List<string> memberPhones = new List<string>();

            for (int x = 0; x < Trips.Count(); x++) // For each Trip
            {
                TripIDIndexer.Add(Trips[x]);
                List<string> TripDetails = DB.GetMemberIDString(Trips[x]);
                List<string> AccountDetails = new List<string>();
                List<string> TempList1 = new List<string>(); // IDs
                List<string> TempList2 = new List<string>(); // Names
                List<string> TempList3 = new List<string>(); // Emails
                List<string> TempList4 = new List<string>(); // Phone
                string[] TripMemberIDs = (DB.GetTrip(Trips[x])[6]).Split("-");
                for (int y = 0; y < TripMemberIDs.Length; y++) // For each Member
                {
                    AccountDetails = DB.GetUserInformation(int.Parse(TripDetails[y]));
                    TempList1.Add(AccountDetails[0]);
                    TempList2.Add(AccountDetails[1]);
                    TempList3.Add(AccountDetails[2]);
                    TempList4.Add(AccountDetails[3]);
                }
                memberIDs.Add(string.Join("/", TempList1));
                memberNames.Add(string.Join("/", TempList2));
                memberEmails.Add(string.Join("/", TempList3));
                memberPhones.Add(string.Join("/", TempList4));
                AmountOfFields.Add(TripMemberIDs.Length);
            }

            foreach (int i in Trips)
            {
                List<string> CardTrip = DB.GetTrip(i);
                List<string> Members = DB.GetMembersNames(int.Parse(CardTrip[0]));

            }
            ViewData["Gallery"] = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), @"Mainproject", @"Pages", @"Trips", @"Gallery");

            // Submitting Variables
            TripIndexerString = string.Join("-", TripIDIndexer.ToArray());
            AmountOfPeopleString = string.Join("-", AmountOfFields.ToArray());
            IDListString = string.Join("|", memberIDs);
            NameListString = string.Join("|", memberNames);
            EmailListString = string.Join("|", memberEmails);
            PhoneListString = string.Join("|", memberPhones);
            // FOR EACH VARIABLE:
            // Member1/Member2/Member3|Member1/Member2/Member3
            // | = new trip


            return Page();
        }
        public IActionResult OnPost(string TripID) {
            TripDBInteract tripDBInteract = new TripDBInteract();
            CheckDB checkDB = new CheckDB();
            List<string> MemberString = tripDBInteract.GetMemberIDString(int.Parse(TripID));
            foreach (string PersonID in MemberString)
            {
                string PersonUserName = checkDB.GetName(int.Parse(PersonID));
                string CurrentString = checkDB.getUserTripString(PersonUserName);
                if (CurrentString.Contains("-"))
                {
                    List<string> partsList = new List<string>();
                    string[] parts = CurrentString.Split("-");
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i] != TripID)
                        {
                            partsList.Add(parts[i]);
                        }
                    }
                    string newTripString = string.Join("-", partsList.ToArray());

                    checkDB.changeUserTrips(PersonUserName, (newTripString));
                }
                else
                {
                    // There's only that trip left in their account
                    checkDB.changeUserTrips(PersonUserName, (""));
                }
            }
            tripDBInteract.DeleteTrip(int.Parse(TripID));
            return Page(); ;
        }
    }
}
