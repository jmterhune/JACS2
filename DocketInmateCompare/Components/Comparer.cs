// File: Components\DefendantNameComparer.cs
using CsvHelper;
using CsvHelper.Configuration;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace tjc.Modules.DocketInmateCompare.Components
{
    public class DefendantNameComparer
    {
        private readonly double _fuzzyThreshold;
        private readonly double _surnameMinSimilarity;

        public DefendantNameComparer(double fuzzyThreshold = 0.90, double surnameMinSimilarity = 0.85)
        {
            _fuzzyThreshold = fuzzyThreshold;
            _surnameMinSimilarity = surnameMinSimilarity;
        }

        public List<NameMatchResult> CompareFiles(string courtCsvPath, string jailXlsxPath)
        {
            var courtEntriesDict = ReadCourtEntries(courtCsvPath);
            var jailEntries = ReadJailEntries(jailXlsxPath);

            return FindMatches(courtEntriesDict, jailEntries);
        }

        private Dictionary<string, CourtEntry> ReadCourtEntries(string filePath)
        {
            var uniqueCourtEntries = new Dictionary<string, CourtEntry>(StringComparer.OrdinalIgnoreCase);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim | TrimOptions.InsideQuotes,
                BadDataFound = null,
                MissingFieldFound = null
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    string rawName = csv.GetField(0)?.Trim('"').Trim();
                    if (string.IsNullOrWhiteSpace(rawName)) continue;

                    // Force space after comma for consistency
                    string originalName = Regex.Replace(rawName, @",(?!\s)", ", ");
                    string normalizedName = originalName.ToUpperInvariant();

                    string originalCase = csv.GetField(3)?.Trim(); // Case # column
                    string rawStart = csv.GetField(6)?.Trim();
                    string motionType = csv.GetField(7)?.Trim();
                    string eventType = csv.GetField(8)?.Trim();

                    // Strip date part from Start time - keep only time
                    string cleanedStart = StripDateFromStart(rawStart);

                    if (!uniqueCourtEntries.ContainsKey(normalizedName))
                    {
                        uniqueCourtEntries[normalizedName] = new CourtEntry
                        {
                            OriginalName = originalName,
                            Name = normalizedName,
                            OriginalCase = originalCase,
                            CaseNum = originalCase?.Replace(" ", "").ToUpperInvariant(),
                            Start = cleanedStart,
                            MotionType = motionType,
                            EventType = eventType
                        };
                    }
                }
            }

            return uniqueCourtEntries;
        }

        private static string StripDateFromStart(string rawStart)
        {
            if (string.IsNullOrWhiteSpace(rawStart)) return rawStart;

            // Try common formats: MM/dd/yyyy h:mm tt  or  MM/dd/yyyy HH:mm
            string[] formats = {
                "MM/dd/yyyy h:mm tt",
                "MM/dd/yyyy hh:mm tt",
                "MM/dd/yyyy HH:mm",
                "M/d/yyyy h:mm tt",
                "M/d/yyyy HH:mm",
                "MM/dd/yy h:mm tt",
                "MM/dd/yy HH:mm"
            };

            if (DateTime.TryParseExact(rawStart, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt))
            {
                // If it has AM/PM → use 12-hour format
                if (rawStart.Contains("AM") || rawStart.Contains("PM"))
                {
                    return dt.ToString("h:mm tt", CultureInfo.InvariantCulture);
                }
                // Otherwise use 24-hour
                return dt.ToString("HH:mm", CultureInfo.InvariantCulture);
            }

            // Fallback: return original if parsing fails
            return rawStart;
        }

        private List<JailEntry> ReadJailEntries(string filePath)
        {
            var entries = new List<JailEntry>();

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(filePath, false))
            {
                var wbPart = doc.WorkbookPart ?? throw new Exception("Workbook part not found.");

                var sheet = wbPart.Workbook.Descendants<Sheet>()
                    .FirstOrDefault(s => s.Name?.Value == "Sheet1")
                    ?? throw new Exception("Sheet1 not found in jail file.");

                var wsPart = (WorksheetPart)wbPart.GetPartById(sheet.Id)
                    ?? throw new Exception("Worksheet part not found.");

                var worksheet = wsPart.Worksheet;
                var sheetData = worksheet.GetFirstChild<SheetData>()
                    ?? throw new Exception("No sheet data found.");

                var sharedStrings = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault()?.SharedStringTable;

                var rows = sheetData.Elements<Row>().ToList();

                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    if (row.RowIndex <= 13) continue; // Skip header rows

                    string rawValue = null;

                    // Search all cells in this row for the one containing a comma (name cell)
                    foreach (var cell in row.Elements<Cell>())
                    {
                        string cellValue = GetCellValue(cell, sharedStrings)?.Trim();
                        if (!string.IsNullOrWhiteSpace(cellValue) && cellValue.Contains(","))
                        {
                            rawValue = cellValue;
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(rawValue)) continue;

                    // Force space after comma for consistency
                    string nameWithPossibleStar = Regex.Replace(rawValue, @",(?!\s)", ", ");

                    // Remove any leading/trailing asterisk(s) and surrounding whitespace
                    string cleanedName = Regex.Replace(nameWithPossibleStar, @"^\s*\*+\s*|\s*\*+\s*$", "").Trim();

                    string originalName = cleanedName;
                    string name = originalName.ToUpperInvariant();

                    // Get case number from next row, column AH (34)
                    if (i + 1 >= rows.Count) continue;
                    var nextRow = rows[i + 1];
                    if (nextRow.RowIndex != row.RowIndex + 1) continue;

                    string col = GetColumnName(34); // AH = column 34
                    string refCell = $"{col}{nextRow.RowIndex}";
                    var caseCell = nextRow.Elements<Cell>()
                        .FirstOrDefault(c => string.Equals(c.CellReference?.Value, refCell, StringComparison.OrdinalIgnoreCase));

                    string originalCase = GetCellValue(caseCell, sharedStrings)?.Trim();
                    string caseNum = originalCase?.Replace(" ", "").ToUpperInvariant();

                    entries.Add(new JailEntry
                    {
                        OriginalName = originalName,
                        Name = name,
                        OriginalCase = originalCase,
                        CaseNum = caseNum
                    });
                }
            }

            return entries;
        }

        private static string GetColumnName(int colNum)
        {
            string col = "";
            while (colNum > 0)
            {
                int mod = (colNum - 1) % 26;
                col = (char)('A' + mod) + col;
                colNum = (colNum - mod - 1) / 26;
            }
            return col;
        }

        private static string GetCellValue(Cell cell, SharedStringTable sharedStrings)
        {
            if (cell?.CellValue == null) return null;

            string value = cell.CellValue.InnerText;

            if (cell.DataType?.Value == CellValues.SharedString && sharedStrings != null)
            {
                if (int.TryParse(value, out int idx) && idx >= 0 && idx < sharedStrings.ChildElements.Count)
                    return sharedStrings.ChildElements[idx].InnerText;
            }

            return value;
        }

        private List<NameMatchResult> FindMatches(Dictionary<string, CourtEntry> courtEntriesDict, List<JailEntry> jailEntries)
        {
            var results = new List<NameMatchResult>();

            foreach (var court in courtEntriesDict.Values)
            {
                string courtSurname = GetSurname(court.Name);

                foreach (var jail in jailEntries)
                {
                    string jailSurname = GetSurname(jail.Name);

                    // Hard filter: surnames must be reasonably similar
                    double surnameSim = JaroWinklerProximity(courtSurname, jailSurname);
                    if (surnameSim < _surnameMinSimilarity) continue;

                    string normalCourt = court.Name.Replace(" ", "");
                    string normalJail = jail.Name.Replace(" ", "");
                    double sim1 = JaroWinklerProximity(normalCourt, normalJail);

                    var wordsCourt = court.Name.Replace(",", " ")
                        .Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    wordsCourt.Sort();
                    string sortedCourt = string.Join(" ", wordsCourt);

                    var wordsJail = jail.Name.Replace(",", " ")
                        .Split(' ', (char)StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                    wordsJail.Sort();
                    string sortedJail = string.Join(" ", wordsJail);

                    double sim2 = JaroWinklerProximity(sortedCourt, sortedJail);

                    double overallSimilarity = Math.Max(sim1, sim2);

                    if (overallSimilarity >= _fuzzyThreshold)
                    {
                        results.Add(new NameMatchResult
                        {
                            CourtName = court.OriginalName,
                            CourtCase = court.OriginalCase ?? "",
                            JailName = jail.OriginalName,
                            JailCase = jail.OriginalCase ?? "",
                            Similarity = overallSimilarity,
                            Start = court.Start,
                            MotionType = court.MotionType,
                            EventType = court.EventType
                        });
                    }
                }
            }

            results.Sort((a, b) =>
            {
                int cmp = b.Similarity.CompareTo(a.Similarity);
                return cmp != 0 ? cmp : string.Compare(a.CourtName, b.CourtName, StringComparison.OrdinalIgnoreCase);
            });

            return results;
        }

        private static string GetSurname(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";

            int commaIndex = name.IndexOf(',');
            if (commaIndex > 0)
            {
                return name.Substring(0, commaIndex).Trim();
            }

            var parts = name.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : "";
        }

        private static double JaroWinklerProximity(string s1, string s2)
        {
            const double threshold = 0.7;
            const int prefixLength = 4;

            int len1 = s1.Length;
            int len2 = s2.Length;
            if (len1 == 0) return len2 == 0 ? 1.0 : 0.0;

            int searchRange = Math.Max(0, Math.Max(len1, len2) / 2 - 1);

            bool[] match1 = new bool[len1];
            bool[] match2 = new bool[len2];

            int common = 0;
            for (int i = 0; i < len1; i++)
            {
                int start = Math.Max(0, i - searchRange);
                int end = Math.Min(i + searchRange + 1, len2);
                for (int j = start; j < end; j++)
                {
                    if (match2[j]) continue;
                    if (s1[i] != s2[j]) continue;
                    match1[i] = match2[j] = true;
                    common++;
                    break;
                }
            }
            if (common == 0) return 0.0;

            int transpositions = 0;
            int k = 0;
            for (int i = 0; i < len1; i++)
            {
                if (!match1[i]) continue;
                while (k < len2 && !match2[k]) k++;
                if (k >= len2 || s1[i] != s2[k]) transpositions++;
                k++;
            }
            transpositions /= 2;

            double weight = (common / (double)len1 + common / (double)len2 + (common - transpositions) / (double)common) / 3.0;

            if (weight <= threshold) return weight;

            int prefix = 0;
            int maxPrefix = Math.Min(prefixLength, Math.Min(len1, len2));
            while (prefix < maxPrefix && s1[prefix] == s2[prefix]) prefix++;

            return weight + 0.1 * prefix * (1.0 - weight);
        }
    }
}