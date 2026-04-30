/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class EmployeeList : EmployeeDBModuleBase
    {
        private Dictionary<int, string> _locationNameCache;

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

        /// <summary>Repeater binding helper: maps an office-location id to its description.</summary>
        protected string GetLocationName(object idValue)
        {
            if (idValue == null || idValue == DBNull.Value) return string.Empty;
            int id;
            if (!int.TryParse(idValue.ToString(), out id) || id <= 0) return string.Empty;
            string name;
            return LocationNameCache.TryGetValue(id, out name) ? name : string.Empty;
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
