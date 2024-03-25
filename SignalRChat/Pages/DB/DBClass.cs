using InventoryManagement.Pages.DB;
using SignalRChat.Pages.DataClasses;
using System.Data.SqlClient;

namespace SignalRChat.Pages.DB
{
    public class DBClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the CollabFusion DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection CollabFusionDBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly string CollabFusionDBConnString =
            "Server=localhost;Database=Lab3;Trusted_Connection=True;";

        public static SqlConnection AuthDBConnection = new SqlConnection();

        private static readonly String? AuthConnString = "Server=Localhost;Database=AUTH;Trusted_Connection=True"; // Added this for AUTH DB


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
            cmdProductRead.Connection.ConnectionString =
            CollabFusionDBConnString;
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

        // PQ -> USER.CSHTML.CS
        public static void ParameterizedCreateUser(Users newUser)
        {
            string insertQuery = "INSERT INTO Users (Username, FirstName, LastName, Email, Phone, Street, City, State, Country, ZipCode, Admin) VALUES (@Username, @FirstName, @LastName, @Email, @Phone, @Street, @City, @State, @Country, @ZipCode, @Admin)";
            SqlCommand cmdInsert = new SqlCommand();
            cmdInsert.Connection = CollabFusionDBConnection;
            cmdInsert.Connection.ConnectionString = CollabFusionDBConnString;

            cmdInsert.CommandText = insertQuery;
            cmdInsert.Parameters.AddWithValue("@Username", newUser.Username);
            cmdInsert.Parameters.AddWithValue("@FirstName", newUser.FirstName);
            cmdInsert.Parameters.AddWithValue("@LastName", newUser.LastName);
            cmdInsert.Parameters.AddWithValue("@Email", newUser.Email);
            cmdInsert.Parameters.AddWithValue("@Phone", newUser.Phone);
            cmdInsert.Parameters.AddWithValue("@Street", newUser.Street);
            cmdInsert.Parameters.AddWithValue("@City", newUser.City);
            cmdInsert.Parameters.AddWithValue("@State", newUser.State);
            cmdInsert.Parameters.AddWithValue("@Country", newUser.Country);
            cmdInsert.Parameters.AddWithValue("@ZipCode", newUser.ZipCode);
            cmdInsert.Parameters.AddWithValue("@Admin", newUser.Admin);

            cmdInsert.Connection.Open();
            cmdInsert.ExecuteNonQuery();
            cmdInsert.Connection.Close();
        }

    }
}