using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Produces the pipe-delimited contact export for Send Word Now bulk
    /// upload. Mirrors the legacy SWN-List.aspx.vb format (column order,
    /// dynamic group/phone/sms widths, custom-label slots) so an existing
    /// SWN-side workflow that consumes this file keeps working.
    ///
    /// Used by both the SwnList page (Download button) and the
    /// SwnController API (Export endpoint) — extracting the format here
    /// keeps the two from drifting.
    /// </summary>
    public static class SwnExportBuilder
    {
        public class Result
        {
            public string Content { get; set; }
            public int RowCount { get; set; }
        }

        public static Result Build()
        {
            var reports = new EmployeeReportController();
            var phoneCtrl = new PhoneController();
            var memberCtrl = new GroupMembershipController();
            var groupCtrl = new GroupController();

            var employees = reports.GetActiveEmployeesForSwn().ToList();
            var groupLookup = groupCtrl.GetAll()
                .ToDictionary(g => g.GroupID, g => g.GroupName);
            var swnGroupIds = new HashSet<int>(groupCtrl.GetSwnGroups().Select(g => g.GroupID));

            // Prefetch per-employee groups + phones; track max widths so we
            // can emit the variable column count the legacy format uses.
            var empGroups = new Dictionary<int, List<GroupInfo>>();
            var empPhones = new Dictionary<int, List<PhoneInfo>>();
            int maxGroups = 0;
            int maxPhones = 0;
            int maxSms = 0;

            foreach (var emp in employees)
            {
                // Only SWN-flagged groups go into the export.
                var membershipGroups = memberCtrl.GetForEmployee(emp.EmployeeId)
                    .Where(m => swnGroupIds.Contains(m.GroupId))
                    .Select(m => groupCtrl.GetById(m.GroupId))
                    .Where(g => g != null)
                    .ToList();
                empGroups[emp.EmployeeId] = membershipGroups;
                if (membershipGroups.Count > maxGroups) maxGroups = membershipGroups.Count;

                var phones = phoneCtrl.GetForEmployee(emp.EmployeeId).ToList();
                empPhones[emp.EmployeeId] = phones;

                var voiceCount = phones.Count(p => p.SwnCall);
                var smsCount = phones.Count(p => p.SwnText);
                if (voiceCount > maxPhones) maxPhones = voiceCount;
                if (smsCount > maxSms) maxSms = smsCount;
            }

            const int customFieldCount = 6;

            // ---------------- Header ----------------
            var header = new List<string>
            {
                "UNIQUE ID", "LAST NAME", "FIRST NAME", "MIDDLE INITIAL"
            };
            if (maxGroups > 1)
            {
                for (int i = 1; i <= maxGroups; i++)
                {
                    header.Add("GROUP ID " + i);
                    header.Add("GROUP DESCRIPTION " + i);
                }
            }
            else
            {
                header.Add("GROUP ID");
                header.Add("GROUP DESCRIPTION");
            }
            header.Add("ADDRESS 1");
            header.Add("ADDRESS 2");
            header.Add("CITY");
            header.Add("STATE/PROVINCE");
            header.Add("ZIP/POSTAL CODE");
            header.Add("COUNTRY");
            header.Add("TIME ZONE");
            header.Add("PREFERRED LANGUAGE");
            for (int i = 1; i <= customFieldCount; i++)
            {
                header.Add("CUSTOM LABEL " + i);
                header.Add("CUSTOM VALUE " + i);
            }
            if (maxPhones > 1)
            {
                for (int i = 1; i <= maxPhones; i++)
                {
                    header.Add("PHONE LABEL " + i);
                    header.Add("PHONE COUNTRY CODE " + i);
                    header.Add("PHONE " + i);
                    header.Add("PHONE EXTENSION " + i);
                    header.Add("CASCADE " + i);
                }
            }
            else
            {
                header.Add("PHONE LABEL");
                header.Add("PHONE COUNTRY CODE");
                header.Add("PHONE");
                header.Add("PHONE EXTENSION");
                header.Add("CASCADE");
            }
            header.Add("EMAIL LABEL 1");
            header.Add("EMAIL 1");
            header.Add("EMAIL LABEL 2");
            header.Add("EMAIL 2");
            if (maxSms > 1)
            {
                for (int i = 1; i <= maxSms; i++)
                {
                    header.Add("SMS LABEL " + i);
                    header.Add("SMS " + i);
                }
            }
            else
            {
                header.Add("SMS LABEL");
                header.Add("SMS");
            }
            header.Add("BB PIN LABEL");
            header.Add("BB PIN");

            var sb = new StringBuilder();
            sb.Append(string.Join("|", header.Select(Sanitize))).Append('\n');

            // ---------------- Rows ----------------
            int rowCount = 0;
            foreach (var emp in employees)
            {
                var row = new List<string>
                {
                    emp.EmployeeId.ToString(),
                    emp.LastName,
                    emp.FirstName,
                    emp.MiddleInitial
                };

                // Group columns: 2 per group (id + description) padded to maxGroups.
                var groupList = empGroups[emp.EmployeeId];
                int groupSlots = Math.Max(maxGroups, 1);
                for (int i = 0; i < groupSlots; i++)
                {
                    if (i < groupList.Count)
                    {
                        row.Add(groupList[i].GroupID.ToString());
                        row.Add(groupList[i].GroupName);
                    }
                    else
                    {
                        row.Add("");
                        row.Add("");
                    }
                }

                // Address1 + Address2: the model joins them into one Address
                // column with a newline separator. Split on the first newline
                // to recover the legacy two-line layout.
                var addressLines = SplitAddress(emp.Address);
                row.Add(addressLines.Item1);
                row.Add(addressLines.Item2);
                row.Add(emp.City ?? "");
                row.Add(string.IsNullOrEmpty(emp.State) ? "FL" : emp.State);
                row.Add(emp.Zip ?? "");
                row.Add("United States");
                row.Add("US/Eastern");
                row.Add("en-US");

                // Custom label / value pairs: 6 of them, all blank in the
                // legacy export. Reserved for future hand-edits in the SWN UI.
                row.Add("Title");        row.Add(emp.JobTitle ?? "");
                row.Add("County");       row.Add(GetCountyName(emp));
                row.Add("Department");   row.Add(GetDepartmentName(emp, groupLookup));
                row.Add("Location");     row.Add(GetLocationName(emp));
                row.Add("");             row.Add("");
                row.Add("");             row.Add("");

                // Voice phones — most-recently-saved first.
                var voicePhones = empPhones[emp.EmployeeId]
                    .Where(p => p.SwnCall)
                    .OrderByDescending(p => p.IsMain)
                    .ToList();
                int voiceSlots = Math.Max(maxPhones, 1);
                for (int i = 0; i < voiceSlots; i++)
                {
                    if (i < voicePhones.Count)
                    {
                        var p = voicePhones[i];
                        row.Add(string.IsNullOrEmpty(p.PhoneType) ? "Other" : p.PhoneType);
                        row.Add("1");
                        row.Add(p.PhoneNumber ?? "");
                        row.Add(p.SwnExcludeExtension ? "" : (p.Extension ?? ""));
                        row.Add(p.PhoneCascade.HasValue ? p.PhoneCascade.Value.ToString() : "");
                    }
                    else
                    {
                        row.Add(""); row.Add(""); row.Add(""); row.Add(""); row.Add("");
                    }
                }

                // Email — Work first, then Home (mirroring the legacy "Work" /
                // "Home" labels). Empty labels for empty addresses so SWN's
                // CSV importer doesn't see "Work" with no value.
                row.Add(string.IsNullOrEmpty(emp.Email) ? "" : "Work");
                row.Add(emp.Email ?? "");
                row.Add(string.IsNullOrEmpty(emp.PersonalEmail) ? "" : "Home");
                row.Add(emp.PersonalEmail ?? "");

                // SMS — phones with SwnText flag set.
                var smsPhones = empPhones[emp.EmployeeId]
                    .Where(p => p.SwnText)
                    .ToList();
                int smsSlots = Math.Max(maxSms, 1);
                for (int i = 0; i < smsSlots; i++)
                {
                    if (i < smsPhones.Count)
                    {
                        var p = smsPhones[i];
                        row.Add("SMS");
                        row.Add(p.PhoneNumber ?? "");
                    }
                    else
                    {
                        row.Add(""); row.Add("");
                    }
                }

                // BB Pin label/value — legacy fields, retained so the legacy
                // SWN consumer doesn't get confused by missing columns.
                row.Add("");
                row.Add("");

                sb.Append(string.Join("|", row.Select(Sanitize))).Append('\n');
                rowCount++;
            }

            return new Result { Content = sb.ToString(), RowCount = rowCount };
        }

        // -------- Lookup helpers (the model FKs are ints; we resolve to text) --------
        private static string GetCountyName(EmployeeInfo emp)
        {
            if (!emp.CountyId.HasValue) return "";
            var c = new CountyController().GetById(emp.CountyId.Value);
            return c?.CountyName ?? "";
        }
        private static string GetDepartmentName(EmployeeInfo emp, IDictionary<int, string> lookup)
        {
            if (!emp.DepartmentId.HasValue) return "";
            return lookup.TryGetValue(emp.DepartmentId.Value, out var name) ? name : "";
        }
        private static string GetLocationName(EmployeeInfo emp)
        {
            if (!emp.OfficeLocationId.HasValue) return "";
            var l = new OfficeLocationController().GetById(emp.OfficeLocationId.Value);
            return l?.Description ?? "";
        }

        private static Tuple<string, string> SplitAddress(string address)
        {
            if (string.IsNullOrEmpty(address)) return Tuple.Create("", "");
            var idx = address.IndexOf('\n');
            if (idx < 0) return Tuple.Create(address, "");
            return Tuple.Create(address.Substring(0, idx).TrimEnd('\r'), address.Substring(idx + 1));
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            // The pipe character is the field separator and tabs/newlines
            // would corrupt rows; collapse them so a single bad data field
            // can't shift every following column.
            return value.Replace("|", "/").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
        }
    }
}
