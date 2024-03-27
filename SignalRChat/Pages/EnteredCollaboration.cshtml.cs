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

                // Populate ChatList with user data
                SqlDataReader chatReader = DBClass.GetAllChats();
                while (chatReader.Read())
                {
                    ChatList.Add(new Chat
                    {
                        ChatID = Convert.ToInt32(chatReader["ChatID"]),
                        UserID = Convert.ToInt32(chatReader["UserID"]),
                        ChatText = chatReader["ChatText"].ToString(),
                        TimeStamp = chatReader["TimeStamp"] as DateTime?,
                    });
                }
                // Close the chatReader and the DataBase Connection
                chatReader.Close();
                DBClass.CollabFusionDBConnection.Close();


                // Populate the KnowledgeItem SELECT control
                SqlDataReader KnowledgeItemReader = DBClass.GeneralReaderQuery("SELECT * FROM KnowledgeItem WHERE CollabID = " + HttpContext.Session.GetInt32("collabid"));

                KnowledgeItems = new List<SelectListItem>();

                while (KnowledgeItemReader.Read())
                {
                    KnowledgeItems.Add(
                        new SelectListItem(
                            KnowledgeItemReader["KnowledgeTitle"].ToString(),
                            KnowledgeItemReader["KnowledgeID"].ToString()));
                }

                DBClass.CollabFusionDBConnection.Close();

                // Populate the KnowledgeItem SELECT control
                SqlDataReader PlanReader = DBClass.GeneralReaderQuery("SELECT * FROM Plans WHERE CollabID = " + HttpContext.Session.GetInt32("collabid"));

                Plans = new List<SelectListItem>();

                while (PlanReader.Read())
                {
                    Plans.Add(
                        new SelectListItem(
                            PlanReader["PlanName"].ToString(),
                            PlanReader["PlanID"].ToString()));
                }

                DBClass.CollabFusionDBConnection.Close();

                // ChatID LOGIC START
                SqlDataReader ChatReader = DBClass.GeneralReaderQuery("Select * FROM Chat");

                Chats = new List<SelectListItem>();

                while (ChatReader.Read())
                {
                    Chats.Add(new SelectListItem(
                        ChatReader["UserID"].ToString(),
                        ChatReader["ChatText"].ToString()));
                    //ChatReader["TimeStamp"].ToString()));
                }
                DBClass.CollabFusionDBConnection.Close();

                // ChatID LOGIC END

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
            // Populate the KnowledgeItem SELECT control again, just like in OnGet
            SqlDataReader KnowledgeItemReader = DBClass.GeneralReaderQuery("SELECT * FROM KnowledgeItem");

            KnowledgeItems = new List<SelectListItem>();

            while (KnowledgeItemReader.Read())
            {
                KnowledgeItems.Add(
                    new SelectListItem
                    {
                        Text = KnowledgeItemReader["KnowledgeTitle"].ToString(),
                        Value = KnowledgeItemReader["KnowledgeID"].ToString(),
                        Selected = (KnowledgeItemReader["KnowledgeID"].ToString() == KnowledgeID.ToString())
                    });
            }

            DBClass.CollabFusionDBConnection.Close();


            // Populate the Plan SELECT control again, just like in OnGet
            SqlDataReader PlanReader = DBClass.GeneralReaderQuery("SELECT * FROM Plans");

            Plans = new List<SelectListItem>();

            while (PlanReader.Read())
            {
                Plans.Add(
                    new SelectListItem
                    {
                        Text = PlanReader["PlanName"].ToString(),
                        Value = PlanReader["PlanID"].ToString(),
                        Selected = (PlanReader["PlanID"].ToString() == PlanID.ToString())
                    });
            }

            DBClass.CollabFusionDBConnection.Close();

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
            DBClass.InsertChat(Chat);
            DBClass.CollabFusionDBConnection.Close();
            return Page();
        }


    }
}