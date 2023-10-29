using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Details { get; set; }

        public void OnGet()
        {
            ViewData["Error"] = "";
        }
        public IActionResult OnPost()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToPage("/Index");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            string Password = Details.Password;

            var crypt = new SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(Password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            // CHECKING FOR ADMIN
            if (Details.Username == "Admin" && hash.ToString() == "1507b7e797cc160df67bfce22bcd370cc89f445fbb18ba9e07a08fdbbaef50f7")
            {
                LoginUser(Details.Username, "AdminAccount@mail.com", Details.RememberMe, true);
                return Redirect("/Index");
            }

            // CHECKING FOR REGULAR USER
            CheckDB check = new CheckDB();
            if (check.CheckExist(Details.Username) == 0)
            {
                ViewData["Error"] = "This Username / Email isn't found!";
                return Page();
            }

            // Get account details
            List<string> AccountDetails = check.getAccount(Details.Username);

            if (AccountDetails[4] == hash.ToString())
            {
                LoginUser(AccountDetails[1], AccountDetails[2], Details.RememberMe, false);
                return Redirect("/Index");
            }

            ViewData["Error"] = "Login details are incorrect!";
            return Page();
        }
        public async void LoginUser(string Username, string Email, bool RememberMe, bool Admin)
        {
            var claims = new List<Claim>
                { new Claim(ClaimTypes.Name, Username) ,
                  new Claim(ClaimTypes.Email, Email),
                  new Claim("Account", "True")
                };
            if (Admin)
            {
                claims.Add(new Claim("Admin", "True"));
            }

            var identity = new ClaimsIdentity(claims, "LoginCookieAuth");
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = RememberMe
            };
            
            await HttpContext.SignInAsync("LoginCookieAuth", claimsPrincipal, authProperties);
        }
        public class Credential {
            [Required(ErrorMessage = "Username Required!")]
            [Display(Name = "Username / Email")]
            public string Username { get; set; }
            [Required(ErrorMessage = "Password Required!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember Me")]
            public bool RememberMe { get; set; }
        }
    }
}
