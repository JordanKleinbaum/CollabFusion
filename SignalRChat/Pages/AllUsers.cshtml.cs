using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class AllUsersModel : PageModel
    {
        public List<Users> UserList { get; set; } = new List<Users>();

        public IActionResult OnGet()
        {

            if (HttpContext.Session.GetString("username") != null)
            {
                SqlDataReader userReader = DBClass.GetAllUsers();
                while (userReader.Read())
                {
                    UserList.Add(new Users
                    {
                        UserID = Convert.ToInt32(userReader["UserID"]),
                        Username = userReader["Username"].ToString(),
                        //Password = userReader["Password"].ToString(),
                        FirstName = userReader["FirstName"].ToString(),
                        LastName = userReader["LastName"].ToString(),
                        Email = userReader["Email"].ToString(),
                        Phone = userReader["Phone"].ToString(),
                        Street = userReader["Street"].ToString(),
                        City = userReader["City"].ToString(),
                        State = userReader["State"]?.ToString(),
                        Country = userReader["Country"].ToString(),
                        ZipCode = userReader["ZipCode"].ToString()
                    });
                }

                DBClass.CollabFusionDBConnection.Close();
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }


        }

        //// IF YOU DON'T WANT A LOGGED OUT USER TO GO TO THIS PAGE, USE THIS METHOD
        //public IActionResult OnGet()
        //{
        //    if (HttpContext.Session.GetString("username") != null)
        //    {
        //        return Page();
        //    }
        //    else
        //    {
        //        HttpContext.Session.SetString("LoginError", "You must login to access that page!");
        //        return RedirectToPage("Index");
        //    }
        //}
    }
}
