using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using static Comp_Sci_IA_Main_Proj_.Pages.Accounts.LoginModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Net.Mail;
using System.Data;
using System.Text.RegularExpressions;
using System.Data.SQLite;

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class RegisterAccountModel : PageModel
    {
        public static string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
        public static string Dir = Path.Combine(parentDir, @"Mainproject", @"database", @"Database.db");
        public static string ConnectionString = ("Data Source=" + Dir);
        [BindProperty]
        public Credential Details { get; set; }
        public class Credential
        {
            [Required(ErrorMessage = "Username Required!")]
            [Display(Name = "Username")]
            public string Username { get; set; }
            [Required(ErrorMessage = "Email Required!")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            [Required(ErrorMessage = "Phone Number Required!")]
            public string PhoneNumber { get; set; }
            [Required(ErrorMessage = "Password Required!")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required(ErrorMessage = "Confirmation Required!")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match!")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            ViewData["Error"] = "";
        }

        public bool EmailIsValid(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        public bool PhoneIsValid(string phone)
        {
                string pattern = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                return Regex.IsMatch(phone, pattern);
        }

        // SUBMIT BUTTON CLICK
        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity.Name != null)
            {
                return RedirectToPage("/Index");
            }
            CheckDB check = new CheckDB();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (EmailIsValid(Details.Email))
            {
                if (PhoneIsValid(Details.PhoneNumber))
                {
                    if (check.CheckExist(Details.Username) == 0 && Details.Username.ToLower() != "admin")
                    {
                        if (check.CheckExist(Details.Email) == 0)
                        {
                            if (Details.Username == "Admin") {
                                ViewData["Error"] = "Invalid Username.";
                            } else
                            {
                                AddAccount(Details.Username, Details.Email, Details.PhoneNumber, Details.Password);
                                return RedirectToPage("/Accounts/Login");
                            }
                        } else
                        {
                            ViewData["Error"] = "Email is already taken.";
                        }
                    } else
                    {
                        ViewData["Error"] = "Username is already taken.";
                    }

                } else
                {
                    ViewData["Error"] = "Invalid Phone Number.";
                }

            } else
            {
                ViewData["Error"] = "Email is invalid.";
            }
            return Page();
        }

        public void AddAccount(string Username, string Email, string PhoneNum, string Password)
        {

            // Convert Password to SHA256 Hash
            var crypt = new SHA256Managed();
            var hashPass = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(Password));
            foreach (byte theByte in crypto)
            {
                hashPass.Append(theByte.ToString("x2"));
            }

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string queryStr = "INSERT INTO UserTable (Username, Email, PhoneNumber, Password, Trips) VALUES (@param1, @param2, @param3, @param4, @param5)";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = queryStr;
                    command.Parameters.AddWithValue("@param1", Username);
                    command.Parameters.AddWithValue("@param2", Email);
                    command.Parameters.AddWithValue("@param3", PhoneNum);
                    command.Parameters.AddWithValue("@param4", hashPass.ToString());
                    command.Parameters.AddWithValue("@param5", "");
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
    }
}
