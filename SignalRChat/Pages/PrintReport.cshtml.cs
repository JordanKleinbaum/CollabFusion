using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class PrintReportModel : PageModel
    {
        // Retrieve the Current Date
        public DateTime CurrentDate { get; set; } = DateTime.Now;

        // Getting the Collaboration Name that you're in
        public string CollaborationName { get; set; }

        // KnowledgeItemsList
        public List<KnowledgeItem> KnowledgeItemList { get; set; } = new List<KnowledgeItem>();

        // Plan List
        public List<Plans> PlanList { get; set; } = new List<Plans>();

        // User List
        public List<Users> UserList { get; set; } = new List<Users>();


        public IActionResult OnGet(int collaborationid)
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                // Get the CollabName
                var collabid = HttpContext.Session.GetInt32("collabid");
                SqlDataReader CollaborationNameReader = DBClass.GeneralReaderQuery($"SELECT CollaborationName FROM Collaboration WHERE CollabID = {HttpContext.Session.GetInt32("collabid")}");
                if (CollaborationNameReader.Read())
                {
                    CollaborationName = CollaborationNameReader["CollaborationName"].ToString();
                }
                CollaborationNameReader.Close();
                DBClass.CollabFusionDBConnection.Close();

                //-------------------------------------------------------------------

                // Getting All KnowledgeItems
                SqlDataReader knowledgeReader = DBClass.GeneralReaderQuery($"SELECT * FROM KnowledgeItem WHERE CollabID = {HttpContext.Session.GetInt32("collabid")} ORDER BY Category");
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
                        //CollabID = Convert.ToInt32(knowledgeReader["CollabID"]),  <---- DON'T DELETE THESE WE NEED THEM LATER
                        //KnowledgeLibID = Convert.ToInt32(knowledgeReader["KnowledgeLibID"]) <---- DON'T DELETE THESE WE NEED THEM LATER
                    });
                }
                knowledgeReader.Close();

                // Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();
                // End of getting the CollabName

                //-------------------------------------------------------------------

                // Getting All Plans
                SqlDataReader planReader = DBClass.GeneralReaderQuery($"SELECT * FROM Plans WHERE CollabID = {HttpContext.Session.GetInt32("collabid")}");
                while (planReader.Read())
                {
                    PlanList.Add(new Plans
                    {
                        PlanID = Convert.ToInt32(planReader["PlanID"]),
                        PlanName = planReader["PlanName"].ToString(),
                        PlanContents = planReader["PlanContents"].ToString(),
                        DateCreated = planReader["DateCreated"] as DateTime?,
                        CollabID = HttpContext.Session.GetInt32("collabid")
                    });
                }

                // Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();

                //-------------------------------------------------------------------

                // Getting All Users
                SqlDataReader userReader = DBClass.GeneralReaderQuery($"SELECT DISTINCT Users.UserID, Users.FirstName, Users.LastName FROM Users INNER JOIN KnowledgeItem ON Users.UserID = KnowledgeItem.UserID WHERE KnowledgeItem.CollabID = {HttpContext.Session.GetInt32("collabid")}");
                while (userReader.Read())
                {
                    UserList.Add(new Users
                    {
                        UserID = Convert.ToInt32(userReader["UserID"]),
                        FirstName = userReader["FirstName"].ToString(),
                        LastName = userReader["LastName"].ToString(),
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
    }
}
