using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MainProject.Pages.Accounts
{
    public class ChangePasswordModel : PageModel
    {
        [BindProperty]
        public Password newPass { get; set; }
        public class Password
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Old Password")]
            public string oldPassword { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string newPassword { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("newPassword", ErrorMessage = "Passwords do not match!")]
            public string ConfirmPassword { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                CheckDB database = new CheckDB();
                List<string> AccountDetails = database.getAccount(User.Identity.Name);
                int AccountID = Int16.Parse(AccountDetails[0]);
                string matchHash = AccountDetails[4];

                string oldPassword = newPass.oldPassword;
                string newPassword = newPass.newPassword;
                var crypt = new SHA256Managed();
                var oldHash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(oldPassword));
                foreach (byte theByte in crypto)
                {
                    oldHash.Append(theByte.ToString("x2"));
                }

                // Validation
                if (oldHash.ToString() == matchHash) 
                {
                    if (newPass.oldPassword != newPass.ConfirmPassword)
                    {
                        var newHash = new System.Text.StringBuilder();
                        crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                        foreach (byte theByte in crypto)
                        {
                            newHash.Append(theByte.ToString("x2"));
                        }
                        database.ChangePass(AccountID, newHash.ToString());
                        return RedirectToPage("/Accounts/Logout");
                    } else
                    {
                        ViewData["Error"] = "Old and New Password are the same!";
                    }
                } else
                {
                    ViewData["Error"] = "Invalid Old Password";
                }
            }
            return Page();
        }
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
    }
}
