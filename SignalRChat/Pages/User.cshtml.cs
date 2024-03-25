using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SignalRChat.Pages
{
    public class UserModel : PageModel
    {
        [BindProperty]
        [Required]
        public Users NewUser { get; set; }

        public List<Users> UserList { get; set; } = new List<Users>();

        /*POPULATE AND CLEAR METHODS*/
        public IActionResult OnPostPopulateButton()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                NewUser.Username = "User100";
                NewUser.Password = "UsersPassword100";
                NewUser.FirstName = "Jeremy";
                NewUser.LastName = "Ezell";
                NewUser.Email = "SkittleWater@gmail.com";
                NewUser.Phone = "9176990397";
                NewUser.Country = "United States of America";
                NewUser.State = "New Jersey";
                NewUser.City = "Scotch Plains";
                NewUser.Street = "2220 New York Avenue";
                NewUser.ZipCode = "07076";
                NewUser.Admin = "No";
            }
            return Page();
        }

        /*CLEAR BUTTON*/
        public IActionResult OnPostClearButton()
        {
            if (HttpContext.Request.Method == "POST")
            {
                ModelState.Clear();
                NewUser.Username = "";
                NewUser.Password = "";
                NewUser.FirstName = "";
                NewUser.LastName = "";
                NewUser.Email = "";
                NewUser.Phone = "";
                NewUser.Country = "";
                NewUser.State = "";
                NewUser.City = "";
                NewUser.Street = "";
                NewUser.ZipCode = "";
                NewUser.Admin = "";
            }
            return Page();
        }

        /*DATABASE STUFF*/
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToPage("CollabHub");
            }

            SqlDataReader userReader = DBClass.GetAllUsers();
            while (userReader.Read())
            {
                UserList.Add(new Users
                {
                    UserID = Convert.ToInt32(userReader["UserID"]),
                    Username = userReader["Username"].ToString(),
                    //Password = userReader["Password"].ToString(), COMMENTED OUT THIS LINE
                    FirstName = userReader["FirstName"].ToString(),
                    LastName = userReader["LastName"].ToString(),
                    Email = userReader["Email"].ToString(),
                    Phone = userReader["Phone"].ToString(),
                    Street = userReader["Street"].ToString(),
                    City = userReader["City"].ToString(),
                    State = userReader["State"]?.ToString(),
                    Country = userReader["Country"].ToString(),
                    ZipCode = userReader["ZipCode"].ToString(),
                    Admin = userReader["Admin"].ToString()
                });
            }

            // Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
            DBClass.AuthDBConnection.Close();


            return Page();

        }

        public IActionResult OnPost()
        {
            // Use parameterized query for insertion
            DBClass.ParameterizedCreateUser(NewUser);
            DBClass.CreateHashedUser(NewUser.Username, NewUser.Password);

            DBClass.AuthDBConnection.Close();

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage("Index");
        }
    }
}
