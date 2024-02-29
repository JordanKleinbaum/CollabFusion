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

                SqlDataReader knowledgeReader = DBClass.GetAllKnowledgeItems();
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