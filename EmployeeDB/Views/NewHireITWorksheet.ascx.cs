/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    /// <summary>
    /// Code-behind for the New Hire IT Worksheet view. The page is mostly
    /// API-driven — JavaScript pulls catalog items, profiles, and submits
    /// the request via /API/NhitItems, /API/NhitProfiles, /API/NhitRequests.
    ///
    /// Server-side responsibilities here are limited to:
    ///   1. Wiring up the AntiForgery token so AJAX calls authenticate.
    ///   2. Surfacing whether the current user is a site admin so the
    ///      "Manage Items" / profile-edit buttons can hide for clerks.
    ///   3. Pre-populating the form when arriving via the new-hire flow
    ///      (?EmployeeId=N) — fetches the saved employee + supervisor +
    ///      department + office location and emits them as a JS literal
    ///      so empdb-nhit.js can fill in matching fields on init.
    /// </summary>
    public partial class NewHireITWorksheet : EmployeeDBModuleBase
    {
        // Lazily-instantiated controllers used to gather the preload data.
        private readonly EmployeeController _employees = new EmployeeController();
        private readonly GroupController _groups = new GroupController();
        private readonly OfficeLocationController _locations = new OfficeLocationController();

        /// <summary>
        /// Emitted as a JS literal in the .ascx so the front-end can hide
        /// admin-only buttons (Save / Update / Delete profile, Manage Items)
        /// from non-admins. Server-side endpoints still re-check, this is
        /// just the UI affordance.
        /// </summary>
        public string IsAdminFlagJs
        {
            get { return IsSiteAdmin ? "true" : "false"; }
        }

        /// <summary>
        /// Cached preload payload. Built once per request in <see cref="BuildPreload"/>
        /// and either rendered as a JS object literal (when the URL carries
        /// an ?EmployeeId=N) or emitted as <c>null</c> (when the user opened
        /// a blank worksheet directly).
        /// </summary>
        public string PreloadJson { get; private set; } = "null";

        /// <summary>True when an employee record drove the preload — used by
        /// the .ascx to emit a "Just-saved {Name}" banner above the form.</summary>
        public bool HasPreload { get; private set; }
        public string PreloadEmployeeName { get; private set; }

        /// <summary>
        /// URL of the EmployeeList view (the module's default / "main" view).
        /// Emitted as a JS string literal so the front-end can navigate back
        /// after a successful Submit / Save as Profile.
        /// </summary>
        public string MainViewUrlJson
        {
            get
            {
                var url = _navigationManager.NavigateURL(TabId, string.Empty, "mid=" + ModuleId);
                return JsonConvert.SerializeObject(url ?? string.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Required so DnnApiController endpoints with
                // [ValidateAntiForgeryToken] accept our AJAX requests.
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                BuildPreload();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// If the URL has ?EmployeeId=N, fetch the matching employee plus the
        /// related supervisor / department / office-location lookup rows, map
        /// the fields onto the worksheet's shape, and stash the result on
        /// <see cref="PreloadJson"/> so the .ascx can emit it for the JS.
        /// Anything missing or unparseable just leaves the worksheet blank.
        /// </summary>
        private void BuildPreload()
        {
            var qs = Request.QueryString["EmployeeId"];
            if (string.IsNullOrEmpty(qs)) return;
            int empId;
            if (!int.TryParse(qs, out empId) || empId <= 0) return;

            var emp = _employees.GetEmployee(empId);
            if (emp == null) return;

            var preload = new Dictionary<string, object>();

            // Display name = "First M. Last" — same shape as legacy worksheet.
            var name = (emp.FirstName ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(emp.MiddleInitial))
            {
                if (name.Length > 0) name += " ";
                name += emp.MiddleInitial.Trim().TrimEnd('.') + ".";
            }
            if (!string.IsNullOrWhiteSpace(emp.LastName))
            {
                if (name.Length > 0) name += " ";
                name += emp.LastName.Trim();
            }
            preload["EmployeeName"] = name;
            PreloadEmployeeName = name;

            // Position Title — JobTitle is the long-form, Position is the
            // short-form / classification. Prefer JobTitle when both exist.
            var positionTitle = !string.IsNullOrWhiteSpace(emp.JobTitle)
                ? emp.JobTitle
                : emp.Position;
            if (!string.IsNullOrWhiteSpace(positionTitle))
                preload["PositionTitle"] = positionTitle.Trim();

            // Supervisor — look up the EmployeeInfo row, prefer DisplayName.
            if (emp.SupervisorId.HasValue && emp.SupervisorId.Value > 0)
            {
                var supervisor = _employees.GetEmployee(emp.SupervisorId.Value);
                if (supervisor != null && !string.IsNullOrWhiteSpace(supervisor.DisplayName))
                {
                    preload["SupervisorName"] = supervisor.DisplayName;
                }
            }

            // Department — tjc_gl_group lookup by DepartmentId.
            if (emp.DepartmentId.HasValue && emp.DepartmentId.Value > 0)
            {
                var dept = _groups.GetById(emp.DepartmentId.Value);
                if (dept != null && !string.IsNullOrWhiteSpace(dept.GroupName))
                {
                    preload["DepartmentUnitGroup"] = dept.GroupName;
                }
            }

            // Building / Location — best-effort match from OfficeLocation
            // description into the worksheet's fixed checkbox set.
            if (emp.OfficeLocationId.HasValue && emp.OfficeLocationId.Value > 0)
            {
                var loc = _locations.GetById(emp.OfficeLocationId.Value);
                var building = MapOfficeLocationToBuilding(loc?.Description);
                if (!string.IsNullOrEmpty(building))
                    preload["BuildingLocation"] = building;
            }

            // Employee Type — the rblAgency on EditEmployee stores S/C/O.
            switch ((emp.AgencyOfEmployment ?? string.Empty).Trim().ToUpperInvariant())
            {
                case "S": preload["EmployeeType"] = "State"; break;
                case "C": preload["EmployeeType"] = "County"; break;
                case "O": preload["EmployeeType"] = "Other"; break;
            }

            // Effective Date — HireDate is the closest match. Render in
            // ISO 8601 (yyyy-MM-dd) so <input type="date"> picks it up
            // without parsing.
            if (emp.HireDate.HasValue)
            {
                preload["EffectiveDate"] = emp.HireDate.Value.ToString("yyyy-MM-dd");
            }
            // Temp/Intern End — only populate if the employee record actually
            // has a TerminationDate (truly temporary hires).
            if (emp.TerminationDate.HasValue)
            {
                preload["TempInternEndDate"] = emp.TerminationDate.Value.ToString("yyyy-MM-dd");
            }

            // EmployeeId is forwarded so the request audit row can FK back
            // to tjc_employee.EmployeeId.
            preload["EmployeeId"] = emp.EmployeeId;

            PreloadJson = JsonConvert.SerializeObject(preload);
            HasPreload = true;
        }

        /// <summary>
        /// Map a free-text office-location description (e.g. "Sarasota - CJC",
        /// "Mound Street, Bradenton") to one of the worksheet's fixed building
        /// values. Returns null when no match is confident — the user picks
        /// the radio button manually in that case.
        /// </summary>
        private static string MapOfficeLocationToBuilding(string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            var d = description.ToLowerInvariant();
            if (d.Contains("mound")) return "Mound Street";
            if (d.Contains("manatee")) return "Manatee";
            if (d.Contains("desoto") || d.Contains("de soto")) return "DeSoto";
            if (d.Contains("venice")) return "Venice";
            if (d.Contains("sarasota") || d.Contains("cjc")) return "Sarasota/CJC";
            return null;
        }
    }
}
