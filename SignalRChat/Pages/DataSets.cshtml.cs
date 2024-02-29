using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ExcelDataReader;

namespace SignalRChat.Pages
{
    public class DataSetsModel : PageModel
    {
        [BindProperty]
        public List<IFormFile> FileList { get; set; }
        public Dictionary<string, List<List<object>>> ExcelDataDict { get; private set; }
        public bool UploadSuccessful { get; private set; }
        public string ErrorFileName { get; private set; }

        // SQL Server connection string
        private readonly string _connectionString = "Server=localhost;Database=Lab3;Trusted_Connection=True;";

        // OnPost, Upload the files to the Uploads Folder in wwwroot
        public IActionResult OnPost()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            ExcelDataDict = new Dictionary<string, List<List<object>>>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (var formFile in FileList)
                {
                    if (formFile != null && formFile.Length > 0)
                    {
                        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");
                        var filePath = Path.Combine(uploadDirectory, formFile.FileName);

                        // Check if filename already exists in the database
                        string fileName = Path.GetFileNameWithoutExtension(formFile.FileName);
                        string checkIfExistsQuery = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{fileName}'";
                        SqlCommand checkIfExistsCommand = new SqlCommand(checkIfExistsQuery, connection);
                        int existingCount = (int)checkIfExistsCommand.ExecuteScalar();

                        if (existingCount > 0)
                        {
                            UploadSuccessful = false;
                            ErrorFileName = formFile.FileName;
                            return Page();
                        }

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            formFile.CopyTo(stream);
                        }

                        List<List<object>> fileData;

                        if (Path.GetExtension(formFile.FileName).ToLower() == ".csv")
                        {
                            fileData = ReadCsv(filePath);
                        }
                        else if (Path.GetExtension(formFile.FileName).ToLower() == ".xlsx")
                        {
                            fileData = ReadExcel(filePath);
                        }
                        else
                        {
                            // Unsupported file format
                            UploadSuccessful = false;
                            ErrorFileName = formFile.FileName;
                            return Page();
                        }

                        ExcelDataDict.Add(formFile.FileName, fileData);

                        // Upload data to SQL Server
                        UploadToSqlServer(formFile.FileName, fileData);

                        UploadSuccessful = true;
                    }
                }
            }

            return Page();
        }

        // Method to read CSV file
        // Method to read CSV file without treating the first row as headers
        private List<List<object>> ReadCsv(string filePath)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<dynamic>().ToList();
                List<List<object>> data = new List<List<object>>();

                foreach (var record in records)
                {
                    var recordDict = record as IDictionary<string, object>;
                    List<object> rowValues = recordDict.Values.ToList();
                    data.Add(rowValues);
                }

                return data;
            }
        }


        // Method to read Excel file
        // Method to read Excel file without treating the first row as headers
        private List<List<object>> ReadExcel(string filePath)
        {
            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataReader.ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataReader.ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false // Do not use the first row as header
                    }
                });

                DataTable dataTable = result.Tables[0];
                List<List<object>> data = new List<List<object>>();

                foreach (DataRow row in dataTable.Rows)
                {
                    List<object> rowData = row.ItemArray.ToList();
                    data.Add(rowData);
                }

                return data;
            }
        }

        private void UploadToSqlServer(string fileName, List<List<object>> data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                if (data == null || data.Count == 0) return; // Ensure there's data to process

                // Generate a table name based on the file name
                string tableName = Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

                // Assuming all rows have the same number of columns, use the first row to determine column count
                int columnCount = data[0].Count;

                // Check if the first row contains column headers
                bool hasHeaders = data.Count > 1 && data[0].All(cell => cell is string);

                List<string> columnDefinitions = new List<string>();
                if (!hasHeaders)
                {
                    // If there are no headers, create generic column names
                    for (int i = 1; i <= columnCount; i++)
                    {
                        columnDefinitions.Add($"Column{i} NVARCHAR(MAX)");
                    }
                }
                else
                {
                    // If there are headers, use them for column names
                    List<string> columnHeaders = data[0].Select(cell => cell.ToString()).ToList();
                    for (int i = 0; i < columnCount; i++)
                    {
                        columnDefinitions.Add($"[{columnHeaders[i]}] NVARCHAR(MAX)");
                    }
                    // Skip the first row containing headers
                    data = data.Skip(1).ToList();
                }

                string createTableCommandText = $"CREATE TABLE [{tableName}] ({string.Join(", ", columnDefinitions)})";

                // Execute the command to create the table
                using (var createTableCommand = new SqlCommand(createTableCommandText, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                // Insert all data rows starting from the first
                foreach (var rowData in data)
                {
                    string insertQuery = $"INSERT INTO [{tableName}] VALUES (";
                    foreach (var cell in rowData)
                    {
                        var cellValue = cell.ToString().Replace("'", "''"); // Escape single quotes
                        insertQuery += $"'{cellValue}', ";
                    }
                    insertQuery = insertQuery.TrimEnd(',', ' ') + ")";

                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
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