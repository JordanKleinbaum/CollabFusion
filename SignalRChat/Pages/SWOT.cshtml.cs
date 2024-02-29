using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class SWOTModel : PageModel
    {
        [BindProperty]
        [Required]
        public KnowledgeItem NewKnowledgeItem { get; set; }
        public List<KnowledgeItem> KnowledgeItemList { get; set; } = new List<KnowledgeItem>();
        public List<Users> UsersList { get; set; } = new List<Users>();
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
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

                NewKnowledgeItem.KnowledgeTitle = "Project SWOT Analysis";
                NewKnowledgeItem.KnowledgeSubject = "Group Project";
                NewKnowledgeItem.Category = "SWOT";
                NewKnowledgeItem.Strengths = "Programming Skills";
                NewKnowledgeItem.Weaknesses = "Communication";
                NewKnowledgeItem.Opportunities = "Team Bonding";
                NewKnowledgeItem.Threats = "Time Constraint";
                NewKnowledgeItem.Information = $"Strengths: {NewKnowledgeItem.Strengths}\nWeaknesses: {NewKnowledgeItem.Weaknesses}\nOpportunities: {NewKnowledgeItem.Opportunities}\nThreats: {NewKnowledgeItem.Threats}";
                //NewKnowledgeItem.Information = "We can do anything";
                NewKnowledgeItem.KNDate = DateTime.Now;

                // Find the UserID associated with the current user
                var user = UsersList.FirstOrDefault(u => u.Username == currentUsername);
                NewKnowledgeItem.UserID = user?.UserID;
                NewKnowledgeItem.CollabID = 1;
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
                //PopulateUsersList();
            }
            return Page();
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
            NewKnowledgeItem.Category = "SWOT";
            NewKnowledgeItem.Information = $"Strengths: {NewKnowledgeItem.Strengths}\nWeaknesses: {NewKnowledgeItem.Weaknesses}\nOpportunities: {NewKnowledgeItem.Opportunities}\nThreats: {NewKnowledgeItem.Threats}";
            NewKnowledgeItem.CollabID = HttpContext.Session.GetInt32("collabid");

            DBClass.InsertKnowledgeItem(NewKnowledgeItem);

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage("SWOT");
        }
    }
}