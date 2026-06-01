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
using tjc.Modules.ExpertWitness.Components;
namespace tjc.Modules.ExpertWitness
{
    public partial class TypeList : ExpertWitnessModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion

        #region Methods
        public TypeList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindList()
        {
            var ctl = new TypeController();
            rptType.DataSource = ctl.GetTypes();
            rptType.DataBind();
        }
        private void ClearForm()
        {
            hdTypeId.Value = string.Empty;
            txtTypeName.Text = string.Empty;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
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
            var ctl = new TypeController();
            Components.Type action = new Components.Type();
            bool isNew = true;
            if (hdTypeId.Value != "")
            {
                isNew = false;
                action = ctl.GetType(Convert.ToInt32(hdTypeId.Value));
            }
            action.TypeName = txtTypeName.Text;
            action.ModifiedDate = DateTime.Now;
            action.ModifiedBy = UserInfo.Username;
            if (isNew)
            {
                action.CreatedBy = UserInfo.Username;
                action.CreatedDate = DateTime.Now;
                ctl.CreateType(action);
            }
            else
            {
                ctl.UpdateType(action);
            }
            ClearForm();
            BindList();
        }
        protected void pnlTypes_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptType_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int actionId = Convert.ToInt32(e.CommandArgument);
            var ctl = new TypeController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteType(actionId);
                BindList();
            }
            if (e.CommandName == "edit")
            {
                Components.Type action = ctl.GetType(actionId);
                hdTypeId.Value = actionId.ToString();
                txtTypeName.Text = action.TypeName;
                ScriptManager.RegisterStartupScript(rptType, rptType.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptType_ItemCreated(object sender, RepeaterItemEventArgs e)
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