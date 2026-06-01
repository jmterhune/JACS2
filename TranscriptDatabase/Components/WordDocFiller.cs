using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace tjc.Modules.TranscriptDatabase.Components
{
    public class WordDocumentFiller
    {
        private readonly string _connectionString;

        public WordDocumentFiller(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Dictionary<string, string> GetUserData(int userId)
        {
            var data = new Dictionary<string, string>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT FirstName, LastName, Email FROM Users WHERE Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        data["FirstName"] = reader["FirstName"].ToString();
                        data["LastName"] = reader["LastName"].ToString();
                        data["Email"] = reader["Email"].ToString();
                    }
                }
            }

            return data;
        }

        public void FillWordTemplate(string templatePath, string outputPath, Dictionary<string, string> fieldValues)
        {
            File.Copy(templatePath, outputPath, true); // Copy template to new file

            using (var wordDoc = WordprocessingDocument.Open(outputPath, true))
            {
                var sdtElements = wordDoc.MainDocumentPart.Document.Descendants<SdtElement>();

                foreach (var sdt in sdtElements)
                {
                    var alias = sdt.SdtProperties.GetFirstChild<Tag>()?.Val?.Value;

                    if (alias != null && fieldValues.ContainsKey(alias))
                    {
                        var textElement = sdt.Descendants<Text>().FirstOrDefault();
                        if (textElement != null)
                        {
                            textElement.Text = fieldValues[alias];
                        }
                    }
                }

                wordDoc.MainDocumentPart.Document.Save();
            }
        }
    }
}