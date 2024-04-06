using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class HelpButtonModel : PageModel
    {
        public List<Document> Doc { get; set; } = new List<Document>();
        public string Admin { get; set; }
        public string UserFirstName { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                var username = HttpContext.Session.GetString("username");
                UserFirstName = DBClass.GetFirstNameByUsername(username);
                Admin = DBClass.GetAdminByUsername(username);
                HttpContext.Session.SetString("_Admin", Admin);
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
    }
}
