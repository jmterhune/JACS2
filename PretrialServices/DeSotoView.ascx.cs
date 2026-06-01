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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.PretrialServices.Components;

namespace tjc.Modules.PretrialServices
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from PretrialServicesModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class DeSotoView : PretrialServicesModuleBase
    {
        private DefendantInProgramController ctl = new DefendantInProgramController();
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                   
                JavaScript.RequestRegistration(CommonJs.jQueryUI);
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                if (!Page.IsPostBack)
                {
                    if (DotNetNuke.Framework.AJAX.IsInstalled())
                    {
                        DotNetNuke.Framework.AJAX.RegisterScriptManager();
                    }
                    GetCookieIntakeDate();
                    PopulateYears();
                    DateTime? cookieDate = GetCookieIntakeDate();
                    if (cookieDate.HasValue && cookieDate.Value != Null.NullDate)
                        IntakeDate = cookieDate.Value;
                    if (QueryDate.HasValue)
                        IntakeDate = QueryDate.Value;
                    hdIntakeDate.Value = IntakeDate.ToShortDateString();
                    PopulateDays();
                    drpYear.SelectedValue = IntakeDate.Year.ToString();
                    drpDay.SelectedValue = IntakeDate.Day.ToString();
                    drpMonth.SelectedValue = IntakeDate.Month.ToString();
                    BindData();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void rptDefendantsInProgram_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "delete")
            {
                ctl.DeleteDefendantInProgram(itemId);
                BindData();
            }
            if (e.CommandName == "edit")
            {
                DefendantInProgram dip = ctl.GetDefendantInProgram(itemId);
                hdItemId.Value = itemId.ToString();
                if (dip.IntakeDate.HasValue)
                    txtIntakeDate.Text = dip.IntakeDate.Value.ToShortDateString();
                txtName.Text = dip.DefendantName;
                txtFTADate.Text = dip.FormattedFTADate;
                txtCaseNumber.Text = dip.CaseNumber;
                txtCompletionDate.Text = dip.FormattedCompletionDate;
                txtCharges.Text = dip.ArrestCharges;
                txtfcDanger.Text = dip.FcDangerous.ToString();
                txtfcNonDanger.Text = dip.FcNonDangerous.ToString();
                txtmcDanger.Text = dip.McDangerous.ToString();
                txtmcNonDanger.Text = dip.McNonDangerous.ToString();
                drpNewArrest.SelectedValue = dip.NonCompArrestViolation;
                txtCourtAppearances.Text = dip.CourtAppearances.ToString();
                chkFtaArrestHearing.Checked = dip.FtaArrestHearing;
                chkIndigent.Checked = dip.Indigent;
                chkBwOrdered.Checked = dip.BwOrdered;
                chkRevoked.Checked = dip.IsRevoked;
                chkCaseScreened.Checked = dip.CaseScreened;
                chkPlaced.Checked = dip.PlacedInProgram;
                chkInterviewed.Checked = dip.Interviewed;
                chkAssessed.Checked = dip.Assessed;
                chkPtrRecommended.Checked = dip.PtrRecommended;
                drpMostSeriosOffense.SelectedValue = dip.MostSeriousOffense;
                chkPtrOrdered.Checked = dip.PtrOrdered;
                chkIndigent.Checked = dip.Indigent;
                chkPtrNotRecommended.Checked = dip.PtrNotRecommended;

                if (dip.Completion.HasValue)
                    drpCompletion.SelectedValue = dip.Completion.ToString();
                if (dip.BondType.HasValue)
                    drpBondType.SelectedValue = dip.BondType.ToString();
                if (dip.NonCompliance.HasValue)
                    drpNonCompliance.SelectedValue = dip.NonCompliance.ToString();
                if (dip.CaseType.HasValue)
                    drpCaseType.SelectedValue = dip.CaseType.ToString();
                ScriptManager.RegisterStartupScript(rptDefendantsInProgram, rptDefendantsInProgram.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }
        protected void rptDefendantsInProgram_ItemCreated(object sender, RepeaterItemEventArgs e)
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
        protected void pnlDefendantsInProgram_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            DefendantInProgram dip = new DefendantInProgram();
            bool isNew = true;
            if (hdItemId.Value != "")
            {
                isNew = false;
                dip = ctl.GetDefendantInProgram(Convert.ToInt32(hdItemId.Value));
            }
            if (DateTime.TryParse(txtIntakeDate.Text, out DateTime intakeDate))
                dip.IntakeDate = intakeDate;
            if (DateTime.TryParse(txtFTADate.Text, out DateTime ftaDate))
                dip.FtaDate = ftaDate;
            else
                dip.FtaDate = null;
            if (DateTime.TryParse(txtCompletionDate.Text, out DateTime compDate))
                dip.CompletionDate = compDate;
            else
                dip.CompletionDate = null;
            dip.DefendantName = txtName.Text;
            dip.CaseNumber = txtCaseNumber.Text;
            dip.ArrestCharges = txtCharges.Text;
            dip.NonCompArrestViolation = drpNewArrest.SelectedValue;
            dip.Indigent = chkIndigent.Checked;
            dip.FtaArrestHearing = chkFtaArrestHearing.Checked;
            dip.BwOrdered = chkBwOrdered.Checked;
            dip.MostSeriousOffense = drpMostSeriosOffense.SelectedValue;
            dip.CaseScreened = chkCaseScreened.Checked;
            dip.PlacedInProgram = chkPlaced.Checked;
            dip.IsRevoked = chkRevoked.Checked;
            dip.Interviewed = chkInterviewed.Checked;
            dip.Indigent = chkIndigent.Checked;
            dip.Assessed = chkAssessed.Checked;
            dip.PtrNotRecommended = chkPtrNotRecommended.Checked;
            dip.PtrOrdered = chkPtrOrdered.Checked;
            dip.PtrRecommended = chkPtrRecommended.Checked;
            if (drpCompletion.SelectedIndex > 0)
                dip.Completion = Int32.Parse(drpCompletion.SelectedValue);
            if (drpCaseType.SelectedIndex > 0)
                dip.CaseType = Int32.Parse(drpCaseType.SelectedValue);
            if (drpNonCompliance.SelectedIndex > 0)
                dip.NonCompliance = Int32.Parse(drpNonCompliance.SelectedValue);
            if (drpBondType.SelectedIndex > 0)
                dip.BondType = Int32.Parse(drpBondType.SelectedValue);
            Int32.TryParse(txtfcDanger.Text, out int fcDanger);
            dip.FcDangerous = fcDanger;
            Int32.TryParse(txtfcNonDanger.Text, out int fcNonDanger);
            dip.FcNonDangerous = fcNonDanger;
            Int32.TryParse(txtmcDanger.Text, out int mcDanger);
            dip.McDangerous = mcDanger;
            Int32.TryParse(txtmcNonDanger.Text, out int mcNonDanger);
            dip.McNonDangerous = mcNonDanger;
            Int32.TryParse(txtCourtAppearances.Text, out int courtAppearances);
            dip.CourtAppearances = courtAppearances;
            dip.LastModifiedDate = DateTime.Now;
            dip.LastModifiedById = UserId;
            dip.CountyId = CountyId;
            if (dip.CompletionDate.HasValue && dip.IntakeDate.HasValue)
            {
                TimeSpan difference = dip.CompletionDate.Value - dip.IntakeDate.Value;
                dip.DaysSpr = difference.Days;
            }
            else
            {
                dip.DaysSpr = 0;
            }
            if (isNew)
            {
                dip.CreatedById = UserId;
                dip.CreatedDate = DateTime.Now;
                ctl.CreateDefendantInProgram(dip);
            }
            else
            {
                ctl.UpdateDefendantInProgram(dip);
            }
            ClearDefendantsInProgramForm();
            BindData();
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            BindData();
        }
        protected void cmdReport_Click(object sender, EventArgs e)
        {
            string reportUrl = string.Format("{0}/PretrialReport.aspx?cid={1}&mid={2}&indate={3}&st={4}", TemplateSourceDirectory, CountyId, ModuleId, IntakeDate.ToShortDateString(), DateTime.Now.ToString("yyyyMMddHHmmss"));
            switch (hdReportType.Value)
            {
                case "0":
                    {
                        Response.Redirect(string.Format("{0}&rid={1}", reportUrl, "daily"), true);
                        break;
                    }

                case "1":
                    {
                        Response.Redirect(string.Format("{0}&rid={1}", reportUrl, "weekly"), true);
                        break;
                    }

                case "2":
                    {
                        Response.Redirect(string.Format("{0}&rid={1}", reportUrl, "monthly"), true);
                        break;
                    }

                case "3":
                    {
                        Response.Redirect(string.Format("{0}&rid={1}", reportUrl, "yearly"), true);
                        break;
                    }
                case "4":
                    {
                        Response.Redirect(EditUrl("date", IntakeDate.ToShortDateString(), "survey"), true);
                        break;
                    }
            }
        }
        protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }
        protected void drpDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }
        protected void drpYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }
        protected void cmdClose_Click(object sender, EventArgs e)
        {
            ClearDefendantsInProgramForm();
        }
        #endregion

        #region Methods
        private void BindData()
        {
            Enumerations.SearchType searchType = (Enumerations.SearchType)Int32.Parse(hdSearchType.Value);
            if (searchType == Enumerations.SearchType.date)
            {
                Int32.TryParse(drpYear.Text, out int year);
                Int32.TryParse(drpMonth.Text, out int month);
                Int32.TryParse(drpDay.Text, out int day);
                IntakeDate = new DateTime(year, month, day);
                hdIntakeDate.Value = IntakeDate.ToShortDateString();
                if (IntakeDate != null)
                {
                    rptDefendantsInProgram.DataSource = ctl.GetDefendantsInProgramByCounty(CountyId, IntakeDate);
                    rptDefendantsInProgram.DataBind();
                }
            }
            else if (searchType == Enumerations.SearchType.caseNumber)
            {
                rptDefendantsInProgram.DataSource = ctl.GetDefendantsInProgramByCaseNumber(CountyId, txtSearchText.Text);
                rptDefendantsInProgram.DataBind();
            }
            else if (searchType == Enumerations.SearchType.defendantName)
            {
                rptDefendantsInProgram.DataSource = ctl.GetDefendantsInProgramByDefendantName(CountyId, txtSearchText.Text);
                rptDefendantsInProgram.DataBind();
            }
            SetCookies();
        }
        private void PopulateYears()
        {
            drpYear.DataSource = ctl.GetYears(CountyId);
            drpYear.DataBind();
        }
        private void PopulateDays()
        {
            int selYear = Int32.Parse(drpYear.SelectedValue);
            int selMonth = Int32.Parse(drpMonth.SelectedValue);
            int daysInMonth = DateTime.DaysInMonth(selYear, selMonth);
            drpDay.Items.Clear();
            for (var i = 1; i <= daysInMonth; i++)
                drpDay.Items.Add(new ListItem(i.ToString()));
        }
        private void SetCookies()
        {
            string cookieName = string.Format("CookieIntakeDate{0}", CountyId);
            HttpCookie aCookie = new HttpCookie("PretrialServices");
            aCookie.Values[cookieName] = IntakeDate.ToShortDateString();
            aCookie.Expires = DateTime.Now.AddDays(30);
            Response.Cookies.Add(aCookie);
        }
        private void ClearDefendantsInProgramForm()
        {
            txtName.Text = string.Empty;
            txtFTADate.Text = string.Empty;
            txtCaseNumber.Text = string.Empty;
            txtCompletionDate.Text = string.Empty;
            txtCharges.Text = string.Empty;
            txtCourtAppearances.Text = string.Empty;
            txtfcDanger.Text = string.Empty;
            txtfcNonDanger.Text = string.Empty;
            txtmcDanger.Text = string.Empty;
            txtmcNonDanger.Text = string.Empty;
            drpNewArrest.SelectedIndex = 0;
            drpCompletion.SelectedIndex = -1;
            drpBondType.SelectedIndex = -1;
            drpCaseType.SelectedIndex = -1;
            drpNonCompliance.SelectedIndex = -1;
            chkBwOrdered.Checked = false;
            chkIndigent.Checked = false;
            chkFtaArrestHearing.Checked = false;
            chkPlaced.Checked = false;
            chkCaseScreened.Checked = false;
            chkRevoked.Checked = false;
            chkInterviewed.Checked = false;
            chkAssessed.Checked = false;
            chkPtrRecommended.Checked = false;
            chkPtrOrdered.Checked = false;
            chkIndigent.Checked = false;
            chkPtrNotRecommended.Checked = false;
            hdItemId.Value = string.Empty;
        }
        private DateTime? GetCookieIntakeDate()
        {
            if (Request.Cookies["PretrialServices"] != null)
            {
                string cookieName = string.Format("CookieIntakeDate{0}", CountyId);
                string sCookieDate = Server.HtmlEncode(Request.Cookies["PretrialServices"][cookieName]);
                if (DateTime.TryParse(sCookieDate, out DateTime cDate))
                    return cDate;
            }
            return null;
        }
        private bool IsWeekend()
        {
            bool weekEnd = false;
            int selectedYear = Int32.Parse(drpYear.SelectedValue);
            int selectedMonth = Int32.Parse(drpMonth.SelectedValue);
            if (drpDay.SelectedValue == "7" | drpDay.SelectedValue == "14" | drpDay.SelectedValue == "21" | drpDay.SelectedValue == "28" | drpDay.SelectedValue == DateTime.DaysInMonth(selectedYear, selectedMonth).ToString())
            {
                weekEnd = true;
            }
            return weekEnd;
        }

        #endregion
        private enum ClearFormType
        {
            NewDay = 0, Deleted = 1, Saved = 2, Cancelled = 3
        }
    }
}
