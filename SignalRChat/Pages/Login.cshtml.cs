using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DB;

namespace SignalRChat.Pages
{
    public class LoginModel : PageModel
    {
        // I DONT KNOW WHAT THIS DOES BUT IT WAS HERE BY DEFAULT ON INDEX PAGE
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(ILogger<LoginModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string Admin { get; set; }

        public IActionResult OnGet(String logout)
        {
            if (logout == "true")
            {
                HttpContext.Session.Clear();
                ViewData["LoginMessage"] = "Successfully Logged Out!";
            }

            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToPage("CollabHub");
            }

            return Page();
        }

        public IActionResult OnPost()
        {

            if (DBClass.sp_Lab3Login(Username, Password))
            {
                HttpContext.Session.SetString("username", Username);
                DBClass.CollabFusionDBConnection.Close();
                DBClass.AuthDBConnection.Close();
                return RedirectToPage("CollabHub");
            }

            else
            {
                ViewData["LoginMessage"] = "Username and/or Password Incorrect";
                DBClass.CollabFusionDBConnection.Close();
                DBClass.AuthDBConnection.Close();

                return Page();
            }
        }

        public IActionResult OnPostLogoutHandler()
        {
            HttpContext.Session.Clear();
            return Page();
        }
    }
}

