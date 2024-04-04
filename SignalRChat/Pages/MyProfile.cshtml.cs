using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System;

namespace SignalRChat.Pages
{
    public class MyProfileModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MyProfileModel(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserFirstName { get; set; }
        public Users User { get; set; } = new Users(); // Only one user

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("_userid");

            if (userId.HasValue)
            {
                // Fetch user information based on userID
                User = DBClass.UserInfoBasedOnID(_httpContextAccessor); // Call the correct method

                if (User != null)
                {
                    UserFirstName = User.FirstName; // Set user's first name
                    return Page();
                }
                else
                {
                    // Handle the case where user info is not found
                    HttpContext.Session.SetString("ErrorMessage", "User information not found.");
                    return RedirectToPage("/Index");
                }
            }

            HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
            return RedirectToPage("/Index");
        }
    }
}
