using CsvHelper;
using ExcelDataReader;
using MathNet.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using MathNet.Numerics.LinearRegression;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data;
//using System.IO;

namespace SignalRChat.Pages
{
    public class ProjectRevenueModel : PageModel
    {

        [BindProperty]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required")]
        public string InterestRate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Description is required")]
        public string InflationRate { get; set; }

        public string FileName { get; private set; }

        public List<List<string>> ExcelData { get; private set; }

        public List<List<double>> excelData { get; private set; }

        public IActionResult OnGet(string fileName)
        {
            //double[] xVals = { 1, 2, 3, 4, 5 };
            //double[] yVals = { 2, 4, 5, 4, 5 };

            //// Call the LinearRegression method and get the results
            //LinearRegression(xVals, yVals, 0, xVals.Length, out double rsquared, out double yintercept, out double slope);

            //// You can use the results as needed, such as printing them or passing them to the CSHTML page
            //Debug.WriteLine($"R-Squared: {rsquared}");
            //Debug.WriteLine($"Y-Intercept: {yintercept}");
            //Debug.WriteLine($"Slope: {slope}");

            //// Pass the results to the CSHTML page using ViewData or a model
            //ViewData["RSquared"] = rsquared;
            //ViewData["YIntercept"] = yintercept;
            //ViewData["Slope"] = slope;

            FileName = fileName;

            //List<List<string>> excelData = ReadExcel(fileName);

       
            List<List<string>> excelData = ReadExcel(fileName);

            // Extract x and y values from excelData
            //List<string> xValues = excelData.Select(row => row[0]).ToList();
            //List<string> yValues = excelData.Select(row => row[1]).ToList();

            // Perform linear regression using the extracted x and y values
            //LinearRegression(xValues.ToArray(), yValues.ToArray(), 0, xValues.Count, out double rsquared, out double yintercept, out double slope);

            ////Pass the regression results to the CSHTML page
            //ViewData["RSquared"] = rsquared;
            //ViewData["YIntercept"] = yintercept;
            //ViewData["Slope"] = slope;

            if (HttpContext.Session.GetString("username") != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", fileName);

                if (Path.GetExtension(fileName).ToLower() == ".csv")
                {
                    ExcelData = ReadCsv(filePath);
                }
                else if (Path.GetExtension(fileName).ToLower() == ".xlsx")
                {
                    //ExcelData = ReadExcel(filePath);
                }
                // Add other supported file formats if necessary
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }


        }

        public static void LinearRegression(double[] xVals, double[] yVals,
                                            int inclusiveStart, int exclusiveEnd,
                                            out double rsquared, out double yintercept,
                                            out double slope)
        {
            Debug.Assert(xVals.Length == yVals.Length);
            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double ssX = 0;
            double ssY = 0;
            double sumCodeviates = 0;
            double sCo = 0;
            double count = exclusiveEnd - inclusiveStart;

            for (int ctr = inclusiveStart; ctr < exclusiveEnd; ctr++)
            {
                double x = xVals[ctr];
                double y = yVals[ctr];
                sumCodeviates += x * y;
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }
            ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
            ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
            double RNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            double RDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            double meanX = sumOfX / count;
            double meanY = sumOfY / count;
            double dblR = RNumerator / Math.Sqrt(RDenom);
            rsquared = dblR * dblR;
            yintercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }


    private List<List<string>> ReadCsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                var data = new List<List<string>>();

                foreach (var record in records)
                {
                    var recordDict = record as IDictionary<string, object>;
                    var rowValues = recordDict.Values.Select(cell => cell.ToString()).ToList();
                    data.Add(rowValues);
                }

                return data;
            }
        }


        private List<List<string>> ReadExcel(string filePath)
        {
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false // Do not use the first row as header
                    }
                });

                var dataTable = result.Tables[0];
                var data = new List<List<string>>();

                foreach (System.Data.DataRow row in dataTable.Rows)
                {
                    var rowData = row.ItemArray.Select(cell => cell.ToString()).ToList();
                    data.Add(rowData);
                }

                return data;
            }
        }

        //private List<List<double>> ReadExcel(string filePath)
        //{
        //    using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
        //    using (var reader = ExcelReaderFactory.CreateReader(stream))
        //    {
        //        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
        //        {
        //            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
        //            {
        //                UseHeaderRow = false // Do not use the first row as header
        //            }
        //        });

        //        var dataTable = result.Tables[0];
        //        var data = new List<List<double>>();

        //        foreach (System.Data.DataRow row in dataTable.Rows)
        //        {
        //            var rowData = row.ItemArray.Select(cell =>
        //            {
        //                double.TryParse(cell.ToString(), out double cellValue);
        //                return cellValue;
        //            }).ToList();
        //            data.Add(rowData);
        //        }

        //        return data;
        //    }
        //}




    }
}
