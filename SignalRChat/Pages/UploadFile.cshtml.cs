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
                    return RedirectToPage("/EnteredCollaboration", new { collaborationid = HttpContext.Session.GetInt32("collabid") });
                }
            }
            return Page();
        }
    }
}
