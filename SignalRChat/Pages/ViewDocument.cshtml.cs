using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DB;
using SignalRChat.Pages.DataClasses;
using System.Threading.Tasks; // Add this namespace for Task

namespace SignalRChat.Pages
{
    public class ViewDocumentModel : PageModel
    {
        private readonly DBClass _db;

        public ViewDocumentModel(DBClass db)
        {
            _db = db;
        }

        public Document Document { get; private set; }

        public async Task<IActionResult> OnGetAsync(int id) // Note the async here
        {
            Document = await _db.GetSingleFileAsync(id); // Await the async method

            if (Document == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
