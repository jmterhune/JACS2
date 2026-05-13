/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class Application : CourtRegistryModuleBase
    {
        private int _applicationId;
        private List<ApplicationJacCodeDetail> _appJacCodes = new List<ApplicationJacCodeDetail>();

        private bool IsApprover
        {
            get
            {
                if (string.IsNullOrEmpty(ApproverUsername) || UserId <= 0)
                    return false;
                return UserInfo.Username.IndexOf(ApproverUsername, StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            var qs = Request.QueryString["aid"];
            if (!string.IsNullOrEmpty(qs) && int.TryParse(qs, out int aid))
            {
                _applicationId = aid;
                if (_applicationId > 0)
                {
                    var ctl = new ApplicationController();
                    _appJacCodes = ctl.GetApplicationJacCodes(_applicationId).ToList();
                    PopulateJacCodes();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    lnkCancel.NavigateUrl = Globals.NavigateURL();
                    if (!IsApprover)
                    {
                        cmdReject.Visible = false;
                        cmdSave.Visible = false;
                        lnkCancel.CssClass = "btn btn-primary";
                        txtRejectText.Enabled = false;
                        var checkboxes = new List<CheckBox>();
                        FindTheControls(checkboxes, rootTbl);
                        foreach (var chk in checkboxes)
                            chk.Enabled = false;
                    }
                    if (_applicationId > 0)
                    {
                        var aCtl = new AttorneyController();
                        var appCtl = new ApplicationController();
                        var application = appCtl.GetApplication(_applicationId);
                        var attorney = aCtl.GetAttorney(application.AttorneyID);
                        PopulateApplication(application);
                        PopulateAttorney(attorney);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                            "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("No Application Selected. Please click Cancel to Return to the Application List.") + "', type: 'error', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateAttorney(Attorney atty)
        {
            ltName.Text = atty.LastName + ", " + atty.FirstName;
            ltBarNumber.Text = atty.BarNumber.ToString();
            ltFirm.Text = atty.LawFirm;
            ltAddress.Text = string.Format("{0}{1}{2} {3}",
                string.IsNullOrEmpty(atty.Address) ? string.Empty : atty.Address + ", ",
                string.IsNullOrEmpty(atty.City) ? string.Empty : atty.City + ", ",
                string.IsNullOrEmpty(atty.State) ? string.Empty : atty.State,
                string.IsNullOrEmpty(atty.Zip) ? string.Empty : atty.Zip);
            ltPhone.Text = atty.Phone;
            ltFax.Text = atty.Fax;
            ltCell.Text = atty.Cell;
            if (!string.IsNullOrEmpty(atty.Email))
            {
                lnkEmail.Text = atty.Email;
                lnkEmail.NavigateUrl = "mailto:" + atty.Email;
            }
            else
            {
                lnkEmail.Visible = false;
            }
            ltLanguages.Text = atty.Language;
        }

        private void PopulateApplication(Components.Application app)
        {
            ltRemoteInfo.Text = app.RemoteContactInfo;
            chkGuardian.Checked = !string.IsNullOrEmpty(app.GuardianSignature);
            chkRenewal.Checked = app.IsRenewal;
            ltYears.Text = app.YearsOnRegistry.ToString();
            txtRejectText.Text = app.RejectionText;
            string status = string.Empty;
            switch ((ApplicationStatus)app.Status)
            {
                case ApplicationStatus.New: status = "New "; break;
                case ApplicationStatus.Approved: status = "Previously Approved "; break;
                case ApplicationStatus.Archived: status = "Currently Archived "; break;
                case ApplicationStatus.Rejected: status = "Previously Rejected "; break;
                case ApplicationStatus.Updated: status = "Updated "; break;
            }
            ltHeading.Text = string.Format("<h2>{0}Application for Fiscal Year {1} - {2}</h2>", status, app.Year - 1, app.Year);
        }

        private void PopulateJacCodes()
        {
            string caseTypeName = string.Empty;
            int currentJacCode = 0;
            foreach (var j in _appJacCodes)
            {
                if (caseTypeName != j.CaseTypeName)
                {
                    caseTypeName = j.CaseTypeName;
                    var tr = new TableRow();
                    var td = new TableCell { Text = "<h3 class=\"mb-0\">" + caseTypeName + "</h3>", CssClass = "header", ColumnSpan = 2 };
                    tr.Cells.Add(td);
                    rootTbl.Rows.Add(tr);
                }

                if (currentJacCode != j.JacCodeID)
                {
                    currentJacCode = j.JacCodeID;
                    var tr = new TableRow();
                    var tdh = new TableCell();
                    var heading = new HtmlGenericControl("h4");
                    heading.Attributes["class"] = "h6 mb-0";
                    heading.InnerText = j.Category + " (" + currentJacCode + ")";
                    tdh.Controls.Add(heading);
                    tr.Cells.Add(tdh);
                    var td = new TableCell { ID = "td-" + currentJacCode, CssClass = "jac-cells" };
                    var flex = new HtmlGenericControl("div") { ID = "row-" + currentJacCode };
                    flex.Attributes["class"] = "d-flex flex-wrap gap-2 align-items-center";
                    td.Controls.Add(flex);
                    AddCheckBox(flex, j);
                    tr.Cells.Add(td);
                    rootTbl.Rows.Add(tr);
                }
                else
                {
                    var existingRow = rootTbl.FindControl("row-" + j.JacCodeID);
                    if (existingRow != null)
                        AddCheckBox(existingRow, j);
                }
            }
        }

        private void AddCheckBox(Control container, ApplicationJacCodeDetail j)
        {
            var statusName = ((CodeStatus)j.Status).ToString().ToLower();
            var chk = new CheckBox
            {
                ID = "chk-" + j.JacCodeID + "-" + j.LocationID,
                CssClass = StatusBadgeClass(j.Status) + " " + statusName,
                Text = j.LocationName
            };
            if (!IsPostBack)
            {
                if ((CodeStatus)j.Status != CodeStatus.Rejected)
                    chk.Checked = true;
                if ((CodeStatus)j.Status == CodeStatus.Locked)
                    chk.Checked = true;
            }
            if (statusName == "removed")
                chk.Text += " (removal)";
            if (statusName == "locked")
                chk.Text += " (locked)";
            container.Controls.Add(chk);
        }

        private static string StatusBadgeClass(int status)
        {
            switch ((CodeStatus)status)
            {
                case CodeStatus.New: return "badge badge-primary";
                case CodeStatus.Approved: return "badge badge-success";
                case CodeStatus.Rejected: return "badge badge-warning";
                case CodeStatus.Removed: return "badge badge-dark";
                case CodeStatus.Locked: return "badge badge-danger";
                default: return "badge badge-default";
            }
        }

        private void FindTheControls(List<CheckBox> found, Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is CheckBox cb && cb.ID != null && cb.ID.StartsWith("chk-"))
                    found.Add(cb);
                if (c.Controls.Count > 0)
                    FindTheControls(found, c);
            }
        }

        protected void cmdReject_Click(object sender, EventArgs e)
        {
            var checkboxes = new List<CheckBox>();
            FindTheControls(checkboxes, rootTbl);
            foreach (var chk in checkboxes)
            {
                chk.Checked = false;
                chk.CssClass = "rejected";
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!IsApprover)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                    "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("You do not have approval rights") + "', type: 'error', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);
                return;
            }
            var appCtl = new ApplicationController();
            var setCtl = new SettingController();
            var appSetting = setCtl.GetSettings().FirstOrDefault();

            var checkboxes = new List<CheckBox>();
            FindTheControls(checkboxes, rootTbl);
            bool hasApproval = false;
            var removedCodes = new List<ApplicationJacCodeDetail>();

            foreach (var chk in checkboxes)
            {
                var parts = chk.ID.Split('-');
                int jacCodeId = int.Parse(parts[1]);
                int locationId = int.Parse(parts[2]);
                var detail = _appJacCodes.FirstOrDefault(c => c.JacCodeID == jacCodeId && c.LocationID == locationId);
                if (detail == null) continue;
                var item = appCtl.GetApplicationJacCode(jacCodeId, locationId, _applicationId);
                if (item == null) continue;

                if (!chk.Checked)
                {
                    if (item.Status == (int)CodeStatus.Removed || item.Status == (int)CodeStatus.Locked)
                    {
                        removedCodes.Add(detail);
                        appCtl.DeleteApplicationJacCode(item);
                    }
                    else
                    {
                        item.Status = (int)CodeStatus.Rejected;
                        appCtl.UpdateApplicationJacCode(item);
                        chk.CssClass = "rejected";
                    }
                }
                else
                {
                    if (item.Status == (int)CodeStatus.Removed || item.Status == (int)CodeStatus.Locked)
                    {
                        item.Status = (int)CodeStatus.Locked;
                        chk.CssClass = "locked";
                    }
                    else
                    {
                        item.Status = (int)CodeStatus.Approved;
                        chk.CssClass = "approved";
                    }
                    appCtl.UpdateApplicationJacCode(item);
                    hasApproval = true;
                }
            }

            var application = appCtl.GetApplication(_applicationId);
            application.DateReviewed = DateTime.Now;
            application.Exported = false;
            application.RejectionText = txtRejectText.Text;
            application.Status = hasApproval ? (int)ApplicationStatus.Approved : (int)ApplicationStatus.Rejected;
            application.LastModifiedByUserId = UserId;
            appCtl.UpdateApplication(application);

            SendNotification(application, !hasApproval, appSetting, removedCodes);
            Response.Redirect(Globals.NavigateURL(), true);
        }

        private void SendNotification(Components.Application app, bool rejected, Setting appSetting, List<ApplicationJacCodeDetail> removedCodes)
        {
            var aCtl = new AttorneyController();
            var attorney = aCtl.GetAttorney(app.AttorneyID);
            if (attorney == null || string.IsNullOrEmpty(attorney.Email))
                return;

            var emailTo = attorney.Email;
            var emailCC = appSetting != null ? appSetting.ContactEmail : string.Empty;
            const string subject = "Your Court Registry Application Has Been Reviewed";

            var appCtl = new ApplicationController();
            var details = appCtl.GetApplicationJacCodes(app.ApplicationID).ToList();
            var approved = details.Where(d => d.Status == (int)CodeStatus.Approved).ToList();
            var rejectedList = details.Where(d => d.Status == (int)CodeStatus.Rejected).ToList();
            var locked = details.Where(d => d.Status == (int)CodeStatus.Locked).ToList();

            var sb = new StringBuilder();
            sb.AppendFormat("<p>Dear {0} {1},</p>", attorney.FirstName, attorney.LastName);
            sb.AppendFormat("<p>The Court Administrator has reviewed your application for the 12<sup>th</sup> Circuit Court Registry.<br />See results for Application #{0} below.</p>", app.ApplicationID);

            AppendCodeList(sb, "Approved Case Types:", approved);
            AppendCodeList(sb, "Rejected Case Types:", rejectedList);
            if (removedCodes != null && removedCodes.Count > 0)
            {
                AppendCodeList(sb, "Approved for Removal*:", removedCodes);
                sb.Append("<p>*These codes have been removed from your application. You will no longer receive requests to handle these case types.</p>");
            }
            if (locked.Count > 0)
            {
                AppendCodeList(sb, "Rejected for Removal*:", locked);
                sb.Append("<p>*The Court Administrator rejected your request to remove these codes from your application. You will continue to receive requests to handle these case types.</p>");
            }
            if (!string.IsNullOrEmpty(app.RejectionText))
            {
                sb.AppendFormat("<p>These items have been denied for the following reason(s):<br /><span class='ms-3 d-block'>{0}</span></p><p>Thank You.<br /></p>", app.RejectionText);
            }

            DotNetNuke.Services.Mail.Mail.SendEmail("cr.noreply@jud12.flcourts.org", emailTo, subject, sb.ToString());
            if (!string.IsNullOrEmpty(emailCC))
            {
                sb.Insert(0, string.Format("<p><strong>The following notification was sent on {0}</strong></p><hr>", DateTime.Now.ToShortDateString()));
                DotNetNuke.Services.Mail.Mail.SendEmail("cr.noreply@jud12.flcourts.org", emailCC, "Application Review Notification", sb.ToString());
            }
        }

        private void AppendCodeList(StringBuilder sb, string heading, List<ApplicationJacCodeDetail> list)
        {
            if (list == null || list.Count == 0) return;
            sb.AppendFormat("<strong>{0}</strong><ul>", heading);
            foreach (var c in list)
                sb.AppendFormat("<li>{0}&nbsp;({1})&nbsp;for&nbsp;{2}</li>", c.Category, c.JacCodeID, c.LocationName);
            sb.Append("</ul>");
        }
    }
}
