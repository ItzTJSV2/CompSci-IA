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

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class RegisterAccountModel : PageModel
    {
        public string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MotorbikeDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
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
            [Required(ErrorMessage = "Confirm Password Required!")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match!")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            ViewData["Error"] = "";
        }

        bool EmailIsValid(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        bool PhoneIsValid(string phone)
        {
                string pattern = "^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                return Regex.IsMatch(phone, pattern);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CheckDB check = new CheckDB();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (EmailIsValid(Details.Email))
            {
                if (PhoneIsValid(Details.PhoneNumber))
                {
                    if ((check.CheckExist(Details.Username) == 0) && (check.CheckExist(Details.Email) == 0) && (Details.Username != "Admin"))
                    {
                        AddAccount(Details.Username, Details.Email, Details.PhoneNumber, Details.Password);
                        return RedirectToPage("/Accounts/Login");
                    }
                }
            }
            ViewData["Error"] = "Invalid Details.";
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

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                string queryStr = "INSERT INTO dbo.UserTable (Username, Email, PhoneNumber, Password) VALUES (@param1, @param2, @param3, @param4)";
                using (SqlCommand cmd = new SqlCommand(queryStr, connection))
                {
                    cmd.Parameters.Add("@param1", SqlDbType.NVarChar).Value = Username;
                    cmd.Parameters.Add("@param2", SqlDbType.NVarChar).Value = Email;
                    cmd.Parameters.Add("@param3", SqlDbType.NVarChar).Value = PhoneNum;
                    cmd.Parameters.Add("@param4", SqlDbType.NVarChar).Value = hashPass.ToString();
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    } catch (SqlException e)
                    {
                        Console.WriteLine(e.Message.ToString(), "Error.");
                    }
                }
            }
        }
    }
}
