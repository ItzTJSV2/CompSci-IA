using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Security.Claims;
using System.Xml.Linq;

namespace MainProject.Pages.Accounts
{
    public class MyAccountModel : PageModel
    {
        [BindProperty]
        public Credential? Account { get; set; }
        public class Credential
        {
            public string? Username { get; set; }
            [DataType(DataType.EmailAddress)]
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public string ChangeUsername = "";
        public string ChangeEmail = "";
        public string ChangePhone = "";
        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.Name != null)
            {
                CheckDB database = new CheckDB();
                List<string> AccountDetails = database.getAccount(User.Identity.Name);
                ViewData["Error"] = "";
                ViewData["AccountID"] = AccountDetails[0];
                ViewData["Username"] = AccountDetails[1];
                ViewData["Email"] = AccountDetails[2];
                ViewData["Phone"] = AccountDetails[3];

                ChangeUsername = AccountDetails[1];
                ChangePhone = AccountDetails[3];
                ChangeEmail = AccountDetails[2];
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        RegisterAccountModel Checks = new RegisterAccountModel();
        CheckDB Database = new CheckDB();

        bool ChangeValidation(List<string> AccountDetails)
        {
            ChangeUsername = AccountDetails[1];
            ChangePhone = AccountDetails[3];
            ChangeEmail = AccountDetails[2];
            bool flag = false;

            // USERNAME BOX
            try // Basically an if statement to check if it's null.
            {
                if (Account.Username != null && Account.Username != ChangeUsername)
                {
                    if (Database.CheckExist(Account.Username) == 0)
                    {
                        flag = true;
                        ChangeUsername = Account.Username;
                    }
                    else
                    {
                        ViewData["Error"] = "Username already taken.";
                    }
                }
            }
            catch (NullReferenceException)
            {
            }

            // EMAIL BOX
            try
            {
                if (Account.Email != null && Account.Email != ChangeEmail)
                {
                    if (Checks.EmailIsValid(Account.Email))
                    {
                        if (Database.CheckExist(Account.Email) == 0)
                        {
                            flag = true;
                            ChangeEmail = Account.Email;
                        }
                        else
                        {
                            ViewData["Error"] = "Email already taken.";
                        }
                    }
                    else
                    {
                        ViewData["Error"] = "Email is invalid.";
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
            try
            {
                if (Account.PhoneNumber != null && Account.PhoneNumber != ChangePhone)
                {
                    if (Checks.PhoneIsValid(Account.PhoneNumber))
                    {
                        flag = true;
                        ChangePhone = Account.PhoneNumber;
                    }
                    else
                    {
                        ViewData["Error"] = "Phone number is invalid.";
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
            return flag;
        }
        public async Task<IActionResult> OnPostAsync(string Type)
        {
            List<string> AccountDetails = Database.getAccount(User.Identity.Name);
            int AccountID = Int16.Parse(AccountDetails[0]);
            if (Type == "Change")
            {
                if (ChangeValidation(AccountDetails))
                {
                    bool IsPersistent = false;
                    if (User.Identity.IsAuthenticated)
                    {
                        Claim claim = ((ClaimsIdentity)User.Identity).FindFirst("IsPersistent");
                        IsPersistent = claim != null ? Convert.ToBoolean(claim.Value) : false;
                    }
                    Database.ChangeField(AccountID, ChangeUsername, ChangeEmail, ChangePhone);

                    await HttpContext.SignOutAsync("LoginCookieAuth");
                    return RedirectToPage("/Index");
                } else
                {
                    ViewData["AccountID"] = AccountDetails[0];
                    ViewData["Username"] = AccountDetails[1];
                    ViewData["Email"] = AccountDetails[2];
                    ViewData["Phone"] = AccountDetails[3];
                }
            }
            if (Type == "Delete")
            {
                Database.DelAccount(AccountID);
                return RedirectToPage("/Index");
            }
            return Page();
        }
        
    }
}
