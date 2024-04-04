using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class KnowledgeLibraryModel : PageModel
    {
        [BindProperty]
        public KnowledgeItem NewKnowledgeItem { get; set; }
        public List<KnowledgeItem> KnowledgeItemList { get; set; } = new List<KnowledgeItem>();

        public List<Users> UsersList { get; set; } = new List<Users>();

        public string SearchTerm { get; set; }
        public List<Document> Doc { get; set; } = new List<Document>();

        public List<PreviousSpendingAnalysis> PreviousSpendingAnalysisList { get; set; } = new List<PreviousSpendingAnalysis>();


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
                DBClass.CollabFusionDBConnection.Close();

                LoadAllKnowledge();
                // Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();

                SqlDataReader previousAnalysisReader = DBClass.GetAllPreviousSpendingAnalysis();
                while (previousAnalysisReader.Read())
                {
                    PreviousSpendingAnalysisList.Add(new PreviousSpendingAnalysis
                    {
                        SpendingAnalysisID = Convert.ToInt32(previousAnalysisReader["SpendingAnalysisID"]),
                        SpendingAnalysisName = previousAnalysisReader["SpendingAnalysisName"].ToString(),
                        SpendingAnalysisDescription = previousAnalysisReader["SpendingAnalysisDescription"].ToString(),
                        BasedOffOf = previousAnalysisReader["BasedOffOf"].ToString(),
                        SpendingAnalysisDate = Convert.ToDateTime(previousAnalysisReader["SpendingAnalysisDate"]).Date

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


        public IActionResult OnPostSearch(String SearchTerm)
        {
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Doc = DB.DBClass.SearchKnowledge(SearchTerm);

            }
            else
            {
                LoadAllKnowledge();
                //DBClass.CollabFusionDBConnection.Close();

            }
            return Page();
        }
        private void LoadAllKnowledge()
        {
            //Document LOGIC START
            //Document LOGIC START
            SqlDataReader reader = DBClass.GeneralReaderQuery("SELECT * FROM Document Order By DateAdded Desc");

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
            DB.DBClass.CollabFusionDBConnection.Close();

        }
    }
}