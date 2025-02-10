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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
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
    public partial class SurveyReport : PretrialServicesModuleBase
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (QueryDate.HasValue)
                        txtReportDate.Text = QueryDate.Value.ToShortDateString();
                    BindData();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            BindData();
        }
        #endregion

        #region Methods
        private void BindData()
        {
            DateTime.TryParse(txtReportDate.Text, out DateTime reportDate);
            DateTime startDate = DateTimeExtensions.FirstDayOfWeek(reportDate);
            DateTime endDate = DateTimeExtensions.LastDayOfWeek(reportDate);
            if (hdRportDate.Value == "Y")
            {
                startDate = DateTimeExtensions.FirstDayOfYear(reportDate);
                endDate = DateTimeExtensions.LastDayOfYear(reportDate);
            }
            var ctl = new DefendantInProgramController();
            IEnumerable<DefendantInProgram> defendantInPrograms = ctl.GetDefendantsInProgram(startDate, endDate);
            lblScreened.Text = defendantInPrograms.Where(x => x.CaseScreened).Count().ToString();
            lblNotScreened.Text = defendantInPrograms.Where(x => x.CaseScreened == false).Count().ToString();
            lblPlacedSPR.Text = defendantInPrograms.Where(x => x.PlacedInProgram == false).Count().ToString();
            lblNotPlacedSPR.Text = defendantInPrograms.Where(x => x.PlacedInProgram).Count().ToString();
            lblMisdemeanor.Text = defendantInPrograms.Where(x => x.CaseType == (int)Enumerations.CaseCategoryValue.Misdemeanor).Count().ToString();
            lblFelony.Text = defendantInPrograms.Where(x => x.CaseType == (int)Enumerations.CaseCategoryValue.Felony).Count().ToString();
            lblNoBond.Text = defendantInPrograms.Where(x => x.BondType == (int)Enumerations.BondTypeValue.Secured).Count().ToString();
            lblWithBond.Text = defendantInPrograms.Where(x => x.BondType == (int)Enumerations.BondTypeValue.NonSecured).Count().ToString();
            lblBothBond.Text = defendantInPrograms.Where(x => x.BondType == (int)Enumerations.BondTypeValue.Both).Count().ToString();
            lblRevokedBond.Text = defendantInPrograms.Where(x => x.BondType == (int)Enumerations.BondTypeValue.Revoked).Count().ToString();
            lblUnsuccessfulCompletion.Text = defendantInPrograms.Where(x => x.Completion == (int)Enumerations.CompletionStatus.unsuccessful).Count().ToString();
            lblSuccessfulCompletion.Text = defendantInPrograms.Where(x => x.Completion == (int)Enumerations.CompletionStatus.successful).Count().ToString();
            lblOtherCompletion.Text = defendantInPrograms.Where(x => x.Completion == (int)Enumerations.CompletionStatus.other).Count().ToString();
            lblTotalExiting.Text = defendantInPrograms.Where(x => x.CompletionDate.HasValue).Count().ToString();
            var monthsSPR = defendantInPrograms.Select(x => x.MonthsSPR);
            if (monthsSPR != null)
                lblAverageLengthSPR.Text = monthsSPR.Average().ToString("0.##");
            else
                lblAverageLengthSPR.Text = "0";
            lblFtaSpr.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.FTA).Count().ToString();
            lblWarrantsFta.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.WarrantIssuedFTA).Count().ToString();
            lblSprRevokedFta.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.ReleaseRevokedFTA).Count().ToString();
            lblNewArrest.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.NewArrest).Count().ToString();
            lblReleaseRevokedNewOffense.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.ReleaseRevokedArrest).Count().ToString();
            lblNoComplaintsProgramConditions.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.SprNonCompliant).Count().ToString();
            lblWarrantNonCompliance.Text = defendantInPrograms.Where(x => x.NonCompliance == (int)Enumerations.ComplianceStatus.WarrantIssuedNonCompliant).Count().ToString();
            lblNumberCarriedOver.Text = defendantInPrograms.Where(x => x.CompletionDate.HasValue).Where(y => y.CompletionDate.Value.Year > y.IntakeDate.Value.Year | y.CompletionDate.HasValue == false).Count().ToString();
        }
        #endregion
    }
}
