/*
' Copyright (c) 2021  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI.WebControls;
using tjc.Modules.ProcessServer.Components;

namespace tjc.Modules.ProcessServer
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from ProcessServerModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Review : ProcessServerModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Review()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindUploadList()
        {
            var ctl = new FileUploadController(_hostSettings);
            rptUploades.DataSource = ctl.GetFileUploadsByProcessServerIdYear(PID, ApplicationYear);
            rptUploades.DataBind();
        }

        private void PopulateFields()
        {

            var ctl = new ProcessServerController(_hostSettings);
            Components.ProcessServer p = ctl.GetItem(PID);
            ltPaymentType.Text = p.PaymentType;
            txtFirstName.Text = p.FirstName;
            txtMiddleInitial.Text = p.MiddelInitial;
            txtLastName.Text = p.LastName;
            txtMaidenName.Text = p.MaidenName;
            txtAliases.Text = p.Aliases;
            txtEMail.Text = p.Email;
            txtPhone.Text = p.HomePhone;
            txtCell.Text = p.CellPhone;
            txtSSN.Text = p.SocialSecurityNumber;
            if (p.DateOfBirth.HasValue)
                txtDOB.Text = p.DateOfBirth.Value.ToString("MM/dd/yyyy");
            txtBirthPlace.Text = p.BirthPlace;
            txtAddress.Text = p.StreetAddress;
            txtCity.Text = p.City;
            drpState.SelectedValue = p.State;
            txtZip.Text = p.ZipCode;
            txtCounty.Text = p.County;
            txtIdNumber.Text = p.ProcessServerNumber.ToString();
            txtHowLong.Text = p.HowLong;
            drpRace.SelectedValue = p.Race;
            rblSex.SelectedValue = p.Sex;
            txtHeight.Text = p.Height;
            txtWeight.Text = p.Weight;
            txtEyeColor.Text = p.EyeColor;
            txtHairColor.Text = p.HairColor;
            chkManatee.Checked = p.Manatee;
            chkSarasota.Checked = p.Sarasota;
            chkDeSoto.Checked = p.DeSoto;
            rblQuestion1.SelectedValue = p.Question1 ? "Yes" : "No";
            rblQuestion2.SelectedValue = p.Question2 ? "Yes" : "No";
            rblQuestion3.SelectedValue = p.Question3 ? "Yes" : "No";
            rblQuestion4.SelectedValue = p.Question4 ? "Yes" : "No";
            rblQuestion5.SelectedValue = p.Question5 ? "Yes" : "No";
            rblQuestion6.SelectedValue = p.Question6 ? "Yes" : "No";
            rblQuestion7.SelectedValue = p.Question7 ? "Yes" : "No";
            rblQuestion8.SelectedValue = p.Question8 ? "Yes" : "No";
            txtQuestion8.Text = p.Quesion8Text;
            txtQuestion6.Text = p.Question6Text;
            txtQuestion7.Text = p.Question7Text;
            rblQuestion9.SelectedValue = p.Question9 ? "Yes" : "No";
            rblQuestion10.SelectedValue = p.Question10 ? "Yes" : "No";
            rblQuestion11.SelectedValue = p.Question11 ? "Yes" : "No";
            txtQuestion11.Text = p.Question11Text;
            chkPolicy.Checked = p.Policy;
            chkAuthorize.Checked = p.Authorized;
            chkAcknowledge.Checked = p.Acknowledged;
            txtReason.Text = p.RejectionReason;
            rblChiefJudgeApproval.Enabled = false;
            rblCourtCounselApproval.Enabled = false;
            txtReason.ReadOnly = true;


            if (p.CCApproved)
            {
                rblCourtCounselApproval.Enabled = false;
                rblCourtCounselApproval.SelectedValue = "1";
            }
            else
            {
                if (UserInfo.IsInRole(CourtCounselRole))
                {
                    rblCourtCounselApproval.Enabled = true;
                    txtReason.ReadOnly = false;
                }
            }
            if (p.CJApproved)
            {
                rblChiefJudgeApproval.Enabled = false;
                rblChiefJudgeApproval.SelectedValue = "1";
                btnSubmit.Enabled = false;
            }
            else
            {
                if (UserInfo.IsInRole(ChiefJudgeRole))
                {
                    rblChiefJudgeApproval.Enabled = true;
                    txtReason.ReadOnly = false;
                }
            }

            if (p.Status == applicationStatus.CourtCounselRejected)
                rblCourtCounselApproval.SelectedValue = "0";
            if (p.Status == applicationStatus.ChiefJudgeRejected)
                rblChiefJudgeApproval.SelectedValue = "0";
            if (p.Status == applicationStatus.Rejected)
            {
                rblCourtCounselApproval.Enabled = false;
                rblChiefJudgeApproval.Enabled = false;
                btnSubmit.Enabled = false;
            }


        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {


                if (!Page.IsPostBack)
                {
                    if (UserId > 0)
                    {
                        if (UserInfo.IsInRole(CourtCounselRole) | UserInfo.IsInRole(ChiefJudgeRole) | UserInfo.IsInRole(AdminRole))
                        {
                            PopulateFields();
                            BindUploadList();
                        }
                        else
                        {
                            pnlView.Visible = false;
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "<i class='fa fa-warning'></i> You do not have permission to use this application", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        }
                    }
                    else
                    {
                        pnlView.Visible = false;
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "<i class='fa fa-info-circle'></i> You must be logged in to use this application", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.BlueInfo);
                    }
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptUploades_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                UploadedFile f = (UploadedFile)e.Item.DataItem;
                HyperLink lnkFile = (HyperLink)e.Item.FindControl("lnkFile");
                lnkFile.NavigateUrl = TemplateSourceDirectory + "/FileViewer.ashx?aid=" + f.AttachmentID.ToString();
            }

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (UserInfo.IsInRole(AdminRole))
            {
                Response.Redirect(EditUrl("list"), true);
            }
            Response.Redirect(_navigationManager.NavigateURL());

        }
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new ProcessServerController(_hostSettings);
                Components.ProcessServer p = ctl.GetItem(PID);
                if (!p.CCApproved)
                {
                    if (rblCourtCounselApproval.SelectedValue == "1")
                    {
                        p.ApprovedByCourtCounsel = DateTime.Now;
                        p.Status = applicationStatus.CourtCounselApproved;
                        SendEmailToChiefJudge(p);
                    }
                    else if (rblCourtCounselApproval.SelectedValue == "0")
                    {
                        p.Status = applicationStatus.CourtCounselRejected;
                        SendEmailToAdmin(p);
                    }
                    ctl.UpdateItem(p);
                }
                else
                {
                    if (!p.CJApproved)
                    {
                        if (rblChiefJudgeApproval.SelectedValue == "1")
                        {
                            p.ApprovedByChiefJudge = DateTime.Now;
                            p.Status = applicationStatus.ChiefJudgeApproved;
                        }
                        else if (rblChiefJudgeApproval.SelectedValue == "0")
                        {
                            p.Status = applicationStatus.ChiefJudgeRejected;
                        }
                        ctl.UpdateItem(p);
                        SendEmailToAdmin(p);
                    }
                }
                if (UserInfo.IsInRole(AdminRole))
                {
                    Response.Redirect(_navigationManager.NavigateURL());
                }

                Response.Redirect(_navigationManager.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void SendEmailToAdmin(Components.ProcessServer p)
        {
            string subject = "";
            string body = "";
            if (p.Status == applicationStatus.CourtCounselRejected)
            {
                subject = "Process Server Application Rejected by Court Counsel";
                body = string.Format("<p>The application for <strong>{0} {1}</strong> has been rejected by court counsel.</p><p><a href='{2}'>View Application</a></p><hr /><h4>Rejection Reason</h4><div>{3}</div>", p.FirstName, p.LastName, EditUrl("pid", p.ProcessServerID.ToString(), "review"), txtReason.Text);
            }
            else if (p.Status == applicationStatus.ChiefJudgeRejected)
            {
                subject = "Process Server Application Rejected by Chief Judge";
                body = string.Format("<p>The application for <strong>{0} {1}</strong> has been rejected by chief judge.</p><p><a href='{2}'>View Application</a></p><hr /><h4>Rejection Reason</h4><div>{3}</div>", p.FirstName, p.LastName, EditUrl("pid", p.ProcessServerID.ToString(), "review"), txtReason.Text);

            }
            else if (p.Status == applicationStatus.CourtCounselApproved)
            {
                subject = "Court Counsel Approved Process Server Application";
                body = string.Format("<p>The application for <strong>{0} {1}</strong> has been approved by Court Counsel.</p><p><a href='{2}'>View Application</a></p>", p.FirstName, p.LastName, EditUrl("pid", p.ProcessServerID.ToString(), "review"));

            }
            else if (p.Status == applicationStatus.ChiefJudgeApproved)
            {
                subject = "Chief Judge Approved Process Server Application";
                body = string.Format("<p>The application for <strong>{0} {1}</strong> has been approved by the Chief Judge.</p><p><a href='{2}'>View Application</a></p>", p.FirstName, p.LastName, EditUrl("pid", p.ProcessServerID.ToString(), "review"));
            }
            var rCtl = RoleController.Instance;
            var users = rCtl.GetUsersByRole(PortalId, AdminRole);
            foreach (var user in users)
            {
                if (user.IsSuperUser == false && user.IsAdmin == false)
                    DotNetNuke.Services.Mail.Mail.SendEmail("noreply.processserver@jud12.flcourts.org", user.Email, subject, body);
            }
        }

        private void SendEmailToChiefJudge(Components.ProcessServer p)
        {
            string subject = "";
            string body = "";

            subject = "Process Server Application Approval Requested";
            body = string.Format("<p>Please review the application for <strong>{0} {1}</strong>.</p><p><a href='{2}'>Review Application</a></p>", p.FirstName, p.LastName, EditUrl("pid", p.ProcessServerID.ToString(), "review"));
            var rCtl = RoleController.Instance;
            var users = rCtl.GetUsersByRole(PortalId, ChiefJudgeRole);
            foreach (var user in users)
            {
                if (user.IsSuperUser == false && user.IsAdmin == false)
                    DotNetNuke.Services.Mail.Mail.SendEmail("noreply.processserver@jud12.flcourts.org", user.Email, subject, body);
            }
        }
    }
}