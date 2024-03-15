using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace MainProject.Pages.Trips
{
    public class MyTripModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? OldMemberString { get; set; }
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

            string OriginalStopString = TripInformation[3];
            if (OriginalStopString.Contains("|"))
            {
                List<string> Latitudes = new List<string>();
                List<string> Longitudes = new List<string>();
                foreach (string Stop in OriginalStopString.Split("|"))
                {
                    string[] tempStopArray = Stop.Split("/");
                    Latitudes.Add(tempStopArray[0]);
                    Longitudes.Add(tempStopArray[1]);
                }
                StopLatSubmit = String.Join("/", Latitudes);
                StopLongSubmit = String.Join("/", Longitudes);
            } else if (OriginalStopString != "")
            {
                string[] tempStopArray = OriginalStopString.Split("/");
                StopLatSubmit = tempStopArray[0];
                StopLongSubmit = tempStopArray[1];
            } else
            {
                StopLatSubmit = "";
                StopLongSubmit = "";
            }

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
            else if (OriginalMemberString != "") // Get that one user ID
            {
                string curName = checkDB.GetName(int.Parse(OriginalMemberString));
                if (curName != User.Identity.Name)
                {
                    MemberString = curName;
                }
            } else
            {
                MemberString = "";
            }
            PeopleIDSubmit = MemberString;
            OldMemberString = TripInformation[6];

            return Page();
        }

        public IActionResult OnPost()
        {
            string Destination = DestinationSubmit;
            string Origin = OriginSubmit;
            string TripName = TripNameSubmit;
            string PeopleIDs = "";
            string StopString = "";

            TripDBInteract tripDBInteract = new TripDBInteract();
            StringCreation stringCreator = new StringCreation();
            CheckDB database = new CheckDB();
            string UserID = "null";
            if (User.Identity.Name != "Admin")
            {
                UserID = database.getAccount(User.Identity.Name)[0];
            }


            if (TripName == "" || TripName == null)
            {
                TripName = "Default Trip Name";
            }
            List<string> PeopleNeedToBeAmmended = new List<string>();
            if (PeopleIDSubmit != null && PeopleIDSubmit != "")
            {
                if (PeopleIDSubmit.Count(c => c == ',') >= 1)
                {
                    string ExistingPeople = PeopleIDSubmit;
                    string[] EachPerson = ExistingPeople.Split(new string[] { ", " }, StringSplitOptions.None);
                    List<string> TempList = new List<string>();
                    for (int x = 0; x < EachPerson.Length; x++)
                    {
                        if (database.CheckExist(EachPerson[x]) != 0 && EachPerson[x] != User.Identity.Name && !TempList.Contains(database.getAccount(EachPerson[x])[0]))
                        {
                            string ID = (database.getAccount(EachPerson[x]))[0];
                            PeopleNeedToBeAmmended.Add(EachPerson[x]);
                            TempList.Add(ID);
                        }
                    }
                    if (TempList.Count > 0)
                    {
                        if (UserID == "null")
                        {
                            PeopleIDs = $"{stringCreator.ExtendableString(TempList.ToArray())}";
                        }
                        else
                        {
                            PeopleIDs = $"{stringCreator.ExtendableString(TempList.ToArray())}-{UserID}";
                        }
                    }
                    else
                    {
                        PeopleIDs = UserID;
                    }
                }
                else if (PeopleIDSubmit != User.Identity.Name && UserID != "null")
                {
                    PeopleIDs = $"{database.getAccount(PeopleIDSubmit)[0]}-{UserID}";
                    PeopleNeedToBeAmmended.Add(User.Identity.Name);
                    PeopleNeedToBeAmmended.Add(PeopleIDSubmit);
                }
                else if (UserID == "null")
                {
                    PeopleIDs = $"{database.getAccount(PeopleIDSubmit)[0]}";
                    PeopleNeedToBeAmmended.Add(PeopleIDSubmit);
                }
                else
                {
                    PeopleIDs = UserID;
                    PeopleNeedToBeAmmended.Add(User.Identity.Name);
                }
            }
            else
            {
                if (!(UserID == "null"))
                {
                    PeopleIDs = UserID;
                    PeopleNeedToBeAmmended.Add(User.Identity.Name);
                }
            }
            if (StopLatSubmit != null && StopLatSubmit != "")
            {
                if (StopLatSubmit.Count(c => c == ',') > 0)
                {
                    string[] EachLat = StopLatSubmit.Split(new string[] { ", " }, StringSplitOptions.None);
                    string[] EachLong = StopLongSubmit.Split(new string[] { ", " }, StringSplitOptions.None);
                    StopString = stringCreator.TripStops(EachLat, EachLong);
                }
                else
                {
                    StopString = $"{StopLatSubmit}/{StopLongSubmit}";
                }
            }
            else
            {
                StopString = "";
            }
            string DateString = DateTimeSubmit;
            // Find the new TripID and append that ID to added users
            foreach (string Person in PeopleNeedToBeAmmended)
            {
                string CurrentString = database.getUserTripString(Person);
                if (CurrentString != null && CurrentString != "")
                {
                    if (!CurrentString.Contains(TripIDSubmit))
                    {
                        database.changeUserTrips(Person, ($"{CurrentString}-{TripIDSubmit}"));
                    }
                }
                else
                {
                    if (!CurrentString.Contains(TripIDSubmit))
                    {
                        database.changeUserTrips(Person, ($"{TripIDSubmit}"));
                    }
                }
            }
            // Compare the new and old string to find those who need to have the trip removed.
            string[] OldIndividualMembers = OldMemberString.Split("-");
            List<string> MembersToRemove = new List<string>();
            for (int OldLoop = 0; OldLoop < OldIndividualMembers.Length; OldLoop++)
            {
                if (!PeopleIDs.Contains(OldIndividualMembers[OldLoop]))
                {
                    MembersToRemove.Add(OldIndividualMembers[OldLoop]);
                }
            }
            foreach (string Person in MembersToRemove)
            {
                string PersonUserName = database.GetName(int.Parse(Person));
                string CurrentString = database.getUserTripString(PersonUserName);
                if (CurrentString.Contains("-"))
                {
                    List<string> partsList = new List<string>();
                    string[] parts = CurrentString.Split("-");
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i] != TripIDSubmit)
                        {
                            partsList.Add(parts[i]);
                        }
                    }
                    string newTripString = string.Join("-", partsList.ToArray());

                    database.changeUserTrips(PersonUserName, (newTripString));
                }
                else
                {
                    // There's only that trip left in their account
                    database.changeUserTrips(PersonUserName, (""));
                }
            }


            tripDBInteract.EditTrip(TripIDSubmit, TripName, Origin, StopString, Destination, DateString, PeopleIDs);
            return RedirectToPage("MyTrips");
        }

    }
}
