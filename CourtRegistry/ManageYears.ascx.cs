/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Utilities.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtRegistryModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class ManageYears : CourtRegistryModuleBase
    {
        #region Methods
        private void SendEmails()
        {
            var sm = new DotNetNuke.Services.Mail.SendTokenizedBulkEmail();
            int year = System.Convert.ToInt32(drpEmailYear.SelectedValue);
            var ctl = new AttorneyController();
            DotNetNuke.UI.UserControls.TextEditor textEditor = (DotNetNuke.UI.UserControls.TextEditor)this.FindControl("txtBody");
            IEnumerable<Attorney> attorneys= ctl.GetAttorneys(drpAttorneys.SelectedValue=="1", year);
            foreach (Attorney atty in attorneys)
            {
                UserInfo user = new UserInfo()
                {
                    FirstName = atty.FirstName,
                    LastName = atty.LastName,
                    DisplayName = atty.FirstName + " " + atty.LastName,
                    Email = atty.Email
                };
                sm.AddAddressedUser(user);
            }
            sm.AddAddressedUser(UserInfo);
            sm.Subject = txtEmailSubject.Text;
            sm.Body = textEditor.RichText.Text;
            sm.BodyFormat = DotNetNuke.Services.Mail.MailFormat.Html;
            UserInfo reply = new UserInfo();
            reply.Email = "cr.noreply@jud12.flcourts.org";
            reply.FirstName = "no";
            reply.LastName = "reply";
            reply.DisplayName = "noreply";
            sm.ReplyTo = reply;
            sm.SendingUser = reply;
            sm.Priority = DotNetNuke.Services.Mail.MailPriority.Normal;
            sm.AddressMethod = DotNetNuke.Services.Mail.SendTokenizedBulkEmail.AddressMethods.Send_TO;
            sm.RemoveDuplicates = true;
            sm.Send();
        }
        private void BindLists()
        {
            var ctl = new ApplicationController();
            IEnumerable<ApplicationPeriod> periods = ctl.GetApplicationPeriods().OrderByDescending(x => x.PeriodYear); ;
            rptYears.DataSource = periods;
            rptYears.DataBind();
            drpExportYear.DataSource = periods;
            drpExportYear.DataBind();
            drpYear.DataSource = periods;
            drpYear.DataBind();
            var lCtl = new LocationController();
            drpLocations.DataSource = lCtl.GetLocations();
            drpLocations.DataBind();
            drpEmailYear.DataSource = periods;
            drpEmailYear.DataBind();
        }

        private void ClearForm()
        {
            txtModificationDeadline.Text = string.Empty;
            txtYearPeriodEnds.Text = string.Empty;
            chkAcceptingApplications.Checked = false;
        }

        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindLists();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptYears_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                Int32.TryParse(e.CommandArgument.ToString(), out int year);
                var ctl = new ApplicationController();
                ctl.DeleteApplicationPeriod(year);
                BindLists();

            }
            if (e.CommandName == "edit")
            {
                Int32.TryParse(e.CommandArgument.ToString(), out int year);
                var ctl = new ApplicationController();
                ApplicationPeriod applicationPeriod = ctl.GetApplicationPeriod(year);
                txtYearPeriodEnds.Text = applicationPeriod.PeriodYear.ToString();
                txtModificationDeadline.Text = applicationPeriod.ModificationDeadline.ToString();
                chkAcceptingApplications.Checked = applicationPeriod.AcceptingNewApplications;
                ScriptManager.RegisterStartupScript(rptYears, rptYears.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void cmdAddRecord_Click(object sender, EventArgs e)
        {
            ApplicationPeriod applicationPeriod = new ApplicationPeriod { AcceptingNewApplications = chkAcceptingApplications.Checked, ApplicationYear = Int32.Parse(txtYearPeriodEnds.Text), ModificationDeadline = DateTime.Parse(txtModificationDeadline.Text) };
            ApplicationController ctl = new ApplicationController();
            ctl.CreateApplicationPeriod(applicationPeriod);
            Response.Redirect(EditUrl("manage"), true);
        }

        protected void pnlPeriods_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }

        protected void cmdViewRegistry_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("loc", drpLocations.SelectedValue, "rpt", "yr=" + drpYear.SelectedValue));
        }

        protected void cmdJACReport_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("yr", drpExportYear.SelectedValue, "counts"), true);

        }

        protected void cmdSend_Click(object sender, EventArgs e)
        {
            DotNetNuke.UI.UserControls.TextEditor textEditor = (DotNetNuke.UI.UserControls.TextEditor)this.FindControl("txtBody");
            if (textEditor.RichText.Text == "")
            {
                ltMessage.Text = string.Format("<div class='alert alert-danger'><i class='fa fa-exclamation-circle'></i> Please enter the text of the email</div>");
                return;
            }
            SendEmails();
            ltMessage.Text = string.Format("<div class='alert alert-success'><i class='fa fa-thumbs-up'></i> Emails Sent! </div>");
        }

        #endregion
    }
}