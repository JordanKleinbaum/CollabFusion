using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DB;
using SignalRChat.Pages.DataClasses;

namespace SignalRChat.Pages
{
    public class UploadFileModel : PageModel
    {

        [BindProperty]
        public IFormFile File { get; set; }

        [BindProperty]
        public string AnalysisType { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (File != null && File.Length > 0)
            {
                // Get the file extension
                string extension = Path.GetExtension(File.FileName).ToLower();

                // Check if the file extension is .xlsx or .csv
                if (extension == ".xlsx" || extension == ".csv")
                {
                    // Redirect with error message
                    TempData["ErrorMessage"] = "Excel Files Cannot Be Uploaded Here. <br /><br />To Upload Excel Files...<br /> Go to \"Spending Levels and Projections\" Collaboration <br /> And then \"View Excel Data\" Button";
                    return RedirectToPage("/UploadFile");
                }

                using (var memoryStream = new MemoryStream())
                {
                    File.CopyTo(memoryStream);
                    var fileData = memoryStream.ToArray();

                    var document = new Document
                    {
                        FileName = Path.GetFileName(File.FileName),
                        FileData = fileData,
                        DateAdded = DateTime.Now,
                        AnalysisType = AnalysisType
                    };

                    DBClass.InsertDocument(document);
                    DBClass.CollabFusionDBConnection.Close();

                    // Set success message in TempData
                    TempData["UploadSuccessMessage"] = "File uploaded successfully.";

                    return RedirectToPage("/UploadFile");
                }
            }
            return Page();
        }
    }
}
