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
            if (HttpContext.Session.GetString("username") != null)
            {
                User = DBClass.UserInfoBasedOnID(_httpContextAccessor);
                var username = HttpContext.Session.GetString("username");
                UserFirstName = DBClass.GetFirstNameByUsername(username);
                DBClass.CollabFusionDBConnection.Close();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnPost()
        {
            DBClass.UpdateUser(ChangeUser, _httpContextAccessor);
            return RedirectToPage("CollabHub");

        }
    }

}
