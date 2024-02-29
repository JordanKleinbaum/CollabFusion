using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration.UserSecrets;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

namespace SignalRChat.Pages
{
    public class KnowledgeItemModel : PageModel
    {

        [BindProperty]
        [Required]
        public KnowledgeItem NewKnowledgeItem { get; set; }
        public List<KnowledgeItem> KnowledgeItemList { get; set; } = new List<KnowledgeItem>();
        public List<Users> UsersList { get; set; } = new List<Users>();

        /*POPULATE BUTTON*/
        public IActionResult OnPostPopulateButton()
        {
            // Populate UsersList with user data from the database
            UsersList.Clear(); // Clear the list to avoid duplicate entries
            SqlDataReader userReader = DBClass.GetAllUsers();
            while (userReader.Read())
            {
                UsersList.Add(new Users
                {
                    UserID = Convert.ToInt32(userReader["UserID"]),
                    Username = userReader["Username"].ToString()
                });
            }
            userReader.Close();
            DBClass.CollabFusionDBConnection.Close();

            var currentUsername = HttpContext.Session.GetString("username");

            if (!ModelState.IsValid)
            {
                ModelState.Clear();

                NewKnowledgeItem.KnowledgeTitle = "Brains";
                NewKnowledgeItem.KnowledgeSubject = "Talking about how amazing our group is";
                NewKnowledgeItem.Category = "Intelligence";
                //NewKnowledgeItem.Strengths = "Programming Skills";
                //NewKnowledgeItem.Weaknesses = "Communication";
                //NewKnowledgeItem.Opportunities = "Team Bonding";
                //NewKnowledgeItem.Threats = "Time Constraint";
                //NewKnowledgeItem.Information = $"Strengths: {NewKnowledgeItem.Strengths}\nWeaknesses: {NewKnowledgeItem.Weaknesses}\nOpportunities: {NewKnowledgeItem.Opportunities}\nThreats: {NewKnowledgeItem.Threats}";
                NewKnowledgeItem.Information = "We can do anything";
                NewKnowledgeItem.KNDate = DateTime.Now;

                // Find the UserID associated with the current user
                var user = UsersList.FirstOrDefault(u => u.Username == currentUsername);
                NewKnowledgeItem.UserID = user?.UserID;

            }

            return Page();
        }


        /*CLEAR BUTTON*/
        public IActionResult OnPostClearButton()
        {
            if (HttpContext.Request.Method == "POST")
            {
                ModelState.Clear();

                // Clear all the fields in NewKnowledgeItem
                NewKnowledgeItem.KnowledgeTitle = "";
                NewKnowledgeItem.KnowledgeSubject = "";
                NewKnowledgeItem.Category = "";
                NewKnowledgeItem.Strengths = "";
                NewKnowledgeItem.Weaknesses = "";
                NewKnowledgeItem.Opportunities = "";
                NewKnowledgeItem.Threats = "";
                NewKnowledgeItem.Information = "";
                NewKnowledgeItem.KNDate = null;
                NewKnowledgeItem.UserID = null;

                // Repopulate UsersList for the form to be able to display the dropdown properly
                PopulateUsersList();
            }
            return Page();
        }

        private void PopulateUsersList()
        {
            UsersList.Clear();
            using (var command = new SqlCommand("SELECT UserID, Username FROM Users", DBClass.CollabFusionDBConnection))
            {
                DBClass.CollabFusionDBConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        UsersList.Add(new Users
                        {
                            UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                            Username = reader.GetString(reader.GetOrdinal("Username"))
                        });

                    }
                }
                DBClass.CollabFusionDBConnection.Close();
            }
        }


        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Populate UsersList with user data from the database
                SqlDataReader userReader = DBClass.GetAllUsers();
                while (userReader.Read())
                {
                    UsersList.Add(new Users
                    {
                        UserID = Convert.ToInt32(userReader["UserID"]),
                        Username = userReader["Username"].ToString()
                    });
                }
                userReader.Close();

                SqlDataReader knowledgeReader = DBClass.GetAllKnowledgeItems();
                while (knowledgeReader.Read())
                {
                    KnowledgeItemList.Add(new KnowledgeItem
                    {
                        KnowledgeID = Convert.ToInt32(knowledgeReader["KnowledgeID"]),
                        KnowledgeTitle = knowledgeReader["KnowledgeTitle"].ToString(),
                        KnowledgeSubject = knowledgeReader["KnowledgeSubject"].ToString(),
                        Category = knowledgeReader["Category"].ToString(),
                        Information = knowledgeReader["Information"]?.ToString(),
                        KNDate = knowledgeReader["KNDate"] as DateTime?,
                        UserID = Convert.ToInt32(knowledgeReader["UserID"]),
                        CollabID = HttpContext.Session.GetInt32("collabid")
                        //KnowledgeLibID = Convert.ToInt32(knowledgeReader["KnowledgeLibID"])

                });
                }

                // Close your connection in DBClass
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
            // Populate UsersList with user data from the database
            UsersList.Clear(); // Clear the list to avoid duplicate entries
            SqlDataReader userReader = DBClass.GetAllUsers();
            while (userReader.Read())
            {
                UsersList.Add(new Users
                {
                    UserID = Convert.ToInt32(userReader["UserID"]),
                    Username = userReader["Username"].ToString()
                });
            }
            userReader.Close();
            DBClass.CollabFusionDBConnection.Close();

            var currentUsername = HttpContext.Session.GetString("username");

            var user = UsersList.FirstOrDefault(u => u.Username == currentUsername);
            NewKnowledgeItem.UserID = user?.UserID;

            NewKnowledgeItem.CollabID = HttpContext.Session.GetInt32("collabid");

            DBClass.InsertKnowledgeItem(NewKnowledgeItem);

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage("KnowledgeItem");
        }
    }

}