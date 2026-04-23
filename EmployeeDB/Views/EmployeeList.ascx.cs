/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.EmployeeDB.Components.Controllers;
using tjc.Modules.EmployeeDB.Components.Models;
using tjc.Modules.EmployeeDB.Components.SWN;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class EmployeeList : EmployeeDBModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl);
                    return;
                }

                if (!IsPostBack)
                {
                    BindAllGrids();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void BindAllGrids()
        {
            BindEmployees();
            BindJobGroups();
            BindJobClasses();
            BindRaces();
            BindLocations();
            BindAssignedItems();
        }

        #region Employees

        private void BindEmployees()
        {
            var ctrl = new EmployeeController();
            rptEmployees.DataSource = ctrl.GetAll()
                                          .OrderBy(x => x.LastName)
                                          .ThenBy(x => x.FirstName)
                                          .ToList();
            rptEmployees.DataBind();
        }

        #endregion

        #region Job Groups (Categories)

        private void BindJobGroups()
        {
            var ctrl = new JobGroupController();
            rptJobGroups.DataSource = ctrl.GetAll().OrderBy(x => x.Description).ToList();
            rptJobGroups.DataBind();
        }

        protected void cmdSaveJobGroup_Click(object sender, EventArgs e)
        {
            try
            {
                var ctrl = new JobGroupController();
                var id = Convert.ToInt32(hdJobGroupId.Value);

                if (id > 0)
                {
                    var item = ctrl.GetById(id);
                    if (item != null)
                    {
                        item.Description = txtJobGroupDescription.Text.Trim();
                        ctrl.Update(item, UserId);
                    }
                }
                else
                {
                    var item = new JobGroupInfo
                    {
                        Description = txtJobGroupDescription.Text.Trim()
                    };
                    ctrl.Create(item, UserId);
                }

                hdJobGroupId.Value = "0";
                txtJobGroupDescription.Text = "";
                BindJobGroups();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptJobGroups_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new JobGroupController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetById(id);
                if (item != null)
                {
                    hdJobGroupId.Value = item.JobGroupId.ToString();
                    txtJobGroupDescription.Text = item.Description;
                    ScriptManager.RegisterStartupScript(upJobGroups, upJobGroups.GetType(), "showModal", "ShowModal('JobGroupEditModal');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.Delete(id);
                BindJobGroups();
            }
        }

        #endregion

        #region Job Classes

        private void BindJobClasses()
        {
            var ctrl = new JobClassController();
            rptJobClasses.DataSource = ctrl.GetAll().OrderBy(x => x.ClassName).ToList();
            rptJobClasses.DataBind();
        }

        protected void cmdSaveJobClass_Click(object sender, EventArgs e)
        {
            try
            {
                var ctrl = new JobClassController();
                var id = Convert.ToInt32(hdJobClassId.Value);

                JobClassInfo item;
                if (id > 0)
                {
                    item = ctrl.GetById(id);
                    if (item == null) return;
                }
                else
                {
                    item = new JobClassInfo();
                }

                item.ClassName = txtClassName.Text.Trim();
                item.ClassCode = ParseInt(txtClassCode.Text) ?? 0;
                item.PayGrade = ParseInt(txtPayGrade.Text);
                item.FLSA = txtFLSA.Text.Trim();
                item.EEO = ParseInt(txtEEO.Text);
                item.MMin = ParseDecimal(txtMMin.Text);
                item.MMax = ParseDecimal(txtMMax.Text);
                item.AMin = ParseDecimal(txtAMin.Text);
                item.AMax = ParseDecimal(txtAMax.Text);

                if (id > 0)
                    ctrl.Update(item, UserId);
                else
                    ctrl.Create(item, UserId);

                hdJobClassId.Value = "0";
                BindJobClasses();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptJobClasses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new JobClassController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetById(id);
                if (item != null)
                {
                    hdJobClassId.Value = item.ClassId.ToString();
                    txtClassName.Text = item.ClassName;
                    txtClassCode.Text = item.ClassCode.ToString();
                    txtPayGrade.Text = item.PayGrade.HasValue ? item.PayGrade.Value.ToString() : "";
                    txtFLSA.Text = item.FLSA;
                    txtEEO.Text = item.EEO.HasValue ? item.EEO.Value.ToString() : "";
                    txtMMin.Text = item.MMin.HasValue ? item.MMin.Value.ToString() : "";
                    txtMMax.Text = item.MMax.HasValue ? item.MMax.Value.ToString() : "";
                    txtAMin.Text = item.AMin.HasValue ? item.AMin.Value.ToString() : "";
                    txtAMax.Text = item.AMax.HasValue ? item.AMax.Value.ToString() : "";
                    ScriptManager.RegisterStartupScript(upJobClasses, upJobClasses.GetType(), "showModal", "ShowModal('JobClassEditModal');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.Delete(id);
                BindJobClasses();
            }
        }

        #endregion

        #region Races

        private void BindRaces()
        {
            var ctrl = new RaceController();
            rptRaces.DataSource = ctrl.GetAll().OrderBy(x => x.RaceCode).ToList();
            rptRaces.DataBind();
        }

        protected void cmdSaveRace_Click(object sender, EventArgs e)
        {
            try
            {
                var ctrl = new RaceController();
                var id = Convert.ToInt32(hdRaceId.Value);

                if (id > 0)
                {
                    var item = ctrl.GetById(id);
                    if (item != null)
                    {
                        item.RaceCode = txtRaceCode.Text.Trim();
                        item.Description = txtRaceDescription.Text.Trim();
                        ctrl.Update(item, UserId);
                    }
                }
                else
                {
                    var item = new RaceInfo
                    {
                        RaceCode = txtRaceCode.Text.Trim(),
                        Description = txtRaceDescription.Text.Trim()
                    };
                    ctrl.Create(item, UserId);
                }

                hdRaceId.Value = "0";
                txtRaceCode.Text = "";
                txtRaceDescription.Text = "";
                BindRaces();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptRaces_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new RaceController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetById(id);
                if (item != null)
                {
                    hdRaceId.Value = item.RaceId.ToString();
                    txtRaceCode.Text = item.RaceCode;
                    txtRaceDescription.Text = item.Description;
                    ScriptManager.RegisterStartupScript(upRaces, upRaces.GetType(), "showModal", "ShowModal('RaceEditModal');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.Delete(id);
                BindRaces();
            }
        }

        #endregion

        #region Office Locations

        private void BindLocations()
        {
            var ctrl = new OfficeLocationController();
            rptLocations.DataSource = ctrl.GetAll().OrderBy(x => x.Description).ToList();
            rptLocations.DataBind();
        }

        protected void cmdSaveLocation_Click(object sender, EventArgs e)
        {
            try
            {
                var ctrl = new OfficeLocationController();
                var id = Convert.ToInt32(hdLocationId.Value);

                if (id > 0)
                {
                    var item = ctrl.GetById(id);
                    if (item != null)
                    {
                        item.Description = txtLocationDescription.Text.Trim();
                        item.Address = txtLocationAddress.Text.Trim();
                        item.City = txtLocationCity.Text.Trim();
                        item.State = txtLocationState.Text.Trim();
                        item.Zip = txtLocationZip.Text.Trim();
                        ctrl.Update(item, UserId);
                    }
                }
                else
                {
                    var item = new OfficeLocationInfo
                    {
                        Description = txtLocationDescription.Text.Trim(),
                        Address = txtLocationAddress.Text.Trim(),
                        City = txtLocationCity.Text.Trim(),
                        State = txtLocationState.Text.Trim(),
                        Zip = txtLocationZip.Text.Trim()
                    };
                    ctrl.Create(item, UserId);
                }

                hdLocationId.Value = "0";
                txtLocationDescription.Text = "";
                txtLocationAddress.Text = "";
                txtLocationCity.Text = "";
                txtLocationState.Text = "";
                txtLocationZip.Text = "";
                BindLocations();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptLocations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new OfficeLocationController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetById(id);
                if (item != null)
                {
                    hdLocationId.Value = item.OfficeLocationId.ToString();
                    txtLocationDescription.Text = item.Description;
                    txtLocationAddress.Text = item.Address;
                    txtLocationCity.Text = item.City;
                    txtLocationState.Text = item.State;
                    txtLocationZip.Text = item.Zip;
                    ScriptManager.RegisterStartupScript(upLocations, upLocations.GetType(), "showModal", "ShowModal('LocationEditModal');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.Delete(id);
                BindLocations();
            }
        }

        #endregion

        #region Assigned Items

        private void BindAssignedItems()
        {
            var itemCtrl = new AssignedItemController();
            var empCtrl = new EmployeeController();

            var employees = empCtrl.GetAll().ToDictionary(x => x.EmployeeId, x => x.DisplayName);
            var items = itemCtrl.GetAll().ToList();

            var view = items.Select(i => new
            {
                i.ItemId,
                i.EmployeeId,
                EmployeeName = employees.ContainsKey(i.EmployeeId) ? employees[i.EmployeeId] : "",
                i.ItemType,
                i.ItemName
            })
            .OrderBy(x => x.EmployeeName)
            .ThenBy(x => x.ItemType)
            .ToList();

            rptAssignedItems.DataSource = view;
            rptAssignedItems.DataBind();
        }

        #endregion

        #region SWN Operations

        protected void cmdShowMissingSWNContacts_Click(object sender, EventArgs e)
        {
            try
            {
                var swn = new SWNServiceRequests();
                var swnIds = swn.GetContactIds() ?? new List<int>();
                var swnSet = new HashSet<int>(swnIds);

                var empCtrl = new EmployeeController();
                var activeEmployees = empCtrl.GetActive()
                                             .Where(x => x.UserId.HasValue && x.UserId.Value > 0)
                                             .ToList();

                var missing = activeEmployees
                    .Where(x => !swnSet.Contains(x.UserId.Value))
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ToList();

                if (missing.Count == 0)
                {
                    ShowMessage("All active employees have corresponding SWN contacts.", false);
                    return;
                }

                var sb = new StringBuilder();
                sb.Append("<strong>Missing SWN Contacts (");
                sb.Append(missing.Count);
                sb.Append("):</strong><ul class=\"mb-0\">");
                foreach (var m in missing)
                {
                    sb.Append("<li>");
                    sb.Append(Server.HtmlEncode(m.DisplayName));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
                ShowMessage(sb.ToString(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSWNSync_Click(object sender, EventArgs e)
        {
            try
            {
                var swn = new SWNServiceRequests();
                var response = swn.BlockUpdateContacts();
                ShowSWNResponse("SWN Sync", response);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdAddAllGroups_Click(object sender, EventArgs e)
        {
            try
            {
                var swn = new SWNServiceRequests();
                var response = swn.AddAllGroups();
                ShowSWNResponse("Add All Groups", response);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void ShowSWNResponse(string header, SWNResponse response)
        {
            if (response == null)
            {
                ShowMessage(header + ": no response returned.", false);
                return;
            }

            var sb = new StringBuilder();
            sb.Append("<strong>");
            sb.Append(Server.HtmlEncode(header));
            sb.Append(":</strong>");
            if (response.MessageList != null && response.MessageList.Count > 0)
            {
                sb.Append("<ul class=\"mb-0\">");
                foreach (var m in response.MessageList)
                {
                    sb.Append("<li>");
                    sb.Append("[");
                    sb.Append(m.MessageType);
                    sb.Append("] ");
                    sb.Append(Server.HtmlEncode(m.MessageText ?? ""));
                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }
            else
            {
                sb.Append(" completed.");
            }

            ShowMessage(sb.ToString(), !response.HasErrors);
        }

        private void ShowMessage(string html, bool success)
        {
            ltMessage.Text = html;
            pnlMessage.CssClass = success ? "alert alert-success" : "alert alert-danger";
            pnlMessage.Visible = true;
        }

        #endregion

        #region Helpers

        private static int? ParseInt(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (int.TryParse(s.Trim(), out int v)) return v;
            return null;
        }

        private static decimal? ParseDecimal(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (decimal.TryParse(s.Trim(), out decimal v)) return v;
            return null;
        }

        #endregion
    }
}
