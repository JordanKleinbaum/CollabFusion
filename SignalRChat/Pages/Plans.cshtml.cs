using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class PlansModel : PageModel
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
