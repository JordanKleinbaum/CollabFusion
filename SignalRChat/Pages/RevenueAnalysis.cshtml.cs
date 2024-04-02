using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using MathNet.Numerics;

namespace SignalRChat.Pages
{
    public class RegressionAnalysisResult
    {
        public double Intercept { get; set; }
        public double Slope { get; set; }
        public double RSquared { get; set; }
    }

    public class RevenueAnalysisModel : PageModel
    {
        private readonly string _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        public RegressionAnalysisResult RegressionAnalysisResult { get; set; }
        public List<double> XValues { get; set; }
        public List<double> YValues { get; set; }
        public List<double> Trendline { get; set; }

        public async Task<IActionResult> OnGetAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return RedirectToPage("/CollabHub");

            var filePath = Path.Combine(_uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return RedirectToPage("/CollabHub");

            await PerformAnalysisAsync(filePath);

            return Page();
        }

        private async Task PerformAnalysisAsync(string filePath)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                return;

            var rowCount = worksheet.Dimension.Rows;
            var colCount = worksheet.Dimension.Columns;

            var xData = new List<double>();
            var yData = new List<double>();

            for (int row = 2; row <= rowCount; row++)
            {
                var xValue = worksheet.Cells[row, 1].GetValue<double>();
                var yValue = worksheet.Cells[row, 2].GetValue<double>();

                xData.Add(xValue);
                yData.Add(yValue);
            }

            XValues = xData;
            YValues = yData;

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
}
