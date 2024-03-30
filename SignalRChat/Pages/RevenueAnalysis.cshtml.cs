using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml; // You might need to install EPPlus package for Excel handling
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;

namespace SignalRChat.Pages
{
    public class RevenueAnalysisModel : PageModel
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RevenueAnalysisModel(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // Chart Stuff
        public List<string> XAxisData { get; private set; }
        public List<int> YAxisData { get; private set; }
        public string XAxisTitle { get; private set; }
        public string YAxisTitle { get; private set; }

        // Insert into DataBase Stuff
        [BindProperty]
        [Required(ErrorMessage = "Analysis Title is required")]
        public string AnalysisName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Analysis Description is required")]
        public string AnalysisDescription { get; set; }

        public void OnGet(string fileName)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    // Assuming the first row contains headers
                    var xAxisTitle = worksheet.Cells["A1"].Value?.ToString();
                    var yAxisTitle = worksheet.Cells["B1"].Value?.ToString();

                    // Get data from columns A and B, starting from the second row
                    var xAxisData = worksheet.Cells["A2:A" + worksheet.Dimension.End.Row]
                                               .Select(cell => cell.Value?.ToString())
                                               .ToList();

                    var yAxisData = worksheet.Cells["B2:B" + worksheet.Dimension.End.Row]
                                               .Select(cell => int.Parse(cell.Value?.ToString() ?? "0")) // Assuming integers in column B
                                               .ToList();

                    // Pass the data to the Razor page
                    XAxisTitle = xAxisTitle;
                    YAxisTitle = yAxisTitle;
                    XAxisData = xAxisData;
                    YAxisData = yAxisData;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // If model state is not valid, return to the page
                return Page();
            }

            // Create a new PreviousSpendingAnalysis object
            var spendingAnalysis = new PreviousSpendingAnalysis
            {
                SpendingAnalysisName = AnalysisName,
                SpendingAnalysisDescription = AnalysisDescription
            };

            // Insert the collaboration into the database
            DBClass.InsertPreviousSpendingAnalysis(spendingAnalysis);

            // Redirect to the CollabHub page or any other page as needed
            return RedirectToPage("CollabHub");
        }


    }
}
