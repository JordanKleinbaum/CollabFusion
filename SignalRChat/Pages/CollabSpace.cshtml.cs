using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;
using System.ComponentModel.DataAnnotations;

namespace SignalRChat.Pages
{
    public class CollabSpaceModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Collaboration Name is required")]
        public string CollaborationName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Collaboration Notes are required")]
        public string CollaborationNotes { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
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

            // Create a new Collaboration object
            var collaboration = new Collaboration
            {
                CollaborationName = CollaborationName,
                NotesAndInformation = CollaborationNotes
            };

            // Insert the collaboration into the database
            DBClass.InsertCollaboration(collaboration);

            // Redirect to the CollabHub page or any other page as needed
            return RedirectToPage("CollabHub");
        }

        /*POPULATE BUTTON*/
        public IActionResult OnPostPopulateButton()
        {
            if (!ModelState.IsValid)
            {
                ModelState.Clear(); // Causes Model Validation to be skipped and reset for next entry
                CollaborationName = "Team Project 1";
                CollaborationNotes = "We will finish this project ultra ultra quickly!";
            }
            return Page();
        }

        /*CLEAR BUTTON*/
        public IActionResult OnPostClearButton()
        {
            if (HttpContext.Request.Method == "POST")
            {
                ModelState.Clear(); // Causes Model Validation to be skipped and reset for next entry
                CollaborationName = "";
                CollaborationNotes = "";
            }
            return Page();
        }
    }
}