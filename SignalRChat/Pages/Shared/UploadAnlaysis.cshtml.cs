using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRChat.Pages.DB;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection.Metadata;
using SignalRChat.Pages.DB;


namespace SignalRChat.Pages.Shared
{
    public class UploadAnlaysisModel : PageModel
    {
        private readonly string _connectionString = "Server=localhost;Database=FileUpload;Trusted_Connection=True;";

        [BindProperty]
        public IFormFile File { get; set; }
        [BindProperty]
        public string AnalysisType { get; set; } // New property to store the selected analysis type

        public async Task<IActionResult> OnPostAsync()
        {
            if (File != null && File.Length > 0)
            {
                // Read the uploaded file into a byte array
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await File.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // Store fileBytes in the database along with the date (without time) and analysis type
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO [Document] (FileData, DateAdded, AnalysisType) VALUES (@FileData, @DateAdded, @AnalysisType)";
                        command.Parameters.AddWithValue("@FileData", fileBytes);
                        command.Parameters.AddWithValue("@DateAdded", DateTime.Today); // Current date without time
                        command.Parameters.AddWithValue("@AnalysisType", AnalysisType); // Selected analysis type
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("/Index"); // Redirect to another page after successful upload
            }

            // If file not selected or uploaded, return to the same page
            return Page();
        }

    }

}
