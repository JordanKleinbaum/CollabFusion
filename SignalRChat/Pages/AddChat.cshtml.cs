using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace SignalRChat.Pages
{
    public class AddChatModel : PageModel
    {
        [BindProperty]
        [Required]
        public Chat NewChat { get; set; }

        public List<Chat> NewChatList { get; set; } = new List<Chat>();

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

                SqlDataReader chatReader = DBClass.GetAllChats();
                while (chatReader.Read())
                {
                    NewChatList.Add(new Chat
                    {
                        ChatID = Convert.ToInt32(chatReader["ChatID"]),
                        UserID = Convert.ToInt32(chatReader["UserID"]),
                        ChatText = chatReader["ChatText"].ToString(),
                        TimeStamp = chatReader["TimeStamp"] as DateTime?,
                        CollabID = HttpContext.Session.GetInt32("collabid")
                    });
                }

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

        public IActionResult OnPost()
        {
            // Populate UsersList with user data from the database
            UsersList.Clear(); // Clear the list to avoid duplicate entries
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

            var currentUsername = HttpContext.Session.GetString("username");

            var user = UsersList.FirstOrDefault(u => u.Username == currentUsername);
            NewChat.UserID = user.UserID;

            NewChat.CollabID = HttpContext.Session.GetInt32("collabid");

            DBClass.InsertChat(NewChat);

            DBClass.CollabFusionDBConnection.Close();

            return RedirectToPage("AddChat");
        }
    }
}
