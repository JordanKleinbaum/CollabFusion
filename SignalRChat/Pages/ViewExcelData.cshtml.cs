using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;

namespace SignalRChat.Pages
{
    public class ViewExcelDataModel : PageModel
    {
        public string FileName { get; private set; }
        public List<List<string>> ExcelData { get; private set; }

        public void OnGet(string fileName)
        {
            FileName = fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", fileName);

            if (Path.GetExtension(fileName).ToLower() == ".csv")
            {
                ExcelData = ReadCsv(filePath);
            }
            else if (Path.GetExtension(fileName).ToLower() == ".xlsx")
            {
                ExcelData = ReadExcel(filePath);
            }
            // Add other supported file formats if necessary
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

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return Page();
            }
            else
            {
                HttpContext.Session.SetString("LoginError", "You must login to access that page!");
                return RedirectToPage("Index");
            }
        }
    }
}
