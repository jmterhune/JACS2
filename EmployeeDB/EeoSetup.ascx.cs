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
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Collections.Generic;

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
    public partial class EeoSetup : EmployeeDBModuleBase
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
                    var ctl = new JobGroupController();
                    drpCategory.DataTextField = "Description";
                    drpCategory.DataValueField = "JobGroupId";
                    drpCategory.DataSource = ctl.GetJobGroups();
                    drpCategory.DataBind();
                    txtEndDate.Text = new DateTime(DateTime.Now.Year, 6, 30).ToShortDateString();
                    txtStartDate.Text = new DateTime(DateTime.Now.AddYears(-1).Year, 7, 1).ToShortDateString();
                    PopulateEeoList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new Components.EEOController();
            EEO eeo = new EEO();
            bool isNew = true;
            if (hdEeoId.Value != "")
            {
                isNew = false;
                eeo = ctl.GetEEO(Convert.ToInt32(hdEeoId.Value));
            }
            if (drpCategory.SelectedValue != "")
            {
                eeo.JobGroupId = Convert.ToInt32(drpCategory.SelectedValue);

            }
            eeo.Year = Convert.ToInt32(txtYear.Text);
            eeo.HireAsian = Convert.ToDecimal(txtHireAsian.Text);
            eeo.HireBlack = Convert.ToDecimal(txtHireBlack.Text);
            eeo.HireFemale = Convert.ToDecimal(txtHireFemale.Text);
            eeo.HireHispanic = Convert.ToDecimal(txtHireHispanic.Text);
            eeo.HireMale = Convert.ToDecimal(txtHireMale.Text);
            eeo.HireOther = Convert.ToDecimal(txtHireOther.Text);
            eeo.HireWhite = Convert.ToDecimal(txtHireWhite.Text);
            eeo.PopulationAsian = Convert.ToDecimal(txtPopAsian.Text);
            eeo.PopulationBlack = Convert.ToDecimal(txtPopBlack.Text);
            eeo.PopulationFemale = Convert.ToDecimal(txtPopFemale.Text);
            eeo.PopulationHispanic = Convert.ToDecimal(txtPopHispanic.Text);
            eeo.PopulationMale = Convert.ToDecimal(txtPopMale.Text);
            eeo.PopulationOther = Convert.ToDecimal(txtPopOther.Text);
            eeo.PopulationWhite = Convert.ToDecimal(txtPopWhite.Text);
            eeo.PromoAsian = Convert.ToDecimal(txtPromAsian.Text);
            eeo.PromoBlack = Convert.ToDecimal(txtPromBlack.Text);
            eeo.PromoFemale = Convert.ToDecimal(txtPromFemale.Text);
            eeo.PromoHispanic = Convert.ToDecimal(txtPromHispanic.Text);
            eeo.PromoMale = Convert.ToDecimal(txtPromMale.Text);
            eeo.PromoOther = Convert.ToDecimal(txtPromOther.Text);
            eeo.PromoWhite = Convert.ToDecimal(txtPromWhite.Text);
            eeo.TransferAsian = Convert.ToDecimal(txtTransAsian.Text);
            eeo.TransferBlack = Convert.ToDecimal(txtTransBlack.Text);
            eeo.TransferFemale = Convert.ToDecimal(txtTransFemale.Text);
            eeo.TransferHispanic = Convert.ToDecimal(txtTransHispanic.Text);
            eeo.TransferMale = Convert.ToDecimal(txtTransMale.Text);
            eeo.TransferOther = Convert.ToDecimal(txtTransOther.Text);
            eeo.TransferWhite = Convert.ToDecimal(txtTransWhite.Text);
            eeo.TermAsian = Convert.ToDecimal(txtTermAsian.Text);
            eeo.TermBlack = Convert.ToDecimal(txtTermBlack.Text);
            eeo.TermFemale = Convert.ToDecimal(txtTermFemale.Text);
            eeo.TermHispanic = Convert.ToDecimal(txtTermHispanic.Text);
            eeo.TermMale = Convert.ToDecimal(txtTermMale.Text);
            eeo.TermOther = Convert.ToDecimal(txtTermOther.Text);
            eeo.TermWhite = Convert.ToDecimal(txtTermWhite.Text);
            eeo.LastModifiedDate = DateTime.Now;
            eeo.LastModifiedById = UserId;
            if (isNew)
            {
                eeo.CreatedById = UserId;
                eeo.CreatedDate = DateTime.Now;
                ctl.CreateEEO(eeo);
            }
            else
            {
                ctl.UpdateEEO(eeo);
            }
            hdEeoId.Value = "";
            PopulateEeoList();
        }

        protected void cmdAccept_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new EmployeeController();
                if (DateTime.TryParse(txtStartDate.Text, out DateTime startDate) && DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                {
                    var ctlJG = new JobGroupController();
                    var ctlEeo = new EEOController();
                    foreach (JobGroup jg in ctlJG.GetJobGroups())
                    {
                        EEO eeo = new EEO
                        {
                            JobGroupId = jg.JobGroupId,
                            PopulationMale = ctl.GetGenderCount(jg.JobGroupId, "M", startDate, endDate),
                            PopulationFemale = ctl.GetGenderCount(jg.JobGroupId, "F", startDate, endDate),
                            PopulationWhite = ctl.GetRaceCount(jg.JobGroupId, "W", startDate, endDate),
                            PopulationAsian = ctl.GetRaceCount(jg.JobGroupId, "A", startDate, endDate),
                            PopulationHispanic = ctl.GetRaceCount(jg.JobGroupId, "H", startDate, endDate),
                            PopulationBlack = ctl.GetRaceCount(jg.JobGroupId, "B", startDate, endDate),
                            PopulationIndian = ctl.GetRaceCount(jg.JobGroupId, "I", startDate, endDate),
                            PopulationOther = ctl.GetRaceCount(jg.JobGroupId, "O", startDate, endDate),
                            HireMale = ctl.GetGenderHireCount(jg.JobGroupId, "M", startDate, endDate),
                            HireIndian = ctl.GetRaceHireCount(jg.JobGroupId, "I", startDate, endDate),
                            HireFemale = ctl.GetGenderHireCount(jg.JobGroupId, "F", startDate, endDate),
                            HireAsian = ctl.GetRaceHireCount(jg.JobGroupId, "A", startDate, endDate),
                            HireBlack = ctl.GetRaceHireCount(jg.JobGroupId, "B", startDate, endDate),
                            HireHispanic = ctl.GetRaceHireCount(jg.JobGroupId, "H", startDate, endDate),
                            HireOther = ctl.GetRaceHireCount(jg.JobGroupId, "O", startDate, endDate),
                            HireWhite = ctl.GetRaceHireCount(jg.JobGroupId, "W", startDate, endDate),
                            PromoIndian = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "I", startDate, endDate, "P"),
                            PromoMale = ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "M", startDate, endDate, "P"),
                            TransferIndian = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "I", startDate, endDate, "T"),
                            TransferMale = ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "M", startDate, endDate, "T"),
                            PromoFemale = ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "F", startDate, endDate, "P"),
                            TransferFemale = ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "F", startDate, endDate, "T"),
                            TransferAsian = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "A", startDate, endDate, "T"),
                            PromoAsian = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "A", startDate, endDate, "P"),
                            TransferBlack = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "B", startDate, endDate, "T"),
                            PromoBlack = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "B", startDate, endDate, "P"),
                            TransferHispanic = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "H", startDate, endDate, "T"),
                            PromoHispanic = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "H", startDate, endDate, "P"),
                            TransferOther = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "O", startDate, endDate, "T"),
                            PromoOther = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "O", startDate, endDate, "P"),
                            TransferWhite = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "W", startDate, endDate, "T"),
                            PromoWhite = ctl.GetRacePromotionTransferCount(jg.JobGroupId, "W", startDate, endDate, "P"),
                            TermMale = ctl.GetGenderTerminationCount(jg.JobGroupId, "M", startDate, endDate),
                            TermFemale = ctl.GetGenderTerminationCount(jg.JobGroupId, "F", startDate, endDate),
                            TermIndian = ctl.GetRaceTerminationCount(jg.JobGroupId, "I", startDate, endDate),
                            TermAsian = ctl.GetRaceTerminationCount(jg.JobGroupId, "A", startDate, endDate),
                            TermBlack = ctl.GetRaceTerminationCount(jg.JobGroupId, "B", startDate, endDate),
                            TermHispanic = ctl.GetRaceTerminationCount(jg.JobGroupId, "H", startDate, endDate),
                            TermOther = ctl.GetRaceTerminationCount(jg.JobGroupId, "O", startDate, endDate),
                            TermWhite = ctl.GetRaceTerminationCount(jg.JobGroupId, "W", startDate, endDate),
                            Year = endDate.Year,
                            CreatedById = UserId,
                            LastModifiedById = UserId,
                            CreatedDate = DateTime.Now,
                            LastModifiedDate = DateTime.Now,
                        };
                        ctlEeo.CreateEEO(eeo);
                        cmdReview.Visible = true;
                        cmdAccept.Visible = false;
                        PopulateEeoList();
                    }
                    ltEEOInfo.Text = "<div class='alert alert-success'><i class='fa fa-thumbs-up'></i> All EEO Information posted to database.</div>";
                }
                else
                {
                    ltEEOInfo.Text = "<div class='alert alert-warning'><i class='fa fa-warning'></i> Invalid Date. Please review the dates entered and try again.</div>";
                }
            }
            catch (Exception exc)
            {
                ltEEOInfo.Text = string.Format("<div class='alert alert-danger'><i class='fa fa-exclamation'></i> {0}</div>", exc.Message);
            }
        }
        protected void cmdReview_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder outStr = new StringBuilder();
                cmdReview.Visible = false;
                cmdAccept.Visible = true;
                var ctl = new EmployeeController();
                if (DateTime.TryParse(txtStartDate.Text, out DateTime startDate) && DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                {
                    var ctlJG = new JobGroupController();
                    var ctlRace = new RaceController();
                    IEnumerable<Race> races = ctlRace.GetRaces();
                    foreach (JobGroup jg in ctlJG.GetJobGroups())
                    {
                        outStr.Append("<h2>" + jg.Description + "</h2>");
                        outStr.Append("<div class='row'><div class='col-auto'><h5>Populations</h5><ul class='list-unstyled'>");
                        outStr.Append(string.Format("<li>Male: {0}</li>", ctl.GetGenderCount(jg.JobGroupId, "M", startDate, endDate)));
                        outStr.Append(string.Format("<li>Female: {0}</li>", ctl.GetGenderCount(jg.JobGroupId, "F", startDate, endDate)));
                        foreach (var r in races)
                            outStr.Append(string.Format("<li>{0}: {1}</li>", r.Description, ctl.GetRaceCount(jg.JobGroupId, r.RaceCode, startDate, endDate)));
                        outStr.Append("</ul></div><div class='col-auto'><h5>New Hires</h5><ul class='list-unstyled'>");
                        outStr.Append(string.Format("<li>Male: {0}</li>", ctl.GetGenderHireCount(jg.JobGroupId, "M", startDate, endDate)));
                        outStr.Append(string.Format("<li>Female: {0}</li>", ctl.GetGenderHireCount(jg.JobGroupId, "F", startDate, endDate)));
                        foreach (var r in races)
                            outStr.Append(string.Format("<li>{0}: {1}</li>", r.Description, ctl.GetRaceHireCount(jg.JobGroupId, r.RaceCode, startDate, endDate)));
                        outStr.Append("</ul></div><div class='col-auto'><h5>Transfers</h5><ul class='list-unstyled'>");
                        outStr.Append(string.Format("<li>Male: {0}</li>", ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "M", startDate, endDate, "T")));
                        outStr.Append(string.Format("<li>Female: {0}</li>", ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "F", startDate, endDate, "T")));
                        foreach (var r in races)
                            outStr.Append(string.Format("<li>{0}: {1}</li>", r.Description, ctl.GetRacePromotionTransferCount(jg.JobGroupId, r.RaceCode, startDate, endDate, "T")));
                        outStr.Append("</ul></div><div class='col-auto'><h5>Promotions</h5><ul class='list-unstyled'>");
                        outStr.Append(string.Format("<li>Male: {0}</li>", ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "M", startDate, endDate, "P")));
                        outStr.Append(string.Format("<li>Female: {0}</li>", ctl.GetGenderPromotionTransferCount(jg.JobGroupId, "F", startDate, endDate, "P")));
                        foreach (var r in races)
                            outStr.Append(string.Format("<li>{0}: {1}</li>", r.Description, ctl.GetRacePromotionTransferCount(jg.JobGroupId, r.RaceCode, startDate, endDate, "P")));
                        outStr.Append("</ul></div><div class='col-auto'><h5>Terminations</h5><ul class='list-unstyled'>");
                        outStr.Append(string.Format("<li>Male: {0}</li>", ctl.GetGenderTerminationCount(jg.JobGroupId, "M", startDate, endDate)));
                        outStr.Append(string.Format("<li>Female: {0}</li>", ctl.GetGenderTerminationCount(jg.JobGroupId, "F", startDate, endDate)));
                        foreach (var r in races)
                            outStr.Append(string.Format("<li>{0}: {1}</li>", r.Description, ctl.GetRaceTerminationCount(jg.JobGroupId, r.RaceCode, startDate, endDate)));
                        outStr.Append("</ul></div></div>");

                    }
                    ltEEOInfo.Text = outStr.ToString();
                }
                else
                {
                    ltEEOInfo.Text = "<div class='alert alert-warning'><i class='fa fa-warning'></i> Invalid Date. Please review the dates entered and try again.</div>";
                }
            }
            catch (Exception exc)
            {
                ltEEOInfo.Text = string.Format("<div class='alert alert-danger'><i class='fa fa-exclamation'></i> {0}</div>", exc.Message);
            }
        }

        protected void pnlEeo_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }
        protected void upReview_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).Last();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void rptEEO_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int eeoId = Convert.ToInt32(e.CommandArgument);
            var ctl = new Components.EEOController();
            if (e.CommandName == "delete")
            {

                ctl.DeleteEEO(eeoId);
                PopulateEeoList();
            }
            if (e.CommandName == "edit")
            {
                EEO eeo = ctl.GetEEO(eeoId);
                drpCategory.SelectedValue = eeo.JobGroupId.ToString();
                hdEeoId.Value = eeoId.ToString();
                txtYear.Text = eeo.Year.ToString();
                if (eeo.HireAsian.HasValue)
                    txtHireAsian.Text = eeo.HireAsian.Value.ToString("F1");
                if (eeo.HireBlack.HasValue)
                    txtHireBlack.Text = eeo.HireBlack.Value.ToString("F1");
                if (eeo.HireFemale.HasValue)
                    txtHireFemale.Text = eeo.HireFemale.Value.ToString("F1");
                if (eeo.HireHispanic.HasValue)
                    txtHireHispanic.Text = eeo.HireHispanic.Value.ToString("F1");
                if (eeo.HireMale.HasValue)
                    txtHireMale.Text = eeo.HireMale.Value.ToString("F1");
                if (eeo.HireOther.HasValue)
                    txtHireOther.Text = eeo.HireOther.Value.ToString("F1");
                if (eeo.HireWhite.HasValue)
                    txtHireWhite.Text = eeo.HireWhite.Value.ToString("F1");
                if (eeo.PopulationAsian.HasValue)
                    txtPopAsian.Text = eeo.PopulationAsian.Value.ToString("F1");
                if (eeo.PopulationBlack.HasValue)
                    txtPopBlack.Text = eeo.PopulationBlack.Value.ToString("F1");
                if (eeo.PopulationFemale.HasValue)
                    txtPopFemale.Text = eeo.PopulationFemale.Value.ToString("F1");
                if (eeo.PopulationHispanic.HasValue)
                    txtPopHispanic.Text = eeo.PopulationHispanic.Value.ToString("F1");
                if (eeo.PopulationMale.HasValue)
                    txtPopMale.Text = eeo.PopulationMale.Value.ToString("F1");
                if (eeo.PopulationOther.HasValue)
                    txtPopOther.Text = eeo.PopulationOther.Value.ToString("F1");
                if (eeo.PopulationWhite.HasValue)
                    txtPopWhite.Text = eeo.PopulationWhite.Value.ToString("F1");
                if (eeo.TransferAsian.HasValue)
                    txtTransAsian.Text = eeo.TransferAsian.Value.ToString("F1");
                if (eeo.TransferBlack.HasValue)
                    txtTransBlack.Text = eeo.TransferBlack.Value.ToString("F1");
                if (eeo.TransferFemale.HasValue)
                    txtTransFemale.Text = eeo.TransferFemale.Value.ToString("F1");
                if (eeo.TransferHispanic.HasValue)
                    txtTransHispanic.Text = eeo.TransferHispanic.Value.ToString("F1");
                if (eeo.TransferMale.HasValue)
                    txtTransMale.Text = eeo.TransferMale.Value.ToString("F1");
                if (eeo.TransferOther.HasValue)
                    txtTransOther.Text = eeo.TransferOther.Value.ToString("F1");
                if (eeo.TransferWhite.HasValue)
                    txtTransWhite.Text = eeo.TransferWhite.Value.ToString("F1");
                if (eeo.PromoAsian.HasValue)
                    txtPromAsian.Text = eeo.PromoAsian.Value.ToString("F1");
                if (eeo.PromoBlack.HasValue)
                    txtPromBlack.Text = eeo.PromoBlack.Value.ToString("F1");
                if (eeo.PromoFemale.HasValue)
                    txtPromFemale.Text = eeo.PromoFemale.Value.ToString("F1");
                if (eeo.PromoHispanic.HasValue)
                    txtPromHispanic.Text = eeo.PromoHispanic.Value.ToString("F1");
                if (eeo.PromoMale.HasValue)
                    txtPromMale.Text = eeo.PromoMale.Value.ToString("F1");
                if (eeo.PromoOther.HasValue)
                    txtPromOther.Text = eeo.PromoOther.Value.ToString("F1");
                if (eeo.PromoWhite.HasValue)
                    txtPromWhite.Text = eeo.PromoWhite.Value.ToString("F1");
                if (eeo.TermAsian.HasValue)
                    txtTermAsian.Text = eeo.TermAsian.Value.ToString("F1");
                if (eeo.TermBlack.HasValue)
                    txtTermBlack.Text = eeo.TermBlack.Value.ToString("F1");
                if (eeo.TermFemale.HasValue)
                    txtTermFemale.Text = eeo.TermFemale.Value.ToString("F1");
                if (eeo.TermHispanic.HasValue)
                    txtTermHispanic.Text = eeo.TermHispanic.Value.ToString("F1");
                if (eeo.TermMale.HasValue)
                    txtTermMale.Text = eeo.TermMale.Value.ToString("F1");
                if (eeo.TermOther.HasValue)
                    txtTermOther.Text = eeo.TermOther.Value.ToString("F1");
                if (eeo.TermWhite.HasValue)
                    txtTermWhite.Text = eeo.TermWhite.Value.ToString("F1");
                ScriptManager.RegisterStartupScript(rptEEO, rptEEO.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void rptEEO_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        private void PopulateEeoList()
        {
            var ctl = new Components.EEOListController();
            rptEEO.DataSource = ctl.GetEeoList();
            rptEEO.DataBind();
        }

        #endregion


    }
}