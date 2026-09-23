using DotNetNuke.Abstractions.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Builds the Crisis24 Person/HR Feed file — a comma-delimited CSV that
    /// gets pushed to the Crisis24 SFTP site by <see cref="Crisis24Uploader"/>.
    ///
    /// Layout follows the "Crisis24 Person Implementation Guide" (rev
    /// 2024-04-23). We emit the six FIELD TYPE 1 required columns plus the
    /// FIELD TYPE 2 optional email/phone columns:
    ///
    ///   Unique Identifier | Legal First Name | Legal Last Name
    ///   Primary Email | Organization Identifier | Status
    ///   Email 2 | Email 3 | Phone1 | Phone2 | Phone3
    ///
    /// Spec rules honoured here:
    ///  - Full file on each transmission; every active employee appears once.
    ///  - Unique Identifier appears exactly one time per file (EmployeeId is
    ///    the PK, so uniqueness is guaranteed by the DB).
    ///  - Blank legal first/last name is replaced with "." per the guide's
    ///    placeholder recommendation.
    ///  - A missing primary email becomes "{EmployeeId}@noemail.com" — the
    ///    guide requires a unique, distinct address for every person, and a
    ///    record without one fails ingestion outright.
    ///  - Phone values are normalised to E.164 (+1XXXXXXXXXX) so they're
    ///    usable by Crisis24 messaging.
    ///  - Status is "A" for every row. Only active employees are exported;
    ///    Crisis24 was not asked to receive "T" (terminated) rows, so
    ///    departed staff simply stop appearing on the feed.
    /// </summary>
    public static class Crisis24ExportBuilder
    {
        /// <summary>Column headers, in emitted order.</summary>
        private static readonly string[] Header =
        {
            "Unique Identifier",
            "Legal First Name",
            "Legal Last Name",
            "Primary Email",
            "Organization Identifier",
            "Status",
            "Email 2",
            "Email 3",
            "Phone1",
            "Phone2",
            "Phone3"
        };

        /// <summary>Crisis24 accepts up to three phone columns on this feed.</summary>
        private const int PhoneColumns = 3;

        /// <summary>Placeholder the guide recommends for an empty legal name.</summary>
        private const string NamePlaceholder = ".";

        public class Result
        {
            /// <summary>Full CSV text, header row included.</summary>
            public string Content { get; set; }

            /// <summary>Data rows written (excludes the header).</summary>
            public int RowCount { get; set; }

            /// <summary>
            /// Remote file name, per the guide's unique-naming convention:
            /// CLIENTNAME_YYYYMMDD.csv (TEST_CLIENTNAME_YYYYMMDD.csv in test
            /// mode). Auto-processing is case-sensitive, so this is all upper.
            /// </summary>
            public string FileName { get; set; }
        }

        public static Result Build(IHostSettings hostSettings)
        {
            var employees = new EmployeeReportController(hostSettings)
                .GetActiveEmployeesForExport()
                .ToList();
            var phoneCtrl = new PhoneController(hostSettings);
            var orgId = OrganizationId;

            var sb = new StringBuilder();
            sb.Append(string.Join(",", Header.Select(Csv))).Append("\r\n");

            // The guide requires a unique/distinct primary email per person and
            // fails any record that repeats one. tjc_employee has no unique
            // constraint on Email, so collisions (shared mailboxes, a stale
            // duplicate row) are caught here rather than on Crisis24's side.
            var usedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int rowCount = 0;
            foreach (var emp in employees)
            {
                var primary = PrimaryEmail(emp, usedEmails);

                // Don't repeat the primary address in Email 2 — that happens
                // when an employee has no work email and the personal one was
                // promoted to primary.
                var secondary = Clean(emp.PersonalEmail);
                if (secondary.Equals(primary, StringComparison.OrdinalIgnoreCase))
                    secondary = string.Empty;

                var row = new List<string>
                {
                    emp.EmployeeId.ToString(CultureInfo.InvariantCulture),
                    NameOrPlaceholder(emp.FirstName),
                    NameOrPlaceholder(emp.LastName),
                    primary,
                    orgId,
                    "A",
                    secondary,
                    string.Empty  // Email 3 — not tracked in tjc_employee.
                };

                var phones = SelectPhones(phoneCtrl.GetForEmployee(emp.EmployeeId));
                for (int i = 0; i < PhoneColumns; i++)
                    row.Add(i < phones.Count ? phones[i] : string.Empty);

                sb.Append(string.Join(",", row.Select(Csv))).Append("\r\n");
                rowCount++;
            }

            return new Result
            {
                Content = sb.ToString(),
                RowCount = rowCount,
                FileName = BuildFileName()
            };
        }

        // ------------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------------

        /// <summary>
        /// Required "Organization Identifier" column — directs where the
        /// profile lands in the Crisis24 hierarchy. Configurable so it can be
        /// switched to whatever code Crisis24 assigns without a rebuild.
        /// </summary>
        private static string OrganizationId
        {
            get
            {
                var v = ConfigurationManager.AppSettings["Crisis24OrgId"];
                return string.IsNullOrWhiteSpace(v) ? "12th Judicial Circuit" : v.Trim();
            }
        }

        /// <summary>
        /// File name stem identifying us to Crisis24's auto-processor. The
        /// guide asks for a unique, case-consistent name per transmission.
        /// </summary>
        private static string ClientName
        {
            get
            {
                var v = ConfigurationManager.AppSettings["Crisis24FilePrefix"];
                return string.IsNullOrWhiteSpace(v) ? "JUD12" : v.Trim();
            }
        }

        /// <summary>
        /// True while the feed is still being validated by Crisis24 — the
        /// guide wants a TEST_ prefix so their side routes it to the test
        /// processor instead of the live person database.
        /// </summary>
        private static bool IsTestMode
        {
            get
            {
                var v = ConfigurationManager.AppSettings["Crisis24TestMode"];
                return !string.IsNullOrWhiteSpace(v)
                    && bool.TryParse(v.Trim(), out var b) && b;
            }
        }

        private static string BuildFileName()
        {
            var name = ClientName + "_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".csv";
            if (IsTestMode) name = "TEST_" + name;
            return name.ToUpperInvariant();
        }

        // ------------------------------------------------------------------
        // Field helpers
        // ------------------------------------------------------------------

        private static string NameOrPlaceholder(string value)
        {
            var clean = Clean(value);
            return clean.Length == 0 ? NamePlaceholder : clean;
        }

        /// <summary>
        /// The guide requires a unique, distinct primary email per person and
        /// fails the whole record without one, so employees with no work email
        /// fall back to their personal address and then to the recommended
        /// uniqueID@noemail.com convention.
        ///
        /// <paramref name="used"/> carries the addresses already emitted in
        /// this file; a collision falls through to the synthetic address rather
        /// than shipping a duplicate that Crisis24 would reject.
        /// </summary>
        private static string PrimaryEmail(EmployeeInfo emp, HashSet<string> used)
        {
            var synthetic = emp.EmployeeId.ToString(CultureInfo.InvariantCulture) + "@noemail.com";

            foreach (var candidate in new[] { Clean(emp.Email), Clean(emp.PersonalEmail), synthetic })
            {
                if (candidate.Length == 0) continue;
                if (used.Add(candidate)) return candidate;
            }

            // Every candidate was taken, which means the synthetic address
            // collided too — only possible if a literal "{id}@noemail.com" is
            // stored on another row. EmployeeId is the PK, so qualifying it
            // once more is guaranteed to be unique within the file.
            var unique = emp.EmployeeId.ToString(CultureInfo.InvariantCulture) + ".dup@noemail.com";
            used.Add(unique);
            return unique;
        }

        /// <summary>
        /// Picks up to three phone numbers for the feed.
        ///
        /// The candidate set is the phones flagged for emergency-notification
        /// call or text (the SwnCall / SwnText columns on tjc_employee_phone —
        /// column names predate Crisis24 and are still the only per-phone
        /// "use this for mass notification" markers HR maintains). Ordering
        /// puts the main number first, then textable mobiles, so the three
        /// slots carry the most reachable numbers when an employee has more.
        /// Duplicate numbers are collapsed — Crisis24 gains nothing from the
        /// same E.164 value in two columns.
        /// </summary>
        private static List<string> SelectPhones(IEnumerable<PhoneInfo> phones)
        {
            var picked = new List<string>();
            if (phones == null) return picked;

            var candidates = phones
                .Where(p => p != null && (p.SwnCall || p.SwnText))
                .OrderByDescending(p => p.IsMain)
                .ThenByDescending(p => p.SwnText)
                .ThenBy(p => p.PhoneId);

            foreach (var p in candidates)
            {
                var e164 = ToE164(p.PhoneNumber);
                if (e164.Length == 0) continue;
                if (picked.Contains(e164)) continue;
                picked.Add(e164);
                if (picked.Count == PhoneColumns) break;
            }
            return picked;
        }

        /// <summary>
        /// Normalises a stored phone number to E.164 (+ country code + number,
        /// max fifteen digits). Numbers are stored digits-only (see
        /// DigitsOnlyAttribute) but historical rows can still carry masks, so
        /// everything non-numeric is stripped first.
        ///
        /// Returns an empty string for anything that can't be expressed as a
        /// valid E.164 number — a malformed value is worse than an empty
        /// column, because Crisis24 would try to dial it.
        /// </summary>
        private static string ToE164(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var digits = new string(raw.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return string.Empty;

            // 10 digits = bare NANP number; prefix the US country code.
            if (digits.Length == 10) return "+1" + digits;

            // 11 digits starting with 1 is already country-code qualified.
            if (digits.Length == 11 && digits[0] == '1') return "+" + digits;

            // Anything else is only usable if the source explicitly supplied
            // an international number. E.164 caps at fifteen digits.
            if (raw.TrimStart().StartsWith("+") && digits.Length >= 8 && digits.Length <= 15)
                return "+" + digits;

            return string.Empty;
        }

        /// <summary>
        /// Collapses embedded CR/LF/tabs to spaces and trims. Newlines inside
        /// a field would break row alignment on Crisis24's side even when
        /// quoted, and the guide asks for a file cleansed of oddities.
        /// </summary>
        private static string Clean(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Trim();
        }

        /// <summary>RFC 4180 field encoding for the comma-delimited file.</summary>
        private static string Csv(string value)
        {
            var clean = Clean(value);
            if (clean.Length == 0) return string.Empty;
            if (clean.IndexOf(',') < 0 && clean.IndexOf('"') < 0) return clean;
            return "\"" + clean.Replace("\"", "\"\"") + "\"";
        }
    }
}
