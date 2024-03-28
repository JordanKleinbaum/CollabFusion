using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{

    public class EnteredCollaborationModel : PageModel
    {
        [BindProperty]
        public int KnowledgeID { get; set; }

        [BindProperty]
        public string KnowledgeTitle { get; set; }

        [BindProperty]
        public int PlanID { get; set; }

        [BindProperty]
        public string PlanName { get; set; }

        public List<SelectListItem> KnowledgeItems { get; set; }

        public List<SelectListItem> Plans { get; set; }

        public List<SelectListItem> Chats { get; set; }

        public List<Users> UsersList { get; set; } = new List<Users>();

        [BindProperty]
        public int ChatID { get; set; }

        [BindProperty]
        public Chat Chat { get; set; }

        public List<Chat> ChatList { get; set; } = new List<Chat>();

        public string UserFirstName { get; set; }

        public string CollaborationName { get; set; }
        public List<Document> Doc { get; set; } = new List<Document>();
       


        public IActionResult OnGet(int collaborationid)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Get the CollabName
                HttpContext.Session.SetInt32("collabid", collaborationid);
                var collabid = HttpContext.Session.GetInt32("collabid");
                SqlDataReader CollaborationNameReader = DBClass.GeneralReaderQuery($"SELECT CollaborationName FROM Collaboration WHERE CollabID = {collabid}");
                if (CollaborationNameReader.Read())
                {
                    CollaborationName = CollaborationNameReader["CollaborationName"].ToString();
                }
                CollaborationNameReader.Close();
                DBClass.CollabFusionDBConnection.Close();
                // End of getting the CollabName

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

                //Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();

   

                //Document LOGIC START
                SqlDataReader reader = DBClass.GeneralReaderQuery("SELECT * FROM Document");

                // Populate PESTDocuments list with data from the database
                while (reader.Read())
                {
                    Doc.Add(new Document
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        FileName = reader["FileName"].ToString(),
                        FileData = (byte[])reader["FileData"],
                        DateAdded = Convert.ToDateTime(reader["DateAdded"]),
                        AnalysisType = reader["AnalysisType"].ToString()
                    });
                }
                reader.Close();
                DBClass.CollabFusionDBConnection.Close();

                

                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }
        }

        public void OnPostSubmit()
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

            //Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
        }

        public IActionResult OnPost()
        {
            
            return Page();
        }

    }
}