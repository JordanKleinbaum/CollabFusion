using InventoryManagement.Pages.DB;
using SignalRChat.Pages.DataClasses;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace SignalRChat.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the CollabFusion DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection CollabFusionDBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly string CollabFusionDBConnString = "Server=sharpmindsdb1.database.windows.net,1433;" + "Database=Lab3;" + "User Id=gwen;" + "Password=sharpminds1!;" + "Encrypt=True;" + "TrustServerCertificate=True";


        public static SqlConnection AuthDBConnection = new SqlConnection();

        private static readonly String? AuthConnString = "Server=sharpmindsdb1.database.windows.net,1433;" + "Database=AUTH;" + "User Id=gwen;" + "Password=sharpminds1!;" + "Encrypt=True;" + "TrustServerCertificate=True;";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DBClass(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Connection Methods:

        // Read all users
        public static SqlDataReader GetAllUsers()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Users";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetAllTableNames()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT TableName FROM DocumentTable";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetAllCollabs()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Collaboration";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        public static SqlDataReader GetAllDocumentTables()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM DocumentTable";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }



        public static SqlDataReader GetAllCollab_User()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Collab_User";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        public static Users UserInfoBasedOnID(IHttpContextAccessor httpContextAccessor)
        {
            int? userId = httpContextAccessor.HttpContext.Session.GetInt32("_userid");

            if (!userId.HasValue)
            {
                throw new Exception("User ID not found in session.");
            }

            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Users WHERE UserID = @userId";
            cmdRead.Parameters.AddWithValue("@userId", userId);

            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader reader = cmdRead.ExecuteReader();

            Users user = null;

            if (reader.Read())
            {
                user = new Users
                {
                    UserID = Convert.ToInt32(reader["UserID"]),
                    Username = reader["Username"].ToString(),
                    FirstName = reader["FirstName"].ToString(),
                    LastName = reader["LastName"].ToString(),
                    Email = reader["Email"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Street = reader["Street"].ToString(),
                    City = reader["City"].ToString(),
                    State = reader["State"].ToString(),
                    Country = reader["Country"].ToString(),
                    ZipCode = reader["ZipCode"].ToString(),
                    Admin = reader["Admin"].ToString()
                };
            }

            reader.Close();

            return user;
        }

        // Insert into Users table
        // Users table changes -> Deleted Password insert statement, and correspomding sqlQuery appending
        public static void InsertUser(Users u)
        {
            string sqlQuery = "INSERT INTO Users (Username, FirstName ,LastName, Email, Phone, Street, City, State, Country, ZipCode) Values ('";
            sqlQuery += u.Username + "','";

            sqlQuery += u.FirstName + "','";
            sqlQuery += u.LastName + "','";
            sqlQuery += u.Email + "','";
            sqlQuery += u.Phone + "','";
            sqlQuery += u.Street + "','";
            sqlQuery += u.City + "','";
            sqlQuery += u.State + "','";
            sqlQuery += u.Country + "','";
            sqlQuery += u.ZipCode + "')";


            SqlCommand cmdUserRead = new SqlCommand();
            cmdUserRead.Connection = CollabFusionDBConnection;
            cmdUserRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdUserRead.CommandText = sqlQuery;
            cmdUserRead.Connection.Open();
            cmdUserRead.ExecuteNonQuery();
            cmdUserRead.Connection.Close();
        }

        public static void UpdateUser(Users u, IHttpContextAccessor httpContextAccessor)
        {
            int? userId = httpContextAccessor.HttpContext.Session.GetInt32("_userid");

            if (!userId.HasValue)
            {
                throw new Exception("User ID not found in session.");
            }

            // Construct the SQL query with parameterized values to prevent SQL injection
            string sqlQuery = $"UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, Street = @Street, City = @City, State = @State, Country = @Country, ZipCode = @ZipCode WHERE UserID = {userId}";

            // Create a new SqlCommand
            using (SqlCommand cmdUserUpdate = new SqlCommand(sqlQuery, CollabFusionDBConnection))
            {
                // Set command parameters
                cmdUserUpdate.Parameters.AddWithValue("@FirstName", u.FirstName);
                cmdUserUpdate.Parameters.AddWithValue("@LastName", u.LastName);
                cmdUserUpdate.Parameters.AddWithValue("@Email", u.Email);
                cmdUserUpdate.Parameters.AddWithValue("@Phone", u.Phone);
                cmdUserUpdate.Parameters.AddWithValue("@Street", u.Street);
                cmdUserUpdate.Parameters.AddWithValue("@City", u.City);
                cmdUserUpdate.Parameters.AddWithValue("@State", u.State);
                cmdUserUpdate.Parameters.AddWithValue("@Country", u.Country);
                cmdUserUpdate.Parameters.AddWithValue("@ZipCode", u.ZipCode);

                // Set the connection string
                cmdUserUpdate.Connection.ConnectionString = CollabFusionDBConnString;

                // Open connection and execute the command
                cmdUserUpdate.Connection.Open();
                cmdUserUpdate.ExecuteNonQuery();
                cmdUserUpdate.Connection.Close();
            } // The using block ensures proper disposal of resources, including closing the connection
        }




        // Read all Knowledge Items
        public static SqlDataReader GetAllKnowledgeItems()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM KnowledgeItem";

            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open();
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        // Insert into KnowledgeItem table
        public static void InsertKnowledgeItem(KnowledgeItem k)
        {
            string sqlQuery = "INSERT INTO KnowledgeItem (KnowledgeTitle, KnowledgeSubject, Category, Information, KNDate, UserID, CollabID) Values ('";
            sqlQuery += k.KnowledgeTitle + "','";
            sqlQuery += k.KnowledgeSubject + "','";
            sqlQuery += k.Category + "','";
            sqlQuery += k.Information + "','";
            sqlQuery += k.KNDate + "','";
            sqlQuery += k.UserID + "','";
            sqlQuery += k.CollabID + "')";
            //sqlQuery += k.KnowledgeLibID + "')";

            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdRead.CommandText = sqlQuery;
            cmdRead.Connection.Open();

            cmdRead.ExecuteNonQuery();
            cmdRead.Connection.Close();

        }

        public static SqlDataReader GetSingleKnowledgeItem(int KnowledgeID)
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.Connection.ConnectionString =
                CollabFusionDBConnString;
            cmdRead.CommandText = "SELECT * FROM KnowledgeItem WHERE KnowledgeID = " + KnowledgeID;
            cmdRead.Connection.Open();

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        // Read all Plans
        public static SqlDataReader GetAllPlans()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Plans";

            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open();
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;

        }

        // Insert into Plans table
        public static void InsertPlan(Plans p)
        {
            string sqlQuery = "INSERT INTO Plans (PlanName, PlanContents, DateCreated, CollabID) Values ('";
            sqlQuery += p.PlanName + "','";
            sqlQuery += p.PlanContents + "','";
            sqlQuery += p.DateCreated + "','";
            sqlQuery += p.CollabID + "')";

            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = CollabFusionDBConnection;
            cmdPlanRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdPlanRead.CommandText = sqlQuery;
            cmdPlanRead.Connection.Open();

            cmdPlanRead.ExecuteNonQuery();

        }

        public static SqlDataReader GetSinglePlan(int PlanID)
        {
            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = CollabFusionDBConnection;
            cmdPlanRead.Connection.ConnectionString =
                CollabFusionDBConnString;
            cmdPlanRead.CommandText = "SELECT * FROM Plans WHERE PlanID = " + PlanID;
            cmdPlanRead.Connection.Open();

            SqlDataReader tempReader = cmdPlanRead.ExecuteReader();

            return tempReader;

        }

        // CHAT
        public static SqlDataReader GetSingleChat(int ChatID)
        {
            SqlCommand cmdChatRead = new SqlCommand();
            cmdChatRead.Connection = CollabFusionDBConnection;
            cmdChatRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdChatRead.CommandText = "SELECT * FROM Chat WHERE ChatID = " + ChatID;
            cmdChatRead.Connection.Open();

            SqlDataReader tempReader = cmdChatRead.ExecuteReader();
            return tempReader;
        }

        // Read all PlanSteps
        public static SqlDataReader GetAllPlanSteps()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM PlanStep";

            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open();
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;

        }

        // Read all PlanSteps
        public static SqlDataReader GetAllMatchingPlanSteps()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = $"SELECT * FROM PlanStep AS ps INNER JOIN Plans AS p ON ps.PlanID = p.PlanID";

            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open();
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;

        }

        // Insert into Steps table
        public static void InsertPlanStep(PlanStep s)
        {
            string sqlQuery = "INSERT INTO PlanStep (StepTitle, StepDescription, PlanID) Values ('";
            sqlQuery += s.StepTitle + "','";
            sqlQuery += s.StepDescription + "','";
            sqlQuery += s.PlanID + "')";


            SqlCommand cmdPlanRead = new SqlCommand();
            cmdPlanRead.Connection = CollabFusionDBConnection;
            cmdPlanRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdPlanRead.CommandText = sqlQuery;
            cmdPlanRead.Connection.Open();

            cmdPlanRead.ExecuteNonQuery();

        }

        public static SqlDataReader GeneralReaderQuery(string sqlQuery)
        {
            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = CollabFusionDBConnection;
            cmdProductRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdProductRead.CommandText = sqlQuery;
            cmdProductRead.Connection.Open();
            SqlDataReader tempReader = cmdProductRead.ExecuteReader();
            return tempReader;
        }

        // LOGIN LOGIC
        public static int LoginQuery(string loginQuery)
        {
            // This method expects to receive an SQL SELECT
            // query that uses the COUNT command.

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = CollabFusionDBConnection;
            cmdLogin.Connection.ConnectionString = CollabFusionDBConnString;
            cmdLogin.CommandText = loginQuery;
            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            int rowCount = (int)cmdLogin.ExecuteScalar();

            return rowCount;
        }

        public static bool StoredProcedureLogin(string Username, string Password)
        {
            SqlCommand cmdLoginRead = new SqlCommand();
            cmdLoginRead.Connection = CollabFusionDBConnection;
            cmdLoginRead.Connection.ConnectionString = CollabFusionDBConnString;
            cmdLoginRead.CommandType = System.Data.CommandType.StoredProcedure;
            cmdLoginRead.Parameters.AddWithValue("@Username", Username);
            cmdLoginRead.Parameters.AddWithValue("@Password", Password);
            cmdLoginRead.CommandText = "sp_simpleLogin";
            cmdLoginRead.Connection.Open();
            if (((int)cmdLoginRead.ExecuteScalar()) > 0)
            {
                return true;
            }
            return false;
        }

        public static string GetFirstNameByUsername(string username)
        {
            string firstName = "";
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                string sqlQuery = "SELECT FirstName FROM Users WHERE Username = @Username";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        firstName = reader["FirstName"].ToString();
                    }
                }
            }
            return firstName;
        }

        public static int GetIdByUsername(string username)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                string sqlQuery = "SELECT UserID FROM Users WHERE Username = @Username";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        id = Convert.ToInt32(reader["UserID"]);
                    }
                }
            }
            return id;
        }

        public static string GetAdminByUsername(string username)
        {
            string adminYesOrNo = "";
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                string sqlQuery = "SELECT Admin FROM Users WHERE Username = @Username";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@Username", username);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        adminYesOrNo = reader["Admin"].ToString();
                    }
                }
            }
            return adminYesOrNo;
        }







        // Insert Into Chat Table
        public static void InsertChat(Chat u)
        {
            string sqlQuery = "INSERT INTO Chat (UserID, ChatText, TimeStamp, CollabID) VALUES (@UserID, @ChatText, @TimeStamp, @CollabID)";

            using (SqlCommand cmdChatInsert = new SqlCommand(sqlQuery, CollabFusionDBConnection))
            {
                cmdChatInsert.Parameters.AddWithValue("@UserID", u.UserID);
                cmdChatInsert.Parameters.AddWithValue("@ChatText", u.ChatText);
                cmdChatInsert.Parameters.AddWithValue("@TimeStamp", DateTime.Now);
                cmdChatInsert.Parameters.AddWithValue("@CollabID", u.CollabID);

                CollabFusionDBConnection.Open();
                cmdChatInsert.ExecuteNonQuery();
            }
        }

        // Get all Chat (aka read from DB, all the Chats)
        public static SqlDataReader GetAllChats()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM Chat";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }

        public static void InsertCollaborations(List<Collaboration> collaborations)
        {
            try
            {
                foreach (var collab in collaborations)
                {
                    InsertCollaboration(collab);
                }
            }
            catch (Exception ex)
            {
                // Handle exception as per your application's requirements
                Console.WriteLine($"Error inserting collaborations: {ex.Message}");
            }
        }

        public static void InsertCollaboration(Collaboration collab)
        {
            try
            {
                string sqlQuery = "INSERT INTO Collaboration (CollaborationName, NotesAndInformation) VALUES (@CollaborationName, @NotesAndInformation)";

                using (SqlCommand cmdCollabInsert = new SqlCommand(sqlQuery, CollabFusionDBConnection))
                {
                    // Ensure the connection string is assigned
                    if (CollabFusionDBConnection.ConnectionString == null || CollabFusionDBConnection.ConnectionString == "")
                    {
                        CollabFusionDBConnection.ConnectionString = CollabFusionDBConnString;
                    }

                    // Check if the connection is already open
                    if (CollabFusionDBConnection.State != System.Data.ConnectionState.Open)
                    {
                        CollabFusionDBConnection.Open();
                    }

                    cmdCollabInsert.Parameters.AddWithValue("@CollaborationName", collab.CollaborationName);
                    cmdCollabInsert.Parameters.AddWithValue("@NotesAndInformation", collab.NotesAndInformation);

                    cmdCollabInsert.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw; // Re-throw the exception after it's been handled or logged
            }
            finally
            {
                // Close the connection if it's open
                if (CollabFusionDBConnection.State == System.Data.ConnectionState.Open)
                {
                    CollabFusionDBConnection.Close();
                }
            }
        }


        public static List<Collaboration> GetAllCollaborations()
        {
            List<Collaboration> collaborations = new List<Collaboration>();

            try
            {
                using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
                {
                    string query = "SELECT * FROM Collaboration";
                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Collaboration collab = new Collaboration
                        {
                            CollabID = Convert.ToInt32(reader["CollabID"]),
                            CollaborationName = Convert.ToString(reader["CollaborationName"]),
                            NotesAndInformation = Convert.ToString(reader["NotesAndInformation"])
                        };

                        collaborations.Add(collab);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving collaborations: {ex.Message}");
            }

            return collaborations;
        }


        // HASHED PASSWORD METHOD #1 | Turned this into a Stored Procedure
        public static bool sp_Lab3Login(string Username, string Password)
        {
            string login = "EXEC sp_Lab3Login @Username";

            SqlCommand cmdProductRead = new SqlCommand();
            cmdProductRead.Connection = AuthDBConnection;
            cmdProductRead.Connection.ConnectionString = AuthConnString;

            //cmdProductRead.CommandType = System.Data.CommandType.StoredProcedure;
            cmdProductRead.CommandText = login;

            cmdProductRead.Parameters.AddWithValue("@Username", Username);
            //cmdProductRead.Parameters.AddWithValue("@Password", Password);
            cmdProductRead.Connection.Open();

            SqlDataReader hashReader = cmdProductRead.ExecuteReader();
            if (hashReader.Read())
            {
                string correctHash = hashReader["Password"].ToString();

                if (PasswordHash.ValidatePassword(Password, correctHash))
                {
                    return true;
                }
            }

            return false;
        }

        // HASHED PASSWORD METHOD #2
        public static void CreateHashedUser(string Username, string Password)
        {
            string loginQuery =
                "INSERT INTO HashedCredentials (Username,Password) values (@Username, @Password)";

            SqlCommand cmdLogin = new SqlCommand();
            cmdLogin.Connection = AuthDBConnection;
            cmdLogin.Connection.ConnectionString = AuthConnString;

            cmdLogin.CommandText = loginQuery;
            cmdLogin.Parameters.AddWithValue("@Username", Username);
            cmdLogin.Parameters.AddWithValue("@Password", PasswordHash.HashPassword(Password));

            cmdLogin.Connection.Open();

            // ExecuteScalar() returns back data type Object
            // Use a typecast to convert this to an int.
            // Method returns first column of first row.
            cmdLogin.ExecuteNonQuery();

        }

        // PARAMETERIZED QUERIES (PQ)

        public static void CreateUserAndInsertCollabUserBridgeTable(Users newUser, List<int> collabIds)
        {
            string insertUserQuery = "INSERT INTO Users (Username, FirstName, LastName, Email, Phone, Street, City, State, Country, ZipCode, Admin) " +
                                     "VALUES (@Username, @FirstName, @LastName, @Email, @Phone, @Street, @City, @State, @Country, @ZipCode, @Admin);" +
                                     "SELECT SCOPE_IDENTITY();"; // This retrieves the automatically generated UserID

            string insertBridgeQuery = "INSERT INTO Collab_User (CollabID, UserID) VALUES (@CollabID, @UserID)";

            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Insert new user and retrieve the generated UserID
                    using (SqlCommand cmdInsertUser = new SqlCommand(insertUserQuery, connection, transaction))
                    {
                        cmdInsertUser.Parameters.AddWithValue("@Username", newUser.Username);
                        cmdInsertUser.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                        cmdInsertUser.Parameters.AddWithValue("@LastName", newUser.LastName);
                        cmdInsertUser.Parameters.AddWithValue("@Email", newUser.Email);
                        cmdInsertUser.Parameters.AddWithValue("@Phone", newUser.Phone);
                        cmdInsertUser.Parameters.AddWithValue("@Street", newUser.Street);
                        cmdInsertUser.Parameters.AddWithValue("@City", newUser.City);
                        cmdInsertUser.Parameters.AddWithValue("@State", newUser.State);
                        cmdInsertUser.Parameters.AddWithValue("@Country", newUser.Country);
                        cmdInsertUser.Parameters.AddWithValue("@ZipCode", newUser.ZipCode);
                        cmdInsertUser.Parameters.AddWithValue("@Admin", newUser.Admin);

                        // Execute the user insert command and retrieve the generated UserID
                        newUser.UserID = Convert.ToInt32(cmdInsertUser.ExecuteScalar());
                    }

                    // Insert records into Collab_User bridge table using the retrieved UserID
                    foreach (int collabId in collabIds)
                    {
                        using (SqlCommand cmdInsertBridge = new SqlCommand(insertBridgeQuery, connection, transaction))
                        {
                            cmdInsertBridge.Parameters.AddWithValue("@CollabID", collabId);
                            cmdInsertBridge.Parameters.AddWithValue("@UserID", newUser.UserID);
                            cmdInsertBridge.ExecuteNonQuery();
                        }
                    }

                    // Commit the transaction
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if an error occurs
                    transaction.Rollback();
                    throw new Exception("Error creating user and inserting bridge table records.", ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }





        public static List<Document> SearchKnowledge(string SearchTerm)
        {
            List<Document> SearchedDocument = new List<Document>();
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                connection.Open();
                string sqlQuery = "SELECT * FROM Document WHERE FileName LIKE @SearchTerm";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + SearchTerm + "%");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Document knowledge = new Document
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
            }
            return SearchedDocument;
        }
        public static List<PublicDocument> SearchPublicKnowledge(string SearchTerm)
        {
            List<PublicDocument> SearchedDocument = new List<PublicDocument>();
            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                connection.Open();
                string sqlQuery = "SELECT * FROM PublicDocument WHERE FileName LIKE @SearchTerm";
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@SearchTerm", "%" + SearchTerm + "%");
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
            }
            return SearchedDocument;
        }


        public static void InsertDocument(Document d)
        {
            string documentTableSelectStatement = $"SELECT DocumentTableID FROM DocumentTable WHERE TableName = @AnalysisType"; // Use parameterized query for safety

            // Construct the SQL query with proper parameter placeholders
            string sqlQuery = "INSERT INTO Document (FileName, FileData, DateAdded, AnalysisType, DocumentTableID) " +
                              $"VALUES (@FileName, @FileData, @DateAdded, @AnalysisType, ({documentTableSelectStatement}))";

            using (SqlCommand cmdDocInsert = new SqlCommand(sqlQuery, CollabFusionDBConnection))
            {
                // Set the parameter values
                cmdDocInsert.Parameters.AddWithValue("@FileName", d.FileName);
                cmdDocInsert.Parameters.AddWithValue("@FileData", d.FileData);
                cmdDocInsert.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                cmdDocInsert.Parameters.AddWithValue("@AnalysisType", d.AnalysisType);

                // Open the database connection
                CollabFusionDBConnection.Open();

                // Execute the query
                // First, execute the SELECT statement to get DocumentTableID
                object documentTableId = cmdDocInsert.ExecuteScalar(); // ExecuteScalar to retrieve a single value

                // Check if DocumentTableID is retrieved successfully
                if (documentTableId != null && documentTableId != DBNull.Value)
                {
                    // Set the retrieved DocumentTableID as a parameter for the INSERT query
                    cmdDocInsert.Parameters.AddWithValue("@DocumentTableID", documentTableId);

                    // Now execute the INSERT statement with all parameters set
                    cmdDocInsert.ExecuteNonQuery();
                }
                else
                {
                    // Handle the case where DocumentTableID is not found (optional)
                    Console.WriteLine("DocumentTableID not found for the specified AnalysisType.");
                    // You can throw an exception or handle this scenario based on your requirements
                }

                // Close the database connection
                CollabFusionDBConnection.Close();
            }
        }


        // Insert into TableDocument table
        public static void InsertTableDocument(DocumentTable t)
        {
            string sqlQuery = $"INSERT INTO DocumentTable (CollabID, TableName) VALUES ({t.CollabID}, '{t.TableName}')";

            using (SqlConnection connection = new SqlConnection(CollabFusionDBConnString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.ExecuteNonQuery();
            } // SqlConnection is automatically closed and disposed after exiting the using block
        }




        public static void InsertPublicDocument(PublicDocument d)
        {
            string sqlQuery = "INSERT INTO PublicDocument (FileName, FileData, DateAdded, AnalysisType) VALUES (@FileName, @FileData, @DateAdded, @AnalysisType)";

            using (SqlCommand cmdDocInsert = new SqlCommand(sqlQuery, CollabFusionDBConnection))
            {
                cmdDocInsert.Parameters.AddWithValue("@FileName", d.FileName);
                cmdDocInsert.Parameters.AddWithValue("@FileData", d.FileData);
                cmdDocInsert.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                cmdDocInsert.Parameters.AddWithValue("@AnalysisType", d.AnalysisType);

                CollabFusionDBConnection.Open();
                cmdDocInsert.ExecuteNonQuery();
                CollabFusionDBConnection.Close();
            }
        }

        public static void InsertPreviousSpendingAnalysis(PreviousSpendingAnalysis spendinganalysis)
        {
            string sqlQuery = "INSERT INTO PreviousSpendingAnalysis (SpendingAnalysisName, SpendingAnalysisDescription, BasedOffOf, SpendingAnalysisDate) VALUES (@SpendingAnalysisName, @SpendingAnalysisDescription, @BasedOffOf, @SpendingAnalysisDate)";

            using (SqlCommand cmdDocInsert = new SqlCommand(sqlQuery, CollabFusionDBConnection))
            {
                cmdDocInsert.Parameters.AddWithValue("@SpendingAnalysisName", spendinganalysis.SpendingAnalysisName);
                cmdDocInsert.Parameters.AddWithValue("@SpendingAnalysisDescription", spendinganalysis.SpendingAnalysisDescription);
                cmdDocInsert.Parameters.AddWithValue("@BasedOffOf", spendinganalysis.BasedOffOf);
                cmdDocInsert.Parameters.AddWithValue("@SpendingAnalysisDate", DateTime.Now);

                CollabFusionDBConnection.Open();
                cmdDocInsert.ExecuteNonQuery();
                CollabFusionDBConnection.Close();

            }

            //string anotherSqlQuery = "INSERT INTO Document (FileName, FileData, DateAdded, AnalysisType) VALUES ('Regression Analysis For: " + spendinganalysis.BasedOffOf + "', CONVERT(varbinary(max), '0x50'), @SpendingAnalysisDate, 'Regression Analysis')";
            //using (SqlCommand cmdDocInsert = new SqlCommand(anotherSqlQuery, CollabFusionDBConnection))
            //{
            //    cmdDocInsert.Parameters.AddWithValue("@BasedOffOf", spendinganalysis.BasedOffOf);
            //    cmdDocInsert.Parameters.AddWithValue("@SpendingAnalysisDate", DateTime.Now);

            //    CollabFusionDBConnection.Open();
            //    cmdDocInsert.ExecuteNonQuery();
            //    CollabFusionDBConnection.Close();
            //}
        }

        public static SqlDataReader GetAllPreviousSpendingAnalysis()
        {
            SqlCommand cmdRead = new SqlCommand();
            cmdRead.Connection = CollabFusionDBConnection;
            cmdRead.CommandText = "SELECT * FROM PreviousSpendingAnalysis";
            if (cmdRead.Connection.State != System.Data.ConnectionState.Open)
            {
                cmdRead.Connection.ConnectionString = CollabFusionDBConnString;
                cmdRead.Connection.Open(); // Open connection here, close in calling method
            }

            SqlDataReader tempReader = cmdRead.ExecuteReader();

            return tempReader;
        }



    }
}