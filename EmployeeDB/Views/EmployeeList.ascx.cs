/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class EmployeeList : EmployeeDBModuleBase
    {
        private Dictionary<int, string> _locationNameCache;
        private Dictionary<int, string> _departmentNameCache;
        private readonly Dictionary<int, string> _photoUrlCache = new Dictionary<int, string>();

        /// <summary>Per-request cache of OfficeLocationId → Description so
        /// the row template doesn't re-hit the controller for each repeater row.</summary>
        private Dictionary<int, string> LocationNameCache
        {
            get
            {
                if (_locationNameCache == null)
                {
                    _locationNameCache = new OfficeLocationController()
                        .GetAll()
                        .ToDictionary(l => l.OfficeLocationId, l => l.Description);
                }
                return _locationNameCache;
            }
        }

        /// <summary>Per-request cache of DepartmentId → GroupName (departments
        /// live in tjc_gl_group). Same shape as the location cache so the
        /// repeater row template stays cheap.</summary>
        private Dictionary<int, string> DepartmentNameCache
        {
            get
            {
                if (_departmentNameCache == null)
                {
                    _departmentNameCache = new GroupController()
                        .GetAll()
                        .ToDictionary(g => g.GroupID, g => g.GroupName);
                }
                return _departmentNameCache;
            }
        }

        /// <summary>Repeater binding helper: maps an office-location id to its description.</summary>
        protected string GetLocationName(object idValue)
        {
            if (idValue == null || idValue == DBNull.Value) return string.Empty;
            int id;
            if (!int.TryParse(idValue.ToString(), out id) || id <= 0) return string.Empty;
            string name;
            return LocationNameCache.TryGetValue(id, out name) ? name : string.Empty;
        }

        /// <summary>Repeater binding helper: maps a DepartmentId to its
        /// human-readable group name. The previous Department column bound
        /// directly to AgencyOfEmployment ("S" / "C" / "O"), which was
        /// neither the department nor user-meaningful.
        ///
        /// Note: GroupID = 0 is a legitimate department id in this DB (the
        /// "Technology Services 1" row has GroupID = 0 — a legacy oddity
        /// that 16 employees still reference). Only NULL / non-numeric
        /// values produce a blank cell; any int that has a matching cache
        /// entry — including 0 — resolves to its name.</summary>
        protected string GetDepartmentName(object idValue)
        {
            if (idValue == null || idValue == DBNull.Value) return string.Empty;
            int id;
            if (!int.TryParse(idValue.ToString(), out id)) return string.Empty;
            string name;
            return DepartmentNameCache.TryGetValue(id, out name) ? name : string.Empty;
        }

        /// <summary>Repeater binding helper: render a 16x16 thumbnail <img>
        /// for the employee's photo, or an empty string if they don't have one.
        ///
        /// The FileId → URL lookup goes through DNN's FileManager, which
        /// internally caches Files-table rows, but we ALSO maintain a
        /// per-request cache here so a 900-row repeater doesn't re-call the
        /// FileManager for every iteration (alt-text on the img stays
        /// per-row so screen readers report the actual employee name).</summary>
        protected string RenderEmployeePhoto(object fileIdValue, object firstName, object lastName)
        {
            if (fileIdValue == null || fileIdValue == DBNull.Value) return string.Empty;
            int id;
            if (!int.TryParse(fileIdValue.ToString(), out id) || id <= 0) return string.Empty;

            string url;
            if (!_photoUrlCache.TryGetValue(id, out url))
            {
                url = string.Empty;
                try
                {
                    var fi = FileManager.Instance.GetFile(id);
                    if (fi != null) url = FileManager.Instance.GetUrl(fi);
                }
                catch
                {
                    // Missing or inaccessible file — render no image rather
                    // than letting the exception bubble up and 500 the page.
                }
                _photoUrlCache[id] = url;
            }
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var altText = HttpUtility.HtmlAttributeEncode(
                (firstName == null ? "" : firstName.ToString().Trim()) + " " +
                (lastName == null ? "" : lastName.ToString().Trim())).Trim();
            // The <span> wrapper is fixed at 16x16 so the table layout
            // doesn't shift when the inner <img> grows on :hover (the img
            // is position:absolute inside the wrapper — see module.css).
            return "<span class=\"empdb-list-photo-wrap\">" +
                   "<img src=\"" + HttpUtility.HtmlAttributeEncode(url) +
                   "\" alt=\"" + altText + "\" class=\"empdb-list-photo\" />" +
                   "</span>";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl);
                    return;
                }

                // Emit the DNN ServicesFramework AntiForgery token so the JS
                // layer can post to the Web API. This adds a hidden
                // __RequestVerificationToken input to the form.
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                if (!IsPostBack)
                {
                    BindEmployees();
                    // Job Categories / Classes / Race / Office Locations /
                    // Departments are all loaded client-side via the Web API —
                    // no server-side bind necessary.
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #region Employees

        private void BindEmployees()
        {
            // EmployeeController.GetAll() now filters to IsEmployee = 1 in the
            // data layer, so non-employee rows never reach this list. Active
            // vs. inactive filtering is done client-side via the toggle at
            // the bottom of the Employees tab (DataTables custom filter).
            var ctrl = new EmployeeController();
            rptEmployees.DataSource = ctrl.GetAll()
                                          .OrderBy(x => x.LastName)
                                          .ThenBy(x => x.FirstName)
                                          .ToList();
            rptEmployees.DataBind();
        }

        #endregion

        // Job Categories / Classes / Race / Office Locations / Departments
        // admin tabs are all driven by the Web API + Scripts/empdb-list.js.
        // No postback handlers are needed for them on this page — see:
        //   Components/Api/JobGroupsController.cs
        //   Components/Api/JobClassesController.cs
        //   Components/Api/RacesController.cs
        //   Components/Api/LocationsController.cs
        //   Components/Api/DepartmentsController.cs

        // SWN Sync / Add All Groups / Show Missing SWN Contacts are all
        // driven from the JS layer now via Components/Api/SwnController.cs.
        // The original postback handlers were removed because the Web Forms
        // postback was leaving the URL in a state DNN's BreadCrumb skin
        // object couldn't parse (e.g. /GroupId/0). See Scripts/empdb-list.js#swn.

        #region Helpers

        /// <summary>
        /// Builds the URL to the Edit page for a given employee. Used by the
        /// Add Employee button (id=0) and the per-row pencil link.
        /// </summary>
        protected string EditEmployeeUrl(int employeeId)
        {
            return _navigationManager.NavigateURL(TabId, "Edit", "mid=" + ModuleId, "EmployeeId=" + employeeId);
        }

        #endregion
    }
}
