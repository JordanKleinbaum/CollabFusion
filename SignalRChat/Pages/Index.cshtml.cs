using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public KnowledgeItem NewKnowledgeItem { get; set; }
        public List<KnowledgeItem> KnowledgeItemList { get; set; } = new List<KnowledgeItem>();

        public List<Users> UsersList { get; set; } = new List<Users>();

        public string SearchTerm { get; set; }
        public List<PublicDocument> Doc { get; set; } = new List<PublicDocument>();

        public List<PublicPreviousSpendingAnalysis> PublicPreviousSpendingAnalysis { get; set; } = new List<PublicPreviousSpendingAnalysis>();

        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
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
            LoadAllAnalysis();
            // Close your connection in DBClass
            DBClass.CollabFusionDBConnection.Close();
            return Page();

        }


        public IActionResult OnPostSearch(String SearchTerm)
        {
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                Doc = DB.DBClass.SearchPublicKnowledge(SearchTerm);

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

            SqlDataReader reader = DBClass.GeneralReaderQuery("SELECT * FROM PublicDocument Order By DateAdded Desc");

            while (reader.Read())
            {
                Doc.Add(new PublicDocument
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

        private void LoadAllAnalysis()
        {
            SqlDataReader reader = DBClass.GeneralReaderQuery("SELECT * FROM PublicPreviousSpendingAnalysis Order By SpendingAnalysisDate Desc");

            while (reader.Read())
            {
                PublicPreviousSpendingAnalysis.Add(new PublicPreviousSpendingAnalysis
                {
                    SpendingAnalysisID = Convert.ToInt32(reader["SpendingAnalysisID"]),
                    SpendingAnalysisName = reader["SpendingAnalysisName"].ToString(),
                    SpendingAnalysisDescription = reader["SpendingAnalysisDescription"].ToString(),
                    BasedOffOf = reader["BasedOffOf"].ToString(),
                    SpendingAnalysisDate = Convert.ToDateTime(reader["SpendingAnalysisDate"]),
                    Column1 = Convert.ToInt32(reader["Column1"]),
                    Column2 = Convert.ToInt32(reader["Column2"])
                });
            }
            reader.Close();
            DB.DBClass.CollabFusionDBConnection.Close();
        }

        public IActionResult OnPost(int column1, int column2, string fileName)
        {
            // Validate and process the column values
            // You can also perform additional validation if needed

            // Redirect to another page passing the required parameters
            return RedirectToPage("/ViewPublicRevenueAnalysis", new { fileName, column1, column2 });
        }
    }
}