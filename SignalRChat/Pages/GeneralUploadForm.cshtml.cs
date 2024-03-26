using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;

namespace SignalRChat.Pages
{
    public class GeneralUploadModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }
        public bool UploadSuccessful { get; private set; }
        public string ErrorFileName { get; private set; }
        public List<string> UploadedFileNames { get; set; } = new List<string>(); // New property

        public IActionResult OnPost()
        {
            foreach (var formFile in FileList)
            {
                if (formFile.Length > 0)
                {
                    var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                    var filePath = Path.Combine(uploadDirectory, formFile.FileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }

                    UploadedFileNames.Add(formFile.FileName); // Add the file name to the list
                    UploadSuccessful = true;
                }
            }
            return Page();
        }
    }
}
