/*
' Copyright (c) 2022  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using tjc.Modules.JudicialReferral.Components;
using Microsoft.Extensions.DependencyInjection;
namespace tjc.Modules.JudicialReferral
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from JudicialReferralModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Referral : JudicialReferralModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Referral()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    SetTargetFolder();
                    txtYear.Attributes.Add("placeholder", DateTime.Now.Year.ToString());
                    PopulateJudgeList();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void PopulateJudgeList()
        {
            DotNetNuke.Security.Roles.RoleController rCtl = new DotNetNuke.Security.Roles.RoleController();
            var judgeList = DotNetNuke.Security.Roles.RoleController.Instance.GetUsersByRole(PortalId, JudgeRole);

            drpJudge.AppendDataBoundItems = true;
            drpJudge.DataTextField = "DisplayName";
            drpJudge.DataValueField = "UserId";
            drpJudge.DataSource = judgeList.OrderBy(jud => jud.DisplayName);
            drpJudge.DataBind();
        }

        protected void SetTargetFolder()
        {
            if (!DotNetNuke.Services.FileSystem.FolderManager.Instance.FolderExists(PortalId, TargetFolder))
                DotNetNuke.Services.FileSystem.FolderManager.Instance.AddFolder(PortalId, TargetFolder);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new JudicialReferralController();
            var aCtl = new AttachmentController();
            var AttacmentIds = hdAttachmentIds.Value.Split(',').ToList();
            Int32.TryParse(drpJudge.SelectedValue, out int judgeId);
            DateTime.TryParse(txtMotionDate.Text, out DateTime mDate);
            Int32.TryParse(txtCaseNumber.Text, out int caseInt);

            string caseNumber = string.Format("{0}-{1}-{2}-{3:000000}", drpCounty.SelectedValue, txtYear.Text, txtCaseType.Text, caseInt);
            var objReferral = new Components.JudicialReferral
            {
                JaID = UserId,
                JudgeID = judgeId,
                JaCreatedDate = DateTime.Now,
                CaseParties = txtCaseParties.Text,
                CaseNumber = caseNumber,
                MotionTitle = txtMotionTitle.Text,
                CaseType = drpCaseType.SelectedValue,
                MotionVacate = chkMotionVacate.Checked,
                MotionCorrect = chkMotionCorrect.Checked,
                MotionDirected = chkMotionDirected.Checked,
                MotionOther = chkMotionOther.Checked,
                MotionDate = mDate,
                Status = Components.JudicialReferral.Statuses.NewReferral
            };
            string directedMotions = "";
            foreach (ListItem item in clsMotionList.Items)
            {
                if ((item.Selected))
                    directedMotions += item.Value + "|";
            }

            objReferral.DirectedMotions = directedMotions.Trim('|');
            ctl.CreateReferral(objReferral);
            foreach (string id in AttacmentIds)
            {
                var attachment = aCtl.GetAttachment(Int32.Parse(id));
                attachment.ReferralID = objReferral.ReferralID;
                aCtl.UpdateAttachment(attachment);
            }
            SendToJudge(objReferral);
            Response.Redirect(_navigationManager.NavigateURL());
        }
        private void SendToJudge(Components.JudicialReferral objReferral)
        {
            string emailFrom = "noreply.intranet@jud12.flcourts.org";
            UserInfo user = UserController.GetUserById(PortalId, objReferral.JudgeID);
            string toEmail = user.Email;
            string subject = "New Judicial Referral Request";
            string body = string.Format("<p>Please review the <a href='{0}'>Judicial Referral Request</a> for case number {1}.</p>", EditUrl("rid", objReferral.ReferralID.ToString(), "review"), objReferral.CaseNumber);
            DotNetNuke.Services.Mail.Mail.SendEmail(emailFrom, toEmail, subject, body);
        }

    }

}