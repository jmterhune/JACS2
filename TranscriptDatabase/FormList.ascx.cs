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
using DotNetNuke.Common.Lists;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.TranscriptDatabase.Components;

namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class FormList : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public FormList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindDropDowns()
        {
            var documentTypes = Enumerations.GetValues<DocumentTypes>();
            foreach (var documentType in documentTypes)
            {
                drpFileType.Items.Add(new ListItem(Enumerations.GetEnumDescription(documentType), documentType.ToString()));
            }
        }
        private void BindList()
        {
            var ctl = new FormController();
            rptForm.DataSource = ctl.GetForms();
            rptForm.DataBind();
        }
        private void ClearForm()
        {
            hdFormId.Value = string.Empty;
            hdFileId.Value = string.Empty;
            drpFileType.SelectedIndex = 0;
            lnkFormUrl.Text = string.Empty;
            lnkFormUrl.NavigateUrl = string.Empty;
            uplFile.PostedFiles.Clear();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsAdmin)
                    Response.Redirect(_navigationManager.NavigateURL());
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                BindDropDowns();
                BindList();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptForms_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdEdit = (LinkButton)e.Item.FindControl("cmdEdit");
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                scriptMan.RegisterAsyncPostBackControl(cmdEdit);
            }
        }
        protected void rptForms_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int formId = Convert.ToInt32(e.CommandArgument);
            var ctl = new FormController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteForm(formId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Form form = ctl.GetForm(formId);
                string filePath = form.FilePath;
                hdFormId.Value = formId.ToString();
                drpFileType.SelectedValue = form.DocumentTypeID.ToString();
                hdFileId.Value = form.FileID.ToString();
                lnkFormUrl.Text = Path.GetFileName(filePath);
                lnkFormUrl.NavigateUrl = filePath;
                ScriptManager.RegisterStartupScript(rptForm, rptForm.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void pnlForms_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new FormController();
            Form form = new Form();
            bool isNew = true;
            if (hdFormId.Value != "")
            {
                isNew = false;
                form = ctl.GetForm(Convert.ToInt32(hdFormId.Value));
            }
            if (drpFileType.SelectedIndex > 0)
                form.DocumentTypeID = Int32.Parse(drpFileType.SelectedValue);
            form.FileID = Int32.Parse(hdFileId.Value);
            form.LastModifiedDate = DateTime.Now;
            form.LastModifiedByUser = UserId;
            if (isNew)
            {
                form.CreatedByUser = UserId;
                form.CreatedDate = DateTime.Now;
                ctl.CreateForm(form);
            }
            else
            {
                ctl.UpdateForm(form);
            }
            ClearForm();
            BindList();
        }
        protected void valUpload_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            if (hdFileId.Value != string.Empty)
            {
                args.IsValid = true;
            }
        }
        #endregion
    }
}