using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Pages
{
    public class CollabHubModel : PageModel
    {
        public string UserFirstName { get; set; }
        public List<Collaboration> Collaborations { get; set; }

        [BindProperty]
        public Collaboration NewCollaboration { get; set; }
        
        [BindProperty]
        public int CollabID { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                var username = HttpContext.Session.GetString("username");
                UserFirstName = DBClass.GetFirstNameByUsername(username);
                Collaborations = DBClass.GetAllCollaborations(); // Retrieve all collaborations from the database
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
            if (!ModelState.IsValid)
            {
                // If model state is not valid, return to the page
                return Page();
            }

            // Insert the new collaboration into the database
            DBClass.InsertCollaboration(NewCollaboration);

            // Redirect to the CollabHub page to display the updated list of collaborations
            return RedirectToPage("CollabHub");
        }
    }
}
