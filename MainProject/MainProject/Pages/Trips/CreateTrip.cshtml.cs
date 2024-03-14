using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System;

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
        [BindProperty(SupportsGet = true)]
        public string? DateTimeSubmit { get; set; }
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
                        } else
                        {
                            PeopleIDs = $"{stringCreator.ExtendableString(TempList.ToArray())}-{UserID}";
                        }
                    } else
                    {
                        PeopleIDs = UserID;
                    }
                } else if (PeopleIDSubmit != User.Identity.Name && UserID != "null")
                {
                    PeopleIDs = $"{database.getAccount(PeopleIDSubmit)[0]}-{UserID}";
                    PeopleNeedToBeAmmended.Add(User.Identity.Name);
                    PeopleNeedToBeAmmended.Add(PeopleIDSubmit);
                } else if (UserID == "null")
                {
                    PeopleIDs = $"{database.getAccount(PeopleIDSubmit)[0]}";
                    PeopleNeedToBeAmmended.Add(PeopleIDSubmit);
                } else
                {
                    PeopleIDs = UserID;
                    PeopleNeedToBeAmmended.Add(User.Identity.Name);
                }
            } else
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
                } else
                {
                    StopString = $"{StopLatSubmit}/{StopLongSubmit}";
                }
            } else
            {
                StopString = "";
            }
            string DateString = DateTimeSubmit;
            string newTripID = tripDBInteract.CreateTrip(TripName, Origin, StopString, Destination, DateString, PeopleIDs);
            // Find the new TripID and append that ID to added users
            foreach (string Person in PeopleNeedToBeAmmended)
            {
                string CurrentString = database.getUserTripString(Person);
                if (CurrentString != null && CurrentString != "")
                {
                    database.changeUserTrips(Person, ($"{CurrentString}-{newTripID}"));
                } else
                {
                    database.changeUserTrips(Person, ($"{newTripID}"));
                }
            }
            return RedirectToPage("MyTrips");
        }

    }
}
