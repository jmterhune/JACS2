using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// Shared helpers + constants for the two bar-member schedulers
    /// (ScheduleImportBarMembers and ScheduleUpdateBarMembers). Holds the CSV
    /// column layout, the "eligible" code, the phone/name formatters, and a
    /// single point of config-loading from web.config.
    ///
    /// The CSV column layout matches the column names used by the standalone
    /// "Bar Member Import" Windows service, which is the canonical source of
    /// truth for the file format published by the Florida Bar.
    /// </summary>
    internal static class BarMemberFile
    {
        // The CSV's status column uses single-letter codes; "Y" is the only
        // value that means "eligible to practice" — every other code
        // (suspended, inactive, resigned, disbarred, deceased, etc.) is
        // skipped on import and treated as enabled=false on update.
        public const string EligibleStatus = "Y";

        // CSV column indices (no header row).
        public const int ColBarNum = 0;
        public const int ColFirstName = 1;
        public const int ColMiddleName = 2;
        public const int ColLastName = 3;
        public const int ColSuffix = 4;
        // 5 = firm (ignored)
        public const int ColStreet = 6;
        // 7 = street2 (ignored)
        public const int ColCity = 8;
        public const int ColState = 9;
        public const int ColZip = 10;
        public const int ColBusAreaCode = 11;
        public const int ColBusPhone = 12;
        public const int ColFaxAreaCode = 13;
        public const int ColFax = 14;
        public const int ColEmail = 15;
        // 16, 17 = filler (ignored)
        public const int ColStatus = 18;
        // 19 = filler (ignored)

        /// <summary>
        /// Reads JACS.BarImport.* appSettings into a typed config struct.
        /// Returns null and writes <paramref name="error"/> if any required
        /// key is missing.
        /// </summary>
        public static BarImportConfig LoadConfig(out string error)
        {
            string host = ConfigurationManager.AppSettings["JACS.BarImport.Host"];
            string user = ConfigurationManager.AppSettings["JACS.BarImport.Username"];
            string pass = ConfigurationManager.AppSettings["JACS.BarImport.Password"];
            string zip = ConfigurationManager.AppSettings["JACS.BarImport.BarFileName"];
            string csv = ConfigurationManager.AppSettings["JACS.BarImport.CsvFileName"];
            string dest = ConfigurationManager.AppSettings["JACS.BarImport.LocalDestinationPath"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass) || string.IsNullOrWhiteSpace(zip) ||
                string.IsNullOrWhiteSpace(csv) || string.IsNullOrWhiteSpace(dest))
            {
                error = "Missing one or more JACS.BarImport.* appSettings " +
                        "(Host, Username, Password, BarFileName, CsvFileName, LocalDestinationPath).";
                return null;
            }

            int.TryParse(ConfigurationManager.AppSettings["JACS.BarImport.Port"], out int port);
            if (port <= 0) port = 22;

            int.TryParse(ConfigurationManager.AppSettings["JACS.BarImport.MaxRetries"], out int retries);
            if (retries <= 0) retries = 5;

            error = null;
            return new BarImportConfig
            {
                Host = host,
                Port = port,
                Username = user,
                Password = pass,
                BarFileName = zip,
                CsvFileName = csv,
                LocalDestinationPath = dest.EndsWith("\\") || dest.EndsWith("/") ? dest : dest + "\\",
                MaxRetries = retries,
            };
        }

        /// <summary>
        /// Reads a cell from a DataRow, returning empty string for nulls,
        /// out-of-range column indices, and DBNulls. Always trims the value.
        /// </summary>
        public static string CellOrEmpty(DataRow row, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= row.Table.Columns.Count) return string.Empty;
            object v = row[columnIndex];
            return v == null || v == DBNull.Value ? string.Empty : v.ToString().Trim();
        }

        /// <summary>
        /// Trim leading zeros so the bar number compares to other normalised
        /// values (matches what EnsureAttorneyByBarNumberAsync already does).
        /// </summary>
        public static string NormalizeBarNum(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            string trimmed = raw.Trim().TrimStart('0');
            return string.IsNullOrEmpty(trimmed) ? raw.Trim() : trimmed;
        }

        /// <summary>
        /// Formats a phone number for display as "(AAA) XXX-XXXX" when the
        /// CSV's area-code and phone parts combine to a 10-digit number. Falls
        /// back gracefully when either piece is missing or malformed so we
        /// preserve whatever the file gave us instead of inventing structure.
        /// </summary>
        public static string FormatPhone(string areaCode, string phone)
        {
            string a = (areaCode ?? string.Empty).Trim();
            string p = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
            if (a.Length == 3 && p.Length == 7)
                return $"({a}) {p.Substring(0, 3)}-{p.Substring(3)}";
            if (a.Length == 3 && p.Length == 10)
                return $"({p.Substring(0, 3)}) {p.Substring(3, 3)}-{p.Substring(6)}";
            if (a.Length > 0 && p.Length > 0) return $"({a}) {p}";
            if (p.Length == 10) return $"({p.Substring(0, 3)}) {p.Substring(3, 3)}-{p.Substring(6)}";
            if (p.Length > 0) return p;
            return string.Empty;
        }

        /// <summary>
        /// Builds a "First Middle Last Suffix" name string from the CSV parts.
        /// Used by the *import* scheduler for newly-created attorneys.
        /// </summary>
        public static string JoinNameFirstMiddleLast(string first, string middle, string last, string suffix)
        {
            var parts = new List<string>(4);
            if (!string.IsNullOrWhiteSpace(first)) parts.Add(first.Trim());
            if (!string.IsNullOrWhiteSpace(middle)) parts.Add(middle.Trim());
            if (!string.IsNullOrWhiteSpace(last)) parts.Add(last.Trim());
            if (!string.IsNullOrWhiteSpace(suffix)) parts.Add(suffix.Trim());
            return string.Join(" ", parts);
        }

        /// <summary>
        /// Builds a "Last, First Middle" name string used by the *update*
        /// scheduler when refreshing existing attorney rows. Suffix, when
        /// present, is appended after the middle name. Returns an empty
        /// string when last name is missing so the caller can decide not to
        /// overwrite the existing value with blanks.
        /// </summary>
        public static string JoinNameLastFirstMiddle(string first, string middle, string last, string suffix)
        {
            string l = (last ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(l)) return string.Empty;

            string firstAndMiddle = string.Join(" ", new[] { first, middle, suffix }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim()));

            return string.IsNullOrEmpty(firstAndMiddle) ? l : l + ", " + firstAndMiddle;
        }
    }

    /// <summary>
    /// Bag of the JACS.BarImport.* appSettings values. Populated by
    /// BarMemberFile.LoadConfig and passed by reference to whichever
    /// scheduler is consuming the file.
    /// </summary>
    internal class BarImportConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BarFileName { get; set; }
        public string CsvFileName { get; set; }
        public string LocalDestinationPath { get; set; }
        public int MaxRetries { get; set; }
    }
}
