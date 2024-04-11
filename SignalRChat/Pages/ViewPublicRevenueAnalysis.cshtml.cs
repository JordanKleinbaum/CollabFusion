using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using MathNet.Numerics;
using System.ComponentModel.DataAnnotations;
using SignalRChat.Pages.DataClasses;
using SignalRChat.Pages.DB;

namespace SignalRChat.Pages
{
    public class ViewPublicRevenueAnalysisModel : PageModel
    {
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        public RegressionAnalysisResult RegressionAnalysisResult { get; set; }
        public List<double> XValues { get; set; }
        public List<double> YValues { get; set; }
        public List<double> Trendline { get; set; }
        public string XColumnHeader { get; set; }
        public string YColumnHeader { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Analysis Title is required")]
        public string AnalysisName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Analysis Description is required")]
        public string AnalysisDescription { get; set; }

        [BindProperty]
        public int column1 { get; set; }
        [BindProperty]
        public int column2 { get; set; }

        private bool ColumnsSelected { get; set; } = false;

        public IActionResult OnGet(string fileName, int column1, int column2)
        {
            if (string.IsNullOrEmpty(fileName))
                return RedirectToPage("/CollabHub");

            var filePath = Path.Combine(_uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return RedirectToPage("/CollabHub");

            // Don't perform analysis if columns are not selected
            if (column1 != null && column2 != null)
            {
                ColumnsSelected = true;
                PerformAnalysis(filePath, column1, column2);
            }

            return Page();
        }

        private void PerformAnalysis(string filePath, int column1, int column2)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                return;

            var rowCount = worksheet.Dimension.Rows;

            var xData = new List<double>();
            var yData = new List<double>();

            // Fetch data based on selected columns
            XColumnHeader = worksheet.Cells[1, column1].GetValue<string>();
            YColumnHeader = worksheet.Cells[1, column2].GetValue<string>();

            for (int row = 2; row <= rowCount; row++)
            {
                var xValue = worksheet.Cells[row, column1].GetValue<double>();
                var yValue = worksheet.Cells[row, column2].GetValue<double>();

                xData.Add(xValue);
                yData.Add(yValue);
            }

            XValues = xData;
            YValues = yData;

            // Perform linear regression only if columns are selected
            if (ColumnsSelected)
            {
                // Perform linear regression
                var regression = Fit.Line(XValues.ToArray(), YValues.ToArray());
                RegressionAnalysisResult = new RegressionAnalysisResult
                {
                    Intercept = regression.Item1,
                    Slope = regression.Item2,
                    RSquared = GoodnessOfFit.RSquared(YValues, XValues.Select(x => regression.Item1 + regression.Item2 * x))
                };

                // Generate trendline
                Trendline = XValues.Select(x => RegressionAnalysisResult.Intercept + RegressionAnalysisResult.Slope * x).ToList();
            }
        }

        public IActionResult OnPost(string fileName)
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
                SpendingAnalysisDescription = AnalysisDescription,
                BasedOffOf = fileName,
                Column1 = column1,
                Column2 = column2
            };

            // Insert the collaboration into the database
            DBClass.InsertPreviousSpendingAnalysis(spendingAnalysis);

            return RedirectToPage("CollabHub");
        }
    }
}
