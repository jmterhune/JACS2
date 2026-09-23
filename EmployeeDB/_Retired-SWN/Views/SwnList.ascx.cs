/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class SwnList : EmployeeDBModuleBase
    {
        private readonly EmployeeReportController _reports = new EmployeeReportController();
        private readonly PhoneController _phones = new PhoneController();
        private readonly GroupMembershipController _memberships = new GroupMembershipController();
        private readonly GroupController _groups = new GroupController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var employees = _reports.GetActiveEmployeesForSwn().ToList();
                var groupLookup = _groups.GetAll().ToDictionary(g => g.GroupID, g => g.GroupName);

                // Prefetch per-employee groups/phones and determine max widths.
                var empGroups = new Dictionary<int, List<string>>();
                var empPhones = new Dictionary<int, List<PhoneInfo>>();
                int maxGroups = 0;
                int maxPhones = 0;
                int maxSms = 0;

                foreach (var emp in employees)
                {
                    var membershipNames = _memberships.GetForEmployee(emp.EmployeeId)
                        .Where(m => groupLookup.ContainsKey(m.GroupId))
                        .Select(m => groupLookup[m.GroupId])
                        .ToList();
                    empGroups[emp.EmployeeId] = membershipNames;
                    if (membershipNames.Count > maxGroups) maxGroups = membershipNames.Count;

                    var phones = _phones.GetForEmployee(emp.EmployeeId).ToList();
                    empPhones[emp.EmployeeId] = phones;

                    var voiceCount = phones.Count(p => p.SwnCall);
                    var smsCount = phones.Count(p => p.SwnText && !string.IsNullOrEmpty(p.PhoneType)
                        && (p.PhoneType.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) >= 0
                            || p.PhoneType.IndexOf("cell", StringComparison.OrdinalIgnoreCase) >= 0));

                    if (voiceCount > maxPhones) maxPhones = voiceCount;
                    if (smsCount > maxSms) maxSms = smsCount;
                }

                if (maxPhones < 1) maxPhones = 1;
                if (maxSms < 1) maxSms = 1;
                if (maxGroups < 1) maxGroups = 1;

                const int customFieldCount = 6;

                var sb = new StringBuilder();

                // Header row
                var header = new List<string>
                {
                    "UNIQUE ID", "LAST NAME", "FIRST NAME", "MIDDLE INITIAL"
                };
                for (int i = 1; i <= maxGroups; i++) header.Add("GROUP " + i);
                header.Add("ADDRESS 1");
                header.Add("ADDRESS 2");
                header.Add("CITY");
                header.Add("STATE");
                header.Add("ZIP");
                header.Add("COUNTRY");
                header.Add("TIME ZONE");
                header.Add("LANGUAGE");
                for (int i = 1; i <= customFieldCount; i++)
                {
                    header.Add("CUSTOM LABEL " + i);
                    header.Add("CUSTOM VALUE " + i);
                }
                for (int i = 1; i <= maxPhones; i++)
                {
                    header.Add("PHONE LABEL " + i);
                    header.Add("PHONE COUNTRY " + i);
                    header.Add("PHONE NUMBER " + i);
                    header.Add("PHONE EXT " + i);
                    header.Add("PHONE CASCADE " + i);
                }
                header.Add("EMAIL LABEL 1");
                header.Add("EMAIL 1");
                header.Add("EMAIL LABEL 2");
                header.Add("EMAIL 2");
                for (int i = 1; i <= maxSms; i++)
                {
                    header.Add("SMS LABEL " + i);
                    header.Add("SMS COUNTRY " + i);
                    header.Add("SMS NUMBER " + i);
                }
                header.Add("BB PIN LABEL");
                header.Add("BB PIN");

                sb.AppendLine(string.Join("|", header.Select(Sanitize)));

                // Data rows
                foreach (var emp in employees)
                {
                    var row = new List<string>
                    {
                        emp.EmployeeId.ToString(),
                        emp.LastName,
                        emp.FirstName,
                        emp.MiddleInitial
                    };

                    var groupList = empGroups[emp.EmployeeId];
                    for (int i = 0; i < maxGroups; i++)
                    {
                        row.Add(i < groupList.Count ? groupList[i] : "");
                    }

                    row.Add(emp.Address);
                    row.Add(""); // Address 2
                    row.Add(emp.City);
                    row.Add(string.IsNullOrEmpty(emp.State) ? "FL" : emp.State);
                    row.Add(emp.Zip);
                    row.Add("United States");
                    row.Add("1000001");
                    row.Add("en-US");

                    // 6 custom label/value pairs (12 columns)
                    for (int i = 0; i < customFieldCount * 2; i++)
                    {
                        row.Add("");
                    }

                    var voicePhones = empPhones[emp.EmployeeId]
                        .Where(p => p.SwnCall)
                        .OrderByDescending(p => p.IsMain)
                        .ToList();

                    for (int i = 0; i < maxPhones; i++)
                    {
                        if (i < voicePhones.Count)
                        {
                            var p = voicePhones[i];
                            row.Add(string.IsNullOrEmpty(p.PhoneType) ? "Other" : p.PhoneType);
                            row.Add("1");
                            row.Add(p.PhoneNumber);
                            row.Add(p.SwnExcludeExtension ? "" : (p.Extension ?? ""));
                            row.Add(p.PhoneCascade.HasValue ? p.PhoneCascade.Value.ToString() : "");
                        }
                        else
                        {
                            row.Add(""); row.Add(""); row.Add(""); row.Add(""); row.Add("");
                        }
                    }

                    row.Add(string.IsNullOrEmpty(emp.Email) ? "" : "Work");
                    row.Add(emp.Email ?? "");
                    row.Add(string.IsNullOrEmpty(emp.PersonalEmail) ? "" : "Home");
                    row.Add(emp.PersonalEmail ?? "");

                    var smsPhones = empPhones[emp.EmployeeId]
                        .Where(p => p.SwnText && !string.IsNullOrEmpty(p.PhoneType)
                            && (p.PhoneType.IndexOf("mobile", StringComparison.OrdinalIgnoreCase) >= 0
                                || p.PhoneType.IndexOf("cell", StringComparison.OrdinalIgnoreCase) >= 0))
                        .ToList();

                    for (int i = 0; i < maxSms; i++)
                    {
                        if (i < smsPhones.Count)
                        {
                            var p = smsPhones[i];
                            row.Add(p.PhoneType);
                            row.Add("1");
                            row.Add("1" + p.PhoneNumber);
                        }
                        else
                        {
                            row.Add(""); row.Add(""); row.Add("");
                        }
                    }

                    row.Add(""); // BB PIN LABEL
                    row.Add(""); // BB PIN

                    sb.AppendLine(string.Join("|", row.Select(Sanitize)));
                }

                var content = sb.ToString();

                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition", "attachment; filename=swn-list.txt");
                Response.Write(content);
                Response.End();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            return value.Replace("|", "/").Replace("\r", " ").Replace("\n", " ");
        }
    }
}
