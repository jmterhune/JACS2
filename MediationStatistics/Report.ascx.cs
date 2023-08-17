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
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.MediationStatistics.Components;

namespace tjc.Modules.MediationStatistics
{
    /// -----------------------------------------------------------------------------
    /// <summary>   
    /// The Edit class is used to manage content
    /// 
    /// Typically your edit control would be used to create new content, or edit existing content within your module.
    /// The ControlKey for this control is "Edit", and is defined in the manifest (.dnn) file.
    /// 
    /// Because the control inherits from MediationStatisticsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Report : MediationStatisticsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private int linenumber = 0;
        private string questionaire = "";
        private string regionheader = "";
        private string caseTypeGroup = "";
        private ReportController ctl = new ReportController();
        #endregion
        #region Properties

        #endregion
        #region Methods
        public Report()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public string FormatNumber(string numberString, string percentageString)
        {
            string numberFormat = "";
            decimal.TryParse(numberString, out decimal number);
            decimal.TryParse(percentageString, out decimal percentage);
            if (percentage > 0)
                numberFormat = "<td>" + number + "</td><td nowrap>" + string.Format("{0:p0}", percentage) + "</td>";
            else if (percentage < 0)
                numberFormat = "<td width=\"30\">" + number + "</td>";
            else
                numberFormat = "<td colspan=\"2\">" + number + "</td>";
            return numberFormat;
        }

        private string GetHeader(string questionaire)
        {
            string header = "";

            switch (questionaire)
            {
                case "CDSP":
                    {
                        header = "<h2 class='mb-0'>CDS Program Questionnaire</h2>";
                        break;
                    }

                case "County":
                    {
                        header = "<h2 class='mb-0'>County Program Questionnaire</h2>";
                        break;
                    }

                case "Family":
                    {
                        header = "<h2 class='mb-0'>Family Program Questionnaire</h2>";
                        break;
                    }

                case "Dependency":
                    {
                        header = "<h2 class='mb-0'>Dependency Program Questionnaire</h2>";
                        break;
                    }

                case "Family Pre-filing":
                    {
                        header = "<h2 class='mb-0'>Family Pre-filing Program Questionnaire</h2>";
                        break;
                    }

                case "Juvenile Restitution":
                    {
                        header = "<h2 class='mb-0'>Juvenile Restitution Program Questionnaire</h2>";
                        break;
                    }

                case "Mediator":
                    {
                        header = "<h2 class='mb-0'>Counts by Mediator Type</h2>";
                        break;
                    }
            }
            header += "<table class='table table-striped'><thead><tr><th>&nbsp;</th><th>QUESTIONS</th><th colspan=\"2\">Sarasota</th><th colspan=\"2\">Manatee</th><th colspan=\"2\">DeSoto</th><th width=\"30\">South County</th><th width=\"30\">North County</th></tr></thead><tbody>";
            return header;
        }

