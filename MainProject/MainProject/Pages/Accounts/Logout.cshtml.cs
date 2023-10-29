using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync("LoginCookieAuth");
            return RedirectToPage("/Index");
        }
    }
}
