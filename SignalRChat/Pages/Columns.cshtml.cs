using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalRChat.Pages
{
    public class ColumnsModel : PageModel
    {
        public int column1 { get; set; }
        public int column2 { get; set; }
        public string FileName { get; set; }

        public void OnGet(string fileName)
        {
            FileName = fileName;
        }


        public IActionResult OnPost(int column1, int column2, string fileName)
        {
            // Validate and process the column values
            // You can also perform additional validation if needed

            // Redirect to another page passing the required parameters
            return RedirectToPage("/RevenueAnalysis", new { fileName, column1, column2 });
        }
    }
}
