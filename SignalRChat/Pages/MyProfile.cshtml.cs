using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System;
using System.ComponentModel.DataAnnotations;

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
        public Users User { get; set; } = new Users();

        [BindProperty]
        [Required]
        public Users ChangeUser { get; set; }

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("_userid");

            if (userId.HasValue)
            {
                User = DBClass.UserInfoBasedOnID(_httpContextAccessor);

                if (User != null)
                {
                    UserFirstName = User.FirstName;
                    DBClass.CollabFusionDBConnection.Close();
                    return Page();
                }
                else
                {
                    HttpContext.Session.SetString("ErrorMessage", "User information not found.");
                    return RedirectToPage("/Index");
                }
            }

            HttpContext.Session.SetString("LoginError", "You must log in to access that page!");
            return RedirectToPage("/Index");
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DBClass.UpdateUser(ChangeUser, _httpContextAccessor);
                    return RedirectToPage("CollabHub");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it appropriately
                    ModelState.AddModelError("", "An error occurred while updating user information.");
                    return Page();
                }
            }
            return Page(); // Return the same page if model state is not valid
        }
    }

}
