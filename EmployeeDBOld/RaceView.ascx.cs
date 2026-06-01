/*
' Copyright (c) 2023  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Services.Exceptions;
using System;
using tjc.Modules.EmployeeDB.Components;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace tjc.Modules.EmployeeDB
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from EmployeeDBModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class RaceView : EmployeeDBModuleBase
    {

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }

                    PopulateRaceList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptRaces_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            int raceId = Convert.ToInt32(e.CommandArgument);
            var ctl = new RaceController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteRace(raceId);
                PopulateRaceList();
            }
            if (e.CommandName == "edit")
            {
                Race race = ctl.GetRace(raceId);

                hdRaceId.Value = raceId.ToString();
                txtDescription.Text = race.Description;
                txtRaceCode.Text = race.RaceCode;

                ScriptManager.RegisterStartupScript(rptRaces, rptRaces.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new RaceController();
            Race race = new Race();
            bool isNew = true;
            if (hdRaceId.Value != "")
            {
                isNew = false;
                race = ctl.GetRace(Convert.ToInt32(hdRaceId.Value));
            }
            race.Description = txtDescription.Text;
            race.RaceCode = txtRaceCode.Text;
            race.LastModifiedDate = DateTime.Now;
            race.LastModifiedByID = UserId;
            if (isNew)
            {
                race.CreatedByID = UserId;
                race.CreatedDate = DateTime.Now;
                ctl.CreateRace(race);
            }
            else
            {
                ctl.UpdateRace(race);
            }
            ClearForm();
            PopulateRaceList();
        }
        protected void pnlRaces_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptRaces_ItemCreated(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
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

        #region Methods
        private void ClearForm()
        {
            hdRaceId.Value = string.Empty;
            txtDescription.Text = string.Empty;
            txtRaceCode.Text = string.Empty;
        }
        private void PopulateRaceList()
        {
            var ctl = new RaceController();
            rptRaces.DataSource = ctl.GetRaces();
            rptRaces.DataBind();
        }
        #endregion

    }
}