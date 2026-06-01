/*
' Copyright (c) 2026 Joe Terhune
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
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCounsel.Components.Controllers;
using tjc.Modules.CourtCounsel.Components.Models;

namespace tjc.Modules.CourtCounsel.Views
{
    public partial class Admin : CourtCounselModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                Response.Redirect(SearchUrl);
                return;
            }

            if (!IsPostBack)
            {
                BindCaseTypes();
                BindAttorneys();
                BindCounties();
                BindPhases();
                BindRequestors();
                BindActions();
                BindTimeSpent();
            }
        }

        #region Case Types

        private void BindCaseTypes()
        {
            var ctrl = new CaseTypeController();
            rptCaseTypes.DataSource = ctrl.GetCaseTypes().OrderBy(c => c.CaseType).ToList();
            rptCaseTypes.DataBind();
        }

        protected void cmdSaveCaseType_Click(object sender, EventArgs e)
        {
            var ctrl = new CaseTypeController();
            var id = Convert.ToInt32(hdCaseTypeId.Value);

            if (id > 0)
            {
                var item = ctrl.GetCaseType(id);
                if (item != null)
                {
                    item.CaseType = txtCaseType.Text.Trim();
                    ctrl.UpdateCaseType(item);
                }
            }
            else
            {
                var item = new CaseTypeInfo { CaseType = txtCaseType.Text.Trim() };
                ctrl.CreateCaseType(item);
            }

            hdCaseTypeId.Value = "0";
            txtCaseType.Text = "";
            BindCaseTypes();
        }

        protected void rptCaseTypes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new CaseTypeController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetCaseType(id);
                if (item != null)
                {
                    hdCaseTypeId.Value = item.CaseTypeId.ToString();
                    txtCaseType.Text = item.CaseType;
                    ScriptManager.RegisterStartupScript(upCaseTypes, upCaseTypes.GetType(), "showModal", "ShowModal('modalCaseType');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteCaseType(id);
                BindCaseTypes();
            }
        }

        protected void upCaseTypes_Unload(object sender, EventArgs e)
        {
            var mgr = ScriptManager.GetCurrent(Page);
            if (mgr != null)
            {
                var mType = mgr.GetType();
                var field = mType.GetField("_updatePanelRequiresUpdate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                // UpdatePanel dispose fix - no action needed here
            }
        }

        #endregion

        #region Attorneys

        private void BindAttorneys()
        {
            var ctrl = new AttorneyController();
            rptAttorneys.DataSource = ctrl.GetAttorneys().OrderBy(a => a.AttorneyName).ToList();
            rptAttorneys.DataBind();
        }

        protected void cmdSaveAttorney_Click(object sender, EventArgs e)
        {
            var ctrl = new AttorneyController();
            var id = Convert.ToInt32(hdAttorneyId.Value);

            if (id > 0)
            {
                var item = ctrl.GetAttorney(id);
                if (item != null)
                {
                    item.AttorneyName = txtAttorneyName.Text.Trim();
                    item.IsActive = chkAttorneyActive.Checked;
                    ctrl.UpdateAttorney(item);
                }
            }
            else
            {
                var item = new AttorneyInfo
                {
                    AttorneyName = txtAttorneyName.Text.Trim(),
                    IsActive = chkAttorneyActive.Checked
                };
                ctrl.CreateAttorney(item);
            }

            hdAttorneyId.Value = "0";
            txtAttorneyName.Text = "";
            chkAttorneyActive.Checked = false;
            BindAttorneys();
        }

        protected void rptAttorneys_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new AttorneyController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetAttorney(id);
                if (item != null)
                {
                    hdAttorneyId.Value = item.AttorneyId.ToString();
                    txtAttorneyName.Text = item.AttorneyName;
                    chkAttorneyActive.Checked = item.IsActive == true;
                    ScriptManager.RegisterStartupScript(upAttorneys, upAttorneys.GetType(), "showModal", "ShowModal('modalAttorney');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteAttorney(id);
                BindAttorneys();
            }
        }

        #endregion

        #region Counties

        private void BindCounties()
        {
            var ctrl = new CountyController();
            rptCounties.DataSource = ctrl.GetCounties().OrderBy(c => c.County).ToList();
            rptCounties.DataBind();
        }

        protected void cmdSaveCounty_Click(object sender, EventArgs e)
        {
            var ctrl = new CountyController();
            var id = Convert.ToInt32(hdCountyId.Value);

            if (id > 0)
            {
                var item = ctrl.GetCounty(id);
                if (item != null)
                {
                    item.County = txtCounty.Text.Trim();
                    ctrl.UpdateCounty(item);
                }
            }
            else
            {
                var item = new CountyInfo { County = txtCounty.Text.Trim() };
                ctrl.CreateCounty(item);
            }

            hdCountyId.Value = "0";
            txtCounty.Text = "";
            BindCounties();
        }

        protected void rptCounties_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new CountyController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetCounty(id);
                if (item != null)
                {
                    hdCountyId.Value = item.CountyId.ToString();
                    txtCounty.Text = item.County;
                    ScriptManager.RegisterStartupScript(upCounties, upCounties.GetType(), "showModal", "ShowModal('modalCounty');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteCounty(id);
                BindCounties();
            }
        }

        #endregion

        #region Phases

        private void BindPhases()
        {
            var ctrl = new PhaseController();
            rptPhases.DataSource = ctrl.GetPhases().OrderBy(p => p.Phase).ToList();
            rptPhases.DataBind();
        }

        protected void cmdSavePhase_Click(object sender, EventArgs e)
        {
            var ctrl = new PhaseController();
            var id = Convert.ToInt32(hdPhaseId.Value);

            if (id > 0)
            {
                var item = ctrl.GetPhase(id);
                if (item != null)
                {
                    item.Phase = txtPhase.Text.Trim();
                    ctrl.UpdatePhase(item);
                }
            }
            else
            {
                var item = new PhaseInfo { Phase = txtPhase.Text.Trim() };
                ctrl.CreatePhase(item);
            }

            hdPhaseId.Value = "0";
            txtPhase.Text = "";
            BindPhases();
        }

        protected void rptPhases_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new PhaseController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetPhase(id);
                if (item != null)
                {
                    hdPhaseId.Value = item.PhaseId.ToString();
                    txtPhase.Text = item.Phase;
                    ScriptManager.RegisterStartupScript(upPhases, upPhases.GetType(), "showModal", "ShowModal('modalPhase');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeletePhase(id);
                BindPhases();
            }
        }

        #endregion

        #region Requestors

        private void BindRequestors()
        {
            var ctrl = new RequestorController();
            rptRequestors.DataSource = ctrl.GetRequestors().OrderBy(r => r.RequestorName).ToList();
            rptRequestors.DataBind();
        }

        protected void cmdSaveRequestor_Click(object sender, EventArgs e)
        {
            var ctrl = new RequestorController();
            var id = Convert.ToInt32(hdRequestorId.Value);

            if (id > 0)
            {
                var item = ctrl.GetRequestor(id);
                if (item != null)
                {
                    item.RequestorName = txtRequestorName.Text.Trim();
                    item.IsActive = chkRequestorActive.Checked;
                    ctrl.UpdateRequestor(item);
                }
            }
            else
            {
                var item = new RequestorInfo
                {
                    RequestorName = txtRequestorName.Text.Trim(),
                    IsActive = chkRequestorActive.Checked
                };
                ctrl.CreateRequestor(item);
            }

            hdRequestorId.Value = "0";
            txtRequestorName.Text = "";
            chkRequestorActive.Checked = false;
            BindRequestors();
        }

        protected void rptRequestors_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new RequestorController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetRequestor(id);
                if (item != null)
                {
                    hdRequestorId.Value = item.RequestorId.ToString();
                    txtRequestorName.Text = item.RequestorName;
                    chkRequestorActive.Checked = item.IsActive == true;
                    ScriptManager.RegisterStartupScript(upRequestors, upRequestors.GetType(), "showModal", "ShowModal('modalRequestor');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteRequestor(id);
                BindRequestors();
            }
        }

        #endregion

        #region Actions

        private void BindActions()
        {
            var ctrl = new ActionTakenController();
            rptActions.DataSource = ctrl.GetActions().OrderBy(a => a.Action).ToList();
            rptActions.DataBind();
        }

        protected void cmdSaveAction_Click(object sender, EventArgs e)
        {
            var ctrl = new ActionTakenController();
            var id = Convert.ToInt32(hdActionId.Value);

            if (id > 0)
            {
                var item = ctrl.GetAction(id);
                if (item != null)
                {
                    item.Action = txtAction.Text.Trim();
                    ctrl.UpdateAction(item);
                }
            }
            else
            {
                var item = new ActionTakenInfo { Action = txtAction.Text.Trim() };
                ctrl.CreateAction(item);
            }

            hdActionId.Value = "0";
            txtAction.Text = "";
            BindActions();
        }

        protected void rptActions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new ActionTakenController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetAction(id);
                if (item != null)
                {
                    hdActionId.Value = item.ActionId.ToString();
                    txtAction.Text = item.Action;
                    ScriptManager.RegisterStartupScript(upActions, upActions.GetType(), "showModal", "ShowModal('modalAction');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteAction(id);
                BindActions();
            }
        }

        #endregion

        #region Time Spent

        private void BindTimeSpent()
        {
            var ctrl = new TimeSpentController();
            rptTimeSpent.DataSource = ctrl.GetTimeSpents().OrderBy(t => t.TimeSpan).ToList();
            rptTimeSpent.DataBind();
        }

        protected void cmdSaveTimeSpent_Click(object sender, EventArgs e)
        {
            var ctrl = new TimeSpentController();
            var id = Convert.ToInt32(hdTimeSpentId.Value);

            if (id > 0)
            {
                var item = ctrl.GetTimeSpent(id);
                if (item != null)
                {
                    item.TimeSpan = txtTimeSpan.Text.Trim();
                    item.IsActive = chkTimeSpentActive.Checked;
                    ctrl.UpdateTimeSpent(item);
                }
            }
            else
            {
                var item = new TimeSpentInfo
                {
                    TimeSpan = txtTimeSpan.Text.Trim(),
                    IsActive = chkTimeSpentActive.Checked
                };
                ctrl.CreateTimeSpent(item);
            }

            hdTimeSpentId.Value = "0";
            txtTimeSpan.Text = "";
            chkTimeSpentActive.Checked = false;
            BindTimeSpent();
        }

        protected void rptTimeSpent_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var ctrl = new TimeSpentController();
            var id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditItem")
            {
                var item = ctrl.GetTimeSpent(id);
                if (item != null)
                {
                    hdTimeSpentId.Value = item.TimeSpanId.ToString();
                    txtTimeSpan.Text = item.TimeSpan;
                    chkTimeSpentActive.Checked = item.IsActive;
                    ScriptManager.RegisterStartupScript(upTimeSpent, upTimeSpent.GetType(), "showModal", "ShowModal('modalTimeSpent');", true);
                }
            }
            else if (e.CommandName == "DeleteItem")
            {
                ctrl.DeleteTimeSpent(id);
                BindTimeSpent();
            }
        }

        #endregion
    }
}
