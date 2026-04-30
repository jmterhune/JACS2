/*
' Copyright (c) 2023  12th Judicial Circuit
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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components;
namespace tjc.Modules.CourtCounsel
{
    public partial class TimeSpanList : CourtCounselModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public TimeSpanList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new TimeSpanController();
            rptTimeSpan.DataSource = ctl.GetTimeSpans();
            rptTimeSpan.DataBind();
        }
        private void ClearForm()
        {
            hdTimeSpanId.Value = string.Empty;
            txtTimeSpanName.Text = string.Empty;
            chkActive.Checked = false;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                chkActive.InputAttributes.Add("class", "form-check-input");
                chkActive.LabelAttributes.Add("class", "form-check-label");

                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new TimeSpanController();
            Components.TimeSpan timeSpan = new Components.TimeSpan();
            bool isNew = true;
            if (hdTimeSpanId.Value != "")
            {
                isNew = false;
                timeSpan = ctl.GetTimeSpan(Convert.ToInt32(hdTimeSpanId.Value));
            }
            timeSpan.TimeSpanName = txtTimeSpanName.Text;
            timeSpan.Active = chkActive.Checked;
            timeSpan.ModifiedDate = DateTime.Now;
            timeSpan.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                timeSpan.CreatedBy = UserInfo.Username;
                timeSpan.CreatedDate = DateTime.Now;
                ctl.CreateTimeSpan(timeSpan);
            }
            else
            {
                ctl.UpdateTimeSpan(timeSpan);
            }
            ClearForm();
            BindList();
        }
        protected void pnlTimeSpans_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptTimeSpan_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int timeSpanId = Convert.ToInt32(e.CommandArgument);
            var ctl = new TimeSpanController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteTimeSpan(timeSpanId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.TimeSpan timeSpan = ctl.GetTimeSpan(timeSpanId);
                hdTimeSpanId.Value = timeSpanId.ToString();
                txtTimeSpanName.Text = timeSpan.TimeSpanName;
                chkActive.Checked = timeSpan.Active;
                ScriptManager.RegisterStartupScript(rptTimeSpan, rptTimeSpan.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptTimeSpan_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        #endregion
    }
}