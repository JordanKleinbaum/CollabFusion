using InventoryManagement.Pages.DB;
using SignalRChat.Pages.DataClasses;
using System.Data.SqlClient;


namespace SignalRChat.Pages.DB

{
    public class FileClass
    {
        // Use this class to define methods that make connecting to
        // and retrieving data from the CollabFusion DB easier.

        // Connection Object at Data Field Level
        public static SqlConnection CollabFusionDBConnection = new SqlConnection();

        // Connection String - How to find and connect to DB
        private static readonly string CollabFusionDBConnString =
            "Server=localhost;Database=Lab3;Trusted_Connection=True;";

        public static SqlConnection AuthDBConnection = new SqlConnection();

        private static readonly String? AuthConnString = "Server=Localhost;Database=FileUpload ;Trusted_Connection=True";


       

    }
}
