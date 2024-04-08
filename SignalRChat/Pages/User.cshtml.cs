using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;


namespace SignalRChat.Pages
{

    public class UserModel : PageModel
    {
        [BindProperty]
        [Required]
        public Users NewUser { get; set; }

        public List<Users> UserList { get; set; } = new List<Users>();
        public List<Collaboration> CollabList { get; set; } = new List<Collaboration>();

        [BindProperty]
        [Required]
        public List<Collaboration> SelectedCollaborations { get; set; } = new List<Collaboration>();

        /*POPULATE AND CLEAR METHODS*/
        public IActionResult OnPostPopulateButton()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear();
                NewUser.Username = "john123";
                NewUser.Password = "john123";
                NewUser.FirstName = "John";
                NewUser.LastName = "Cathell";
                NewUser.Email = "john@madisoncounty.org";
                NewUser.Phone = "9176990397";
                NewUser.Country = "United States of America";
                NewUser.State = "Virginia";
                NewUser.City = "Leesburg";
                NewUser.Street = "2363 Bayberry Lane";
                NewUser.ZipCode = "22801";
                NewUser.Admin = "General User";
            }
            SqlDataReader collabReader = DBClass.GetAllCollabs();
            while (collabReader.Read())
            {
                CollabList.Add(new Collaboration
                {
                    CollabID = Convert.ToInt32(collabReader["CollabID"]),
                    CollaborationName = collabReader["CollaborationName"].ToString(),
                    NotesAndInformation = collabReader["NotesAndInformation"].ToString(),
                });
            }

            collabReader.Close();
            // Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
            DBClass.AuthDBConnection.Close();
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
            SqlDataReader collabReader = DBClass.GetAllCollabs();
            while (collabReader.Read())
            {
                CollabList.Add(new Collaboration
                {
                    CollabID = Convert.ToInt32(collabReader["CollabID"]),
                    CollaborationName = collabReader["CollaborationName"].ToString(),
                    NotesAndInformation = collabReader["NotesAndInformation"].ToString(),
                });
            }

            collabReader.Close();
            // Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
            DBClass.AuthDBConnection.Close();
            return Page();
        }

        /*DATABASE STUFF*/
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
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
            userReader.Close();
            // Close your connection in DBClass




            SqlDataReader collabReader = DBClass.GetAllCollabs();
            while (collabReader.Read())
            {
                CollabList.Add(new Collaboration
                {
                    CollabID = Convert.ToInt32(collabReader["CollabID"]),
                    CollaborationName = collabReader["CollaborationName"].ToString(),
                    NotesAndInformation = collabReader["NotesAndInformation"].ToString(),
                });
            }

            collabReader.Close();
            // Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
            DBClass.AuthDBConnection.Close();


            return Page();

        }

        public IActionResult OnPost()
        {



            // Retrieve selected collaboration IDs from form
            var selectedCollabIds = HttpContext.Request.Form["SelectedCollabs"];

            // Populate selectedIds with Collaboration IDs
            List<int> selectedIds = new List<int>();
            foreach (var collabId in selectedCollabIds)
            {
                if (int.TryParse(collabId, out int id))
                {
                    selectedIds.Add(id);
                }
            }


            DBClass.CreateHashedUser(NewUser.Username, NewUser.Password);

            // Create new user and insert into bridge table
            DBClass.CreateUserAndInsertCollabUserBridgeTable(NewUser, selectedIds);

            return RedirectToPage("/User");
        }



    }
}
