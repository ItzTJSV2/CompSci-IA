using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MainProject.Pages.Trips
{
    public class GalleryModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? TripID { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ExistingPhotos { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? ExistingVideos { get; set; }
        public IActionResult OnGet()
        {
            int tripID = -1;
            string inValue = Request.Query["tripID"];
            if (User.Identity.Name != null && inValue != null && inValue.Length > 0)
            {
                tripID = int.Parse(inValue);
            }
            else
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
            string TripID = Request.Query["tripID"];
            string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
            string PhotoGalleryString = Path.Combine(parentDir, @"Mainproject", @"wwwroot", @"Gallery", TripID.ToString(), "Photos");
            string VideoGalleryString = Path.Combine(parentDir, @"Mainproject", @"wwwroot", @"Gallery", TripID.ToString(), "Videos");
            string[] photos = Directory.GetFiles(PhotoGalleryString).Select(Path.GetFileName).ToArray();
            string[] videos = Directory.GetFiles(VideoGalleryString).Select(Path.GetFileName).ToArray();

            TripID = Request.Query["tripID"];
            ExistingPhotos  = string.Join("-", photos);
            ExistingVideos = string.Join("-", videos);

            return Page();
        }
        private readonly IWebHostEnvironment environment;

        public GalleryModel(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public async Task<IActionResult> OnPostAsync(IFormFile[] photos, IFormFile[] videos)
        {
            string TripID = Request.Query["tripID"];
            string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
            string PhotoGalleryString = Path.Combine(parentDir, @"Mainproject", @"wwwroot", @"Gallery", TripID.ToString(), "Photos");
            string VideoGalleryString = Path.Combine(parentDir, @"Mainproject", @"wwwroot", @"Gallery", TripID.ToString(), "Videos");
            // Handle photo uploads
            foreach (var photo in photos)
            {
                if (photo.Length > 0)
                {
                    var filePath = Path.Combine(PhotoGalleryString, photo.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(fileStream);
                    }
                }
            }

            // Handle video uploads
            foreach (var video in videos)
            {
                if (video.Length > 0)
                {
                    var filePath = Path.Combine(VideoGalleryString, video.FileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await video.CopyToAsync(fileStream);
                    }
                }
            }
            return RedirectToPage("MyTrips");
        }
    }
}
