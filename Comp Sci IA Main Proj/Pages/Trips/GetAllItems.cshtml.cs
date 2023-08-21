using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Comp_Sci_IA_Main_Proj_.Pages.Trips
{
    [Authorize(Policy = "AdminAccount")]
    public class GetAllItemsModel : PageModel
    {
        public void OnGet()
        {

        }
    }
}
