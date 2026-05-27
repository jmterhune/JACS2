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

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Claims;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.RecordDestruction.Components;

namespace tjc.Modules.RecordDestruction
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from Record_DestructionModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : RecordDestructionModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindLists()
        {
            var dmCtl = new DestructionMethodController();
            var gCtl=new GroupController();
            var rtCtl=new RecordTypeController();
            var rpCtl=new RetentionPeriodController();

            drpDepartment.DataSource = gCtl.GetGroups();
            drpDepartment.DataBind();
            drpDestructionMethod.DataSource=dmCtl.GetDestructionMethods();
            drpDestructionMethod.DataBind();
            drpRecordType.DataSource=rtCtl.GetRecordTypes();
            drpRecordType.DataBind();
            drpRetentionPeriod.DataSource=rpCtl.GetRetentionPeriods();
            drpRetentionPeriod.DataBind();
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmdCancel.NavigateUrl = _navigationManager.NavigateURL();
                txtYearCreated.Attributes["max"] = DateTime.Now.Year.ToString();
                string postBackJavascript = Page.ClientScript.GetPostBackEventReference(cmdSave,"");

                if (UserId > 0)
                {
                    txtName.Text = UserInfo.DisplayName;
                    if (IsAdmin)
                    {
                        phAdminTabs.Visible = true;
                    }
                    BindLists();
                }
                else
                {
                    cmdSave.Enabled = false;
                    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "msg" + Guid.NewGuid().ToString("N"),
                        "new Noty({ text: '" + System.Web.HttpUtility.JavaScriptStringEncode("You must be logged in to use this form") + "', type: 'error', timeout: 4500, layout: 'topRight', theme: 'mint' }).show();", true);

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new LogController();
            Log log = new Log();
            int fileId = 0;

            if (ctlLogFile.HasFile)
            {
                IFileInfo file;
                IFolderInfo folder;
                if (FolderManager.Instance.FolderExists(PortalId, AttachmentDirectory))
                    folder = FolderManager.Instance.GetFolder(PortalId, AttachmentDirectory);
                else
                    folder = FolderManager.Instance.AddFolder(PortalId, AttachmentDirectory);

                file = FileManager.Instance.AddFile(folder, ctlLogFile.FileName, ctlLogFile.PostedFile.InputStream);
                if (file != null)
                    fileId = file.FileId;
            }
            log.CreatedByID = UserId;
            log.CreatedDate = DateTime.Now;
            log.LastModifiedByID = UserId;
            log.LastModifiedDate = DateTime.Now;
            log.FileID = fileId;
            if (DateTime.TryParse(txtDateDestroyed.Text, out DateTime dateDestroyed))
            {
                log.DateDestroyed = dateDestroyed;
            }
            log.GroupID =Int32.Parse( drpDepartment.SelectedValue);
            log.Description = txtDescription.Text;
            log.DestructionMethodID = Int32.Parse(drpDestructionMethod.SelectedValue);
            log.RecordTypeID = Int32.Parse(drpRecordType.SelectedValue);
            log.RetentionPeriodID = Int32.Parse(drpRetentionPeriod.SelectedValue);
            if (Int32.TryParse(txtYearCreated.Text,out int yearCreated))
                log.YearCreated = yearCreated;
          ctl.CreateLog(log);
            Response.Redirect(_navigationManager.NavigateURL(),true);
        }
    }
}