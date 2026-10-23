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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
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
    public partial class Edit : ProcessServerModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Edit()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!UserInfo.IsInRole(AdminRole))
                {
                    Response.Redirect(_navigationManager.NavigateURL());
                }

                string ImagePath = "Images/Pages/process-servers"; // Set default value for folder
                CheckImagePath(ImagePath);
                // Inject filepicker control into placeholder
                var userControl = Page.LoadControl("~/controls/filepickeruploader.ascx");
                DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader _fileControl = userControl as DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader;
                _fileControl.ID = "fpUploader";
                _fileControl.FolderPath = ImagePath;
                _fileControl.EnableViewState = true;
                _fileControl.ViewStateMode = System.Web.UI.ViewStateMode.Enabled;
                _fileControl.UsePersonalFolder = false;
                _fileControl.User = null;

                //Implement your edit logic for your module
                if (!Page.IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    var ctl = new ProcessServerController(_hostSettings);
                    ProcessServer.Components.ProcessServer p = ctl.GetItem(PID);
                    BindUploadList();
                    _fileControl.FileID = p.ProfileImageID;
                    txtCell.Text = p.CellPhone;
                    txtEMail.Text = p.Email;
                    txtFirstName.Text = p.FirstName;
                    txtLastName.Text = p.LastName;
                    txtMiddleInitial.Text = p.MiddelInitial;
                    txtNumber.Text = p.ProcessServerNumber.ToString();
                    txtPhone.Text = p.HomePhone;
                    txtCity.Text = p.City;
                    ltPaymentType.Text = p.PaymentType;
                    txtExpirationDate.Text = DateTime.Today.AddYears(1).ToString("MM/dd/yyyy");
                    txtZip.Text = p.ZipCode;
                    drpStatus.SelectedValue = ((int)p.Status).ToString();
                    txtNotes.Text = p.Notes;
                    if (p.ExpirationDate.HasValue)
                        txtExpirationDate.Text = p.ExpirationDate.Value.ToString("MM/dd/yyyy");
                    chkShowPhone.Checked = p.ShowPhone;
                }
                phFileUpload.Controls.Add(_fileControl);

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void updateProcessServer_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader _fileControl = phFileUpload.Controls[0] as DotNetNuke.Web.UI.WebControls.DnnFilePickerUploader;
            var ctl = new ProcessServerController(_hostSettings);
            ProcessServer.Components.ProcessServer p = ctl.GetItem(PID);
            p.ProfileImageID = _fileControl.FileID;
            p.CellPhone = txtCell.Text;
            p.Email = txtEMail.Text;
            p.FirstName = txtFirstName.Text;
            p.LastName = txtLastName.Text;
            p.MiddelInitial = txtMiddleInitial.Text;
            Int32.TryParse(txtNumber.Text, out int psNumber);
            p.ProcessServerNumber = psNumber;
            p.HomePhone = txtPhone.Text;
            p.City = txtCity.Text;
            p.ZipCode = txtZip.Text;
            p.ShowPhone = chkShowPhone.Checked;
            if (!string.IsNullOrEmpty(txtExpirationDate.Text))
                p.ExpirationDate = DateTime.Parse(txtExpirationDate.Text);
            ctl.UpdateItem(p);
            Response.Redirect(EditUrl("list"));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("list"));
        }
        private void CheckImagePath(string imagePath)
        {
            var objFolder = DotNetNuke.Services.FileSystem.FolderManager.Instance;
            if (objFolder.FolderExists(PortalId, imagePath) == false)
            {
                objFolder.AddFolder(PortalId, imagePath);
            }
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            var ctl = new ProcessServerController(_hostSettings);
            ProcessServer.Components.ProcessServer p = ctl.GetItem(PID);
            p.Status = (applicationStatus)Int32.Parse(drpStatus.SelectedValue);
            if (p.Status == applicationStatus.New)
            {
                p.ApprovedByChiefJudge = null;
                p.ApprovedByCourtCounsel = null;
                p.CriminalHistoryCompleted = null;
            }
            if (p.Status == applicationStatus.CriminalHistoryComplete)
            {
                p.ApprovedByCourtCounsel = null;
                p.ApprovedByChiefJudge = null;
                p.CriminalHistoryCompleted = DateTime.Now;
            }
            if (p.Status == applicationStatus.CourtCounselRejected)
            {
                p.ApprovedByCourtCounsel = null;
                p.ApprovedByChiefJudge = null;
            }
            if (p.Status == applicationStatus.ChiefJudgeRejected)
            {
                p.ApprovedByChiefJudge = null;
            }
            if (p.Status == applicationStatus.CourtCounselApproved)
            {
                if (!p.CriminalHistoryCompleted.HasValue)
                    p.CriminalHistoryCompleted = DateTime.Now;
                p.ApprovedByChiefJudge = null;
                p.ApprovedByCourtCounsel = DateTime.Now;
            }
            if (p.Status == applicationStatus.ChiefJudgeApproved)
            {
                if (!p.CriminalHistoryCompleted.HasValue)
                    p.CriminalHistoryCompleted = DateTime.Now;
                if (!p.ApprovedByCourtCounsel.HasValue)
                    p.ApprovedByCourtCounsel = DateTime.Now;

                p.ApprovedByChiefJudge = DateTime.Now;
            }
            if (p.Status == applicationStatus.Approved)
            {
                if (!p.ApprovedByCourtCounsel.HasValue) p.ApprovedByCourtCounsel = DateTime.Now;
                if (!p.CriminalHistoryCompleted.HasValue) p.CriminalHistoryCompleted = DateTime.Now;
                if (!p.ApprovedByChiefJudge.HasValue) p.ApprovedByChiefJudge = DateTime.Now;
            }
            ctl.UpdateItem(p);
            ltMessage1.Text = "<div class='alert alert-success alert-dismissible fade show' role=alert><i class='fas fa-thumbs-up'></i> <strong>Status Updated</strong><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
        }

        private void SendNotification(bool rejected, Components.ProcessServer p)
        {
            try
            {
                string subject = "Process Server Application ";
                string body = "";

                if (rejected)
                {
                    subject += "Rejected";
                    body = string.Format("<p>Your Application has been Rejected.</p><p>{0}</p>", txtRejectionNotification.Text);
                }
                else
                {
                    subject += "Approved";
                    body = string.Format("<p>Your Application has been Approved.</p><p>{0}</p>", txtRejectionNotification.Text);
                }
                string link = ApplicantLink(p.IsRenewal);
                if (!string.IsNullOrEmpty(link))
                    body += string.Format("<p><a href='{0}'>View your application</a></p>", link);
                DotNetNuke.Services.Mail.Mail.SendEmail(NotificationEmail, p.Email, subject, body);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            var ctl = new ProcessServerController(_hostSettings);
            ProcessServer.Components.ProcessServer p = ctl.GetItem(PID);
            p.Status = applicationStatus.Approved;
            ctl.UpdateItem(p);
            SendNotification(false, p);
            ltMessage1.Text = "<div class='alert alert-success alert-dismissible fade show' role=alert><i class='fas fa-thumbs-up'></i> <strong>Application Approved. Email sent to User</strong><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            var ctl = new ProcessServerController(_hostSettings);
            ProcessServer.Components.ProcessServer p = ctl.GetItem(PID);
            p.Status = applicationStatus.Rejected;
            ctl.UpdateItem(p);
            SendNotification(true, p);
            ltMessage1.Text = "<div class='alert alert-warning alert-dismissible fade show' role=alert><i class='fas fa-thumbs-down'></i> <strong>Application Rejected. Email sent to User</strong><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
        }

        protected void btnSaveFile_Click(object sender, EventArgs e)
        {
            foreach (HttpPostedFile file in fuUpload.PostedFiles)
            {
                string filename = file.FileName;
                InsertAttachment(file, filename, PID);
            }
            BindUploadList();
            ClientScriptManager cs = Page.ClientScript;
            cs.RegisterStartupScript(this.GetType(), "AKey", "SetTab();", true);
        }

        protected void rptUploades_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                UploadedFile f = (UploadedFile)e.Item.DataItem;
                HyperLink lnkFile = (HyperLink)e.Item.FindControl("lnkFile");
                lnkFile.NavigateUrl = TemplateSourceDirectory + "/FileViewer.ashx?aid=" + f.AttachmentID.ToString();
                LinkButton cmdHide = (LinkButton)e.Item.FindControl("cmdHide");
                cmdHide.Text = f.Hidden ? "<em data-label='Active' class='fa fa-check-square'></em>" : "<em data-label='InActive' class='fa fa-square'></em>";

            }
        }

        protected void rptUploades_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {

            int attachmentId = Int32.Parse(e.CommandArgument.ToString());
            if (e.CommandName.ToLower() == "hide")
            {
                FileUploadController ctl = new FileUploadController(_hostSettings);
                UploadedFile attachment = ctl.GetFileUpload(attachmentId);
                attachment.Hidden = !attachment.Hidden;
                ctl.UpdateFileUpload(attachment);
                BindUploadList();
            }
            if (e.CommandName.ToLower() == "delete")
            {
                FileUploadController ctl = new FileUploadController(_hostSettings);
                ctl.DeleteFileUpload(attachmentId);
                BindUploadList();
            }
        }
        protected void rptUploades_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RepeaterItem item = e.Item;
                var sm = ScriptManager.GetCurrent(Page);
                LinkButton cmdHide = (LinkButton)item.FindControl("cmdHide");
                LinkButton cmdDelete = (LinkButton)item.FindControl("cmdDelete");
                sm.RegisterAsyncPostBackControl(cmdHide);
                sm.RegisterAsyncPostBackControl(cmdDelete);
            }
        }
        #endregion

        #region Methods
        private void BindUploadList()
        {
            var ctl = new FileUploadController(_hostSettings);
            rptUploades.DataSource = ctl.GetFileUploadsByProcessServerId(PID);
            rptUploades.DataBind();
        }
        public int InsertAttachment(HttpPostedFile file, string filename, int processServerId)
        {
            var ctl = new FileUploadController(_hostSettings);
            int fileId = 0;
            string extension = Path.GetExtension(filename);
            var strings = new List<string> { ".jpg", ".jpeg", ".pdf" };
            bool validExtension = strings.Contains(extension, StringComparer.OrdinalIgnoreCase);
            if (validExtension)
            {
                try
                {
                    UploadedFile uf = new UploadedFile
                    {
                        DateUploaded = DateTime.Now,
                        ProcessServerID = processServerId,
                        FullPath = "",
                        Hidden = chkHide.Checked,
                    };
                    ctl.CreateFileUpload(uf);
                    fileId = uf.AttachmentID;
                    uf.FullPath = processServerId.ToString() + "_" + fileId.ToString() + "_" + filename.Replace(" ", "_");
                    uf.FileData = ReadToEnd(file.InputStream);
                    ctl.UpdateFileUpload(uf);
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                    if (fileId > 0)
                    {
                        ctl.DeleteFileUpload(fileId);
                    }
                }
            }
            return fileId;
        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        protected void btnSendChiefJudge_Click(object sender, EventArgs e)
        {
            string subject = "Review Process Server Application";
            string link = EditUrl("pid", PID.ToString(), "review");
            string body = string.Format("<p>Please review the following application.</p><p><a href='{0}'>Review Application</a></p>", link);
            var rCtl = RoleController.Instance;
            var users = rCtl.GetUsersByRole(PortalId, ChiefJudgeRole);
            foreach (var user in users)
            {
                if (user.IsSuperUser == false && user.IsAdmin == false)
                    DotNetNuke.Services.Mail.Mail.SendEmail("noreply.processserver@jud12.flcourts.org", user.Email, subject, body);
            }

            ltMessage1.Text = "<div class='alert alert-success alert-dismissible fade show' role=alert><i class='fas fa-thumbs-up'></i> <strong>Notification Sent to Chief Judge.</strong><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";
        }

        protected void btnSendCourtCounsel_Click(object sender, EventArgs e)
        {
            string subject = "Review Process Server Application";
            string link = EditUrl("pid", PID.ToString(), "review");
            string body = string.Format("<p>Please review the following application.</p><p><a href='{0}'>Review Application</a></p>", link);
            var rCtl = RoleController.Instance;
            var users = rCtl.GetUsersByRole(PortalId, CourtCounselRole);
            foreach (var user in users)
            {
                if (user.IsSuperUser == false && user.IsAdmin == false)
                    DotNetNuke.Services.Mail.Mail.SendEmail("noreply.processserver@jud12.flcourts.org", user.Email, subject, body);
            }
            ltMessage1.Text = "<div class='alert alert-success alert-dismissible fade show' role=alert><i class='fas fa-thumbs-up'></i> <strong>Notification Sent to Court Counsel.</strong><button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button></div>";

            #endregion

        }
    }
}