        private string GetFeesHeader(string value, bool IsRegion)
        {
            string header = "";
            if (IsRegion)
                header = string.Format("<div class='heading heading-border heading-bottom-border'><h2 class='text-center mb-0'>{0}</h2></div>", value);
            else
            {
                switch (value)
                {
                    case "Citizen Dispute Settlement Program":
                        {
                            header = "<h3 class='mb-0'>CDSP</h3>";
                            break;
                        }

                    case "County":
                        {
                            header = "<h3 class='mb-0'>County</h3>";
                            break;
                        }

                    case "Family":
                        {
                            header = "<h3 class='mb-0'>Family</h3>";
                            break;
                        }

                    case "Dependency":
                        {
                            header = "<h3 class='mb-0'>Dependency</h3>";
                            break;
                        }

                    case "Small Claims":
                        {
                            header = "<h3 class='mb-0'>Small Claims</h3>";
                            break;
                        }

                    case "Family Pre-filing":
                        {
                            header = "<h3 class='mb-0'>Family Pre-filing</h3>";
                            break;
                        }

                    case "Juvenile Restitution":
                        {
                            header = "<h3 class='mb-0'>Juvenile Restitution</h3>";
                            break;
                        }
                }
                header += "<table class='table table-striped mb-3'><thead><tr><th>Case Number</th><th>Party</th><th><abbr title='Fee Owed'>FO</abbr></th><th>Attorney</th><th>Phone</th><th>Address</th><th>Mediation Date</th><th><abbr title='Fee Agreement'>FA</abbr></th><th><abbr title='Fee Judgement'>FJ</abbr></th><th><abbr title='Fee Waived'>FW</abbr></th><th><abbr title='Order to Show Cause'>OTSC</abbr></th><th nowrap>P-FTA</th><th nowrap>R-FTA</th></tr></thead><tbody>";
            }
            return header;
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lnkCancel.NavigateUrl = _navigationManager.NavigateURL();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdReport_Click(object sender, EventArgs e)
        {
            Conpendium.Visible = false;
            FeesOwed.Visible = false;
            Referrals.Visible = false;
            Checker.Visible = false;
            CollectedPaid.Visible = false;
            DateTime.TryParse(txtStartDate.Text, out DateTime startDate);
            DateTime.TryParse(txtEndDate.Text, out DateTime endDate);
            switch (drpReport.SelectedIndex)
            {
                case 0:
                    {
                        Conpendium.Visible = true;
                        if (!string.IsNullOrEmpty(txtEndDate.Text) && !string.IsNullOrEmpty(txtStartDate.Text))
                        {
                            rptConpendium.DataSource = ctl.GetStatReport(startDate, endDate);
                            rptConpendium.DataBind();
                        }
                        else
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "You must select a start and end date", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        break;
                    }

                case 1:
                    {
                        FeesOwed.Visible = true;
                        if (!string.IsNullOrEmpty(txtEndDate.Text) && !string.IsNullOrEmpty(txtStartDate.Text))
                        {
                            var feesOwed = ctl.GetFeesOwed(startDate, endDate).OrderBy(x => x.Region).ThenBy(x => x.CaseTypeGroup);
                            rptFeesOwed.DataSource = feesOwed;
                            rptFeesOwed.DataBind();
                        }
                        else
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "You must select a start and end date", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                        break;
                    }

                case 2:
                    {
                        Referrals.Visible = true;
                        var ctlSession = new SessionController();
                        var counts = ctlSession.GetReferralSourceItems(startDate, endDate);
                        rptReferrals.DataSource = counts;
                        rptReferrals.DataBind();
                        break;
                    }

                case 3:
                    {
                        Checker.Visible = true;
                        rgChecker.DataSource = ctl.GetSessionCounts(startDate, endDate);
                        rgChecker.DataBind();
                        break;
                    }

                case 4:
                    {
                        FeeReportCollectedOwed objCollected = ctl.GetFeeReportCollectedOwed(startDate, endDate).FirstOrDefault();
                        {


                            int familyPaid120 = objCollected.FamilyPaid120 ?? 0;
                            int familyPaid60 = objCollected.FamilyPaid60 ?? 0;
                            int familyPaidIndigent = objCollected.FamilyPaidIndigent ?? 0;
                            int countyPaid = objCollected.CountyPaid60 ?? 0;
                            int countyPaidIndigent = objCollected.CountyPaidIndigent ?? 0;
                            int familyOwed60 = objCollected.FamilyOwed60 ?? 0;
                            int familyOwed120 = objCollected.FamilyOwed120 ?? 0;
                            int familyOwedIndigent = objCollected.FamilyOwedIndigent ?? 0;
                            int familyOwed60FTA = objCollected.FamilyOwed60FTA ?? 0;
                            int familyOwed120FTA = objCollected.FamilyOwed120FTA ?? 0;
                            int familyOwedIndigentFTA = objCollected.FamilyOwedIndigentFTA ?? 0;
                            int countyOwed = objCollected.CountyOwed ?? 0;
                            int countyOwedFTA = objCollected.CountyOwedFTA ?? 0;
                            int countyOwedIndigent = objCollected.CountyOwedIndigent ?? 0;
                            int countyOwedIndigentFTA = objCollected.CountyOwedIndigentFTA ?? 0;
                            int countyOwedWaived = objCollected.CountyOwedWaived ?? 0;
                            int countyOwedWaivedFTA = objCollected.CountyOwedWaivedFTA ?? 0;
                            int countyPaidWaived = objCollected.CountyPaidWaived ?? 0;
                            int familyOwedWaived = objCollected.FamilyOwedWaived ?? 0;
                            int familyOwedWaivedFTA = objCollected.FamilyOwedWaivedFTA ?? 0;
                            int familyPaidWaived = objCollected.FamilyPaidWaived ?? 0;


                            if (familyPaid120 > 0)
                                lblFeeCollect120_f.Text = "$" + familyPaid120 * 120 + " (" + familyPaid120.ToString() + ")";
                            else
                                lblFeeCollect120_f.Text = "(" + familyPaid120.ToString() + ")";
                            if (countyPaid > 0)
                                lblFeeCollect60_c.Text = "$" + countyPaid * 60 + " (" + countyPaid.ToString() + ")";
                            else
                                lblFeeCollect60_c.Text = "(" + countyPaid.ToString() + ")";
                            if (familyPaid60 > 0)
                                lblFeeCollect60_f.Text = "$" + familyPaid60 * 60 + " (" + familyPaid60.ToString() + ")";
                            else
                                lblFeeCollect60_f.Text = "(" + familyPaid60.ToString() + ")";
                            if (familyOwed120 > 0)
                                lblFeeOwedHeld120_f.Text = "$" + familyOwed120 * 120 + " (" + familyOwed120.ToString() + ")";
                            else
                                lblFeeOwedHeld120_f.Text = "(" + familyOwed120.ToString() + ")";
                            if (countyOwed > 0)
                                lblFeeOwedHeld60_c.Text = "$" + countyOwed * 60 + " (" + countyOwed.ToString() + ")";
                            else
                                lblFeeOwedHeld60_c.Text = "(" + countyOwed.ToString() + ")";
                            if (familyOwed60 > 0)
                                lblFeeOwedHeld60_f.Text = "$" + familyOwed60 * 60 + " (" + familyOwed60.ToString() + ")";
                            else
                                lblFeeOwedHeld60_f.Text = "(" + familyOwed60.ToString() + ")";
                            if (familyOwed120FTA > 0)
                                lblFeeOwedNH120_f.Text = "$" + familyOwed120FTA * 120 + " (" + familyOwed120FTA.ToString() + ")";
                            else
                                lblFeeOwedNH120_f.Text = "(" + familyOwed120FTA.ToString() + ")";
                            if (countyOwedFTA > 0)
                                lblFeeOwedNH60_c.Text = "$" + countyOwedFTA * 60 + " (" + countyOwedFTA.ToString() + ")";
                            else
                                lblFeeOwedNH60_c.Text = "(" + countyOwedFTA.ToString() + ")";
                            if (familyOwed60FTA > 0)
                                lblFeeOwedNH60_f.Text = "$" + familyOwed60FTA * 60 + " (" + familyOwed60FTA.ToString() + ")";
                            else
                                lblFeeOwedNH60_f.Text = "(" + familyOwed60FTA.ToString() + ")";

                            lblMediationHeld_c.Text = objCollected.CountyCount.ToString();
                            lblMediationHeld_f.Text = objCollected.FamilyCount.ToString();
                            lblFeeCollectIndigent_c.Text = objCollected.CountyPaidIndigent.ToString();
                            lblFeeCollectIndigent_f.Text = objCollected.FamilyPaidIndigent.ToString();
                            lblFeeOwedHeldIndigent_c.Text = objCollected.CountyOwedIndigent.ToString();
                            lblFeeOwedHeldIndigent_f.Text = objCollected.FamilyOwedIndigent.ToString();
                            lblFeeOwedIndigentNH_f.Text = objCollected.FamilyOwedIndigentFTA.ToString();
                            lblFeeOwedNHIndigent_c.Text = objCollected.CountyOwedIndigentFTA.ToString();
                            lblFamilyPaidWaived.Text = objCollected.FamilyPaidWaived.ToString();
                            lblFamilyOwedWaived.Text = objCollected.FamilyOwedWaived.ToString();
                            lblFamilyOwedWaivedFTA.Text = objCollected.FamilyOwedWaivedFTA.ToString();
                            lblCountyOwedWaived.Text = objCollected.CountyOwedWaived.ToString();
                            lblCountyPaidWaived.Text = objCollected.CountyPaidWaived.ToString();
                            lblCountyOwedWaivedFTA.Text = objCollected.CountyOwedWaivedFTA.ToString();
                        }

                        // rgCollectedPaid.Rebind()
                        CollectedPaid.Visible = true;
                        break;
                    }
            }
        }

        protected void rptConpendium_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem | e.Item.ItemType == ListItemType.Item)
            {
                StatisticalReport objRecord = (StatisticalReport)e.Item.DataItem;
                if (objRecord != null)
                {
                    if (objRecord.questionaire != questionaire)
                    {
                        Literal ltHeader = (Literal)e.Item.FindControl("ltHeader");
                        if (questionaire != "")
                        {

                            if (ltHeader != null)
                                ltHeader.Text = "</tbody></table><hr />";
                        }
                        questionaire = objRecord.questionaire;
                        linenumber = 0;

                        if (ltHeader != null)
                            ltHeader.Text += GetHeader(questionaire);
                    }
                    Literal ltLineNumber = (Literal)e.Item.FindControl("ltLineNumber");
                    linenumber += 1;
                    ltLineNumber.Text = linenumber.ToString();
                }
            }
        }

        protected void cmdReturn_Click(object sender, System.EventArgs e)
        {
            Response.Redirect(_navigationManager.NavigateURL());
        }

        protected void rptFeesOwed_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem | e.Item.ItemType == ListItemType.Item)
            {
                FeesOwed objRecord = (FeesOwed)e.Item.DataItem;
                if (objRecord != null)
                {
                    Literal ltHeader = (Literal)e.Item.FindControl("ltHeader");
                    if (objRecord.Region != regionheader)
                    {
                        if (regionheader != "")
                        {
                            if (ltHeader != null)
                                ltHeader.Text = "</tbody></table>";
                        }
                        caseTypeGroup = "";
                        regionheader = objRecord.Region;
                        linenumber = 0;
                        if (ltHeader != null)
                            ltHeader.Text += GetFeesHeader(regionheader, true);
                    }
                    if (objRecord.CaseTypeGroup != caseTypeGroup)
                    {
                        if (caseTypeGroup != "")
                        {
                            if (ltHeader != null)
                                ltHeader.Text += "</tbody></table>";
                        }
                        caseTypeGroup = objRecord.CaseTypeGroup;
                        linenumber = 0;
                        if (ltHeader != null)
                            ltHeader.Text += GetFeesHeader(caseTypeGroup, false);
                    }
                }
            }
        }
        #endregion //Events


    }
}