using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Constraints;
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
        //

        public List<PreviousSpendingAnalysis> PreviousSpendingAnalysisList { get; set; } = new List<PreviousSpendingAnalysis>();

        [BindProperty]
        public IFormFile File { get; set; }

        [BindProperty]
        public string AnalysisType { get; set; }
        public string FileName { get; set; }

        public string FileData { get; set; }

        public string DateAdded { get; set; }

        public string AnalysesType { get; set; }
        // Connection Object at Data Field Level
        public static SqlConnection CollabFusionDBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly string CollabFusionDBConnString = "Server=sharpmindsdb1.database.windows.net,1433;" + "Database=Lab3;" + "User Id=gwen;" + "Password=sharpminds1!;" + "Encrypt=True;" + "TrustServerCertificate=True";


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
                        SpendingAnalysisDate = Convert.ToDateTime(previousAnalysisReader["SpendingAnalysisDate"]).Date,
                        Column1 = Convert.ToInt32(previousAnalysisReader["Column1"]),
                        Column2 = Convert.ToInt32(previousAnalysisReader["Column2"])
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

        public IActionResult OnPost(int column1, int column2, string fileName)
        {
            // Validate and process the column values
            // You can also perform additional validation if needed

            // Redirect to another page passing the required parameters
            return RedirectToPage("/ViewRevenueAnalysis", new { fileName, column1, column2 });
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

        //public IActionResult OnPostPush(int Id)
        //{
        //    //var reader = new DBClass.PublicKnowledgeReader(id);
        //    //FileName = reader.FileName;
        //    ////Document documentToPush = Doc.Find(doc => doc.Id == id);
        //    //if (File != null && File.Length > 0)
        //    //{
        //    //    using (var memoryStream = new MemoryStream())
        //    //    {
        //    //        File.CopyTo(memoryStream);
        //    //        var fileData = memoryStream.ToArray();

        //    //        var document = new PublicDocument
        //    //        {
        //    //            FileName = Path.GetFileName(File.FileName),
        //    //            FileData = fileData,
        //    //            DateAdded = DateTime.Now,
        //    //            AnalysisType = AnalysisType
        //    //        };

        //    //        DBClass.InsertPublicDocument(document);
        //    //        DBClass.CollabFusionDBConnection.Close();

        //    //        // Set success message in TempData
        //    //        TempData["UploadSuccessMessage"] = "File uploaded successfully.";


        //    //        return Page();
        //    //        //return RedirectToPage("/EnteredCollaboration", new { collaborationid = HttpContext.Session.GetInt32("collabid") });
        //    //    }
        //    //}
        //    //return Page();


        //    List<PublicDocument> SearchedDocument = new List<PublicDocument>();

        //    using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
        //    {
        //        connection.Open();
        //        string sqlQuery = "SELECT * FROM PublicDocument WHERE Id LIKE @Id";
        //        using (SqlCommand command = new SqlCommand(sqlQuery, connection))
        //        {
        //            command.Parameters.AddWithValue("@Id", "%" + Id + "%");
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    PublicDocument knowledge = new PublicDocument
        //                    {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        FileName = reader["FileName"].ToString(),
        //                        FileData = (byte[])reader["FileData"],
        //                        DateAdded = Convert.ToDateTime(reader["DateAdded"]),
        //                        AnalysisType = reader["AnalysisType"].ToString()
        //                    };
        //                    SearchedDocument.Add(knowledge);
        //                }
        //            }
        //        }
        //    }
        //    //return (IActionResult)SearchedDocument;
        //    CollabFusionDBConnection.Close();



        //    string sqlQuery2 = "INSERT INTO PublicDocument (FileName, FileData, DateAdded, AnalysisType) VALUES (@FileName, @FileData, @DateAdded, @AnalysisType)";

        //    using (SqlCommand cmdDocInsert = new SqlCommand(sqlQuery2, CollabFusionDBConnection))
        //    {
        //        cmdDocInsert.Parameters.AddWithValue("@FileName", FileName);
        //        cmdDocInsert.Parameters.AddWithValue("@FileData", FileData);
        //        cmdDocInsert.Parameters.AddWithValue("@DateAdded", DateAdded);
        //        cmdDocInsert.Parameters.AddWithValue("@AnalysisType", AnalysisType);

        //        //CollabFusionDBConnection.Open();
        //        //cmdDocInsert.ExecuteNonQuery();
        //        //CollabFusionDBConnection.Close();

        //       // SqlCommand cmdRead = new SqlCommand();
        //        cmdDocInsert.Connection = CollabFusionDBConnection;
        //        cmdDocInsert.Connection.ConnectionString = CollabFusionDBConnString;
        //        cmdDocInsert.CommandText = sqlQuery2;
        //        cmdDocInsert.Connection.Open();

        //        cmdDocInsert.ExecuteNonQuery();
        //    }

        //    return Page();

        //}


        public IActionResult OnPostPush(int Id)
        {
            List<PublicDocument> SearchedDocument = new List<PublicDocument>();

            // Step 1: Read data from the source table and close the SqlDataReader
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Document WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PublicDocument knowledge = new PublicDocument
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FileName = reader["FileName"].ToString(),
                                FileData = (byte[])reader["FileData"],
                                DateAdded = Convert.ToDateTime(reader["DateAdded"]),
                                AnalysisType = reader["AnalysisType"].ToString()
                            };
                            SearchedDocument.Add(knowledge);
                        }
                    }
                }
            } // SqlConnection automatically closed here

            // Step 2: Open a new SqlConnection and insert data into the destination table
            using (SqlConnection insertConnection = new SqlConnection(CollabFusionDBConnString))
            {
                insertConnection.Open();
                foreach (var item in SearchedDocument)
                {
                    string insertQuery = "INSERT INTO PublicDocument (FileName, FileData, DateAdded, AnalysisType) " +
                                         "VALUES (@FileName, @FileData, @DateAdded, @AnalysisType)";
                    using (SqlCommand insertCommand = new SqlCommand(insertQuery, insertConnection))
                    {
                        insertCommand.Parameters.AddWithValue("@FileName", item.FileName);
                        insertCommand.Parameters.AddWithValue("@FileData", item.FileData);
                        insertCommand.Parameters.AddWithValue("@DateAdded", item.DateAdded);
                        insertCommand.Parameters.AddWithValue("@AnalysisType", item.AnalysisType);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            } // SqlConnection automatically closed here

            return RedirectToPage();
        }



    }
}