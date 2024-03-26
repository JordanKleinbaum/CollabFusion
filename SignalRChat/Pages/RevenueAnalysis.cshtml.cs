using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalRChat.Pages
{
    public class RevenueAnalysisModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }
        }
    }
}
