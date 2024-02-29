using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class AllChatsModel : PageModel
    {
        public List<Users> UsersList { get; set; } = new List<Users>();
        public List<Chat> ChatList { get; set; } = new List<Chat>();

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

                //Close your connection in DBClass
                DBClass.CollabFusionDBConnection.Close();


                //_____________________________________


                // Populate ChatList with user data
                SqlDataReader chatReader = DBClass.GetAllChats();
                while (chatReader.Read())
                {
                    ChatList.Add(new Chat
                    {
                        ChatID = Convert.ToInt32(chatReader["ChatID"]),
                        UserID = Convert.ToInt32(chatReader["UserID"]),
                        ChatText = chatReader["ChatText"].ToString(),
                        TimeStamp = chatReader["TimeStamp"] as DateTime?,
                    });
                }
                // Close the chatReader and the DataBase Connection
                chatReader.Close();
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
