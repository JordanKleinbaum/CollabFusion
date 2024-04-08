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

        public List<DocumentTable> Table { get; set; } = new List<DocumentTable>();
        public DocumentTable NewTable { get; set; }

        public List<PreviousSpendingAnalysis> PreviousSpendingAnalysisList { get; set; } = new List<PreviousSpendingAnalysis>();

        public string ErrorMessage { get; set; }


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
                SqlDataReader DocReader = DBClass.GeneralReaderQuery("SELECT * FROM Document WHERE DocumentTableID IS NOT NULL");

                // Populate Documents list with data from the database
                while (DocReader.Read())
                {
                    Doc.Add(new Document
                    {
                        Id = Convert.ToInt32(DocReader["Id"]),
                        FileName = DocReader["FileName"].ToString(),
                        FileData = (byte[])DocReader["FileData"],
                        DateAdded = Convert.ToDateTime(DocReader["DateAdded"]),
                        AnalysisType = DocReader["AnalysisType"].ToString(),
                        DocumentTableID = Convert.ToInt32(DocReader["DocumentTableID"])
                    });
                }
                DocReader.Close();
                DBClass.CollabFusionDBConnection.Close();

                //TableDocument Logic Start
                SqlDataReader TableReader = DBClass.GeneralReaderQuery($"SELECT * FROM DocumentTable WHERE CollabID = {collabid}");


                while (TableReader.Read())
                {
                    Table.Add(new DocumentTable
                    {
                        DocumentTableID = Convert.ToInt32(TableReader["DocumentTableID"]),
                        CollabID = Convert.ToInt32(TableReader["CollabID"]),
                        TableName = TableReader["TableName"].ToString()
                    });
                }
                TableReader.Close();
                DBClass.CollabFusionDBConnection.Close();
                //TableDocument Logic Ends


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

        public IActionResult OnPost(string tableName)
        {
            int collabId = HttpContext.Session.GetInt32("collabid") ?? 0;

            List<string> existingTableNames = new List<string>();

            using (SqlDataReader tableNamesReader = DBClass.GetAllTableNames())
            {
                while (tableNamesReader.Read())
                {
                    existingTableNames.Add(tableNamesReader["TableName"].ToString());
                }
                DBClass.CollabFusionDBConnection.Close();

            } // SqlDataReader is automatically closed and disposed after exiting the using block

            // Check if the entered table name already exists
            if (existingTableNames.Contains(tableName))
            {
                // Display error message to the user
                ErrorMessage = "Table name already exists. Please choose a different name.";
                return Page();

            }


            DocumentTable newTable = new DocumentTable
            {
                CollabID = collabId,
                TableName = tableName
            };

            DBClass.InsertTableDocument(newTable);
            DBClass.CollabFusionDBConnection.Close();

            SqlDataReader CollaborationNameReader = DBClass.GeneralReaderQuery($"SELECT CollaborationName FROM Collaboration WHERE CollabID = {collabId}");
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
            SqlDataReader DocReader = DBClass.GeneralReaderQuery("SELECT * FROM Document WHERE DocumentTableID IS NOT NULL");

            // Populate Documents list with data from the database
            while (DocReader.Read())
            {
                Doc.Add(new Document
                {
                    Id = Convert.ToInt32(DocReader["Id"]),
                    FileName = DocReader["FileName"].ToString(),
                    FileData = (byte[])DocReader["FileData"],
                    DateAdded = Convert.ToDateTime(DocReader["DateAdded"]),
                    AnalysisType = DocReader["AnalysisType"].ToString(),
                    DocumentTableID = Convert.ToInt32(DocReader["DocumentTableID"])
                });
            }
            DocReader.Close();
            DBClass.CollabFusionDBConnection.Close();

            //TableDocument Logic Start
            SqlDataReader TableReader = DBClass.GeneralReaderQuery($"SELECT * FROM DocumentTable WHERE CollabID = {collabId}");


            while (TableReader.Read())
            {
                Table.Add(new DocumentTable
                {
                    DocumentTableID = Convert.ToInt32(TableReader["DocumentTableID"]),
                    CollabID = Convert.ToInt32(TableReader["CollabID"]),
                    TableName = TableReader["TableName"].ToString()
                });
            }
            TableReader.Close();
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
            previousAnalysisReader.Close();
            DBClass.CollabFusionDBConnection.Close();

            return Page();
        }

    }
}