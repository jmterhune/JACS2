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
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.MediationStatistics.Components;

namespace tjc.Modules.MediationStatistics
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from MediationStatisticsModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : MediationStatisticsModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public string isAdminUser = "false";
        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private void PopulateDropdowns()
        {
            var ctlRegion = new RegionController();
            var ctlGroup = new GroupController();
            IEnumerable<Group> groups = ctlGroup.GetGroups().OrderByDescending(y => y.CourtOrdered).ThenBy(x => x.Description);
            drpGroup.Items.Add(new ListItem("Court Ordered", "<"));
            bool isFirstNonCourtOrdered = false;
            foreach (Group group in groups)
            {
                ListItem listItem = new ListItem(group.Description, group.GroupId.ToString());

                if (group.CourtOrdered.HasValue)
                {
                    if (group.CourtOrdered.Value)
                    {
                        listItem.Attributes.Add("data-co", "1");
                        drpGroup.Items.Add(listItem);
                    }
                    else
                    {
                        if (!isFirstNonCourtOrdered)
                        {
                            drpGroup.Items.Add(new ListItem("Court Ordered", ">"));
                            drpGroup.Items.Add(new ListItem("Not Court Ordered", "<"));
                            isFirstNonCourtOrdered = true;
                        }
                        listItem.Attributes.Add("data-co", "0");
                        drpGroup.Items.Add(listItem);
                    }
                }
            }
            drpGroup.Items.Add(new ListItem("Not Court Ordered", ">"));
            drpRegion.DataSource = ctlRegion.GetRegions().OrderBy(x => x.Description);
            drpRegion.DataTextField = "Description";
            drpRegion.DataValueField = "RegionId";
            drpRegion.DataBind();
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    isAdminUser = UserId > 0 ? UserInfo.IsAdmin.ToString().ToLower() : "false";
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    PopulateDropdowns();
                    lnkReset.NavigateUrl = _navigationManager.NavigateURL();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void cmdAddCase_Click(object sender, EventArgs e)
        {
            Case newCase = new Case
            {
                CaseNumber = GetCaseNumber(),
                CDSPNumber = GetCDSPNumber(),
                CreatedById = UserId,
                CreatedDate = DateTime.Now,
                LastModifiedById = UserId,
                LastModifiedDate = DateTime.Now,
                GroupId = Int32.Parse(drpGroup.SelectedValue),
                RegionId = Int32.Parse(drpRegion.SelectedValue),
                p1_business = txtBusinessName.Text,
                p1_FirstName = txtFirstName.Text,
                p1_LastName = txtLastName.Text,
            };
            try
            {
                var ctl = new CaseController();
                ctl.CreateCase(newCase);
                NavigateToCase(newCase.GroupEnum,newCase.CaseId);
            }
            catch (Exception exc)
            {
                ltMessage.Text =string.Format("<div class='alert alert-danger'><i class='fa fa-warning'>{0}</i>", exc.Message);
            }
        }

        private string GetCaseNumber()
        {
            string caseNumber = "";
            caseNumber += txtCaseYear.Text;

            var year = txtCaseYear.Text;
            var code = txtCaseType.Text;
            var number = txtCaseSequence.Text;
            var suffix = txtSuffix.Text;
            if (year.Length == 2)
                year = "20" + year;
            if (number.Length > 0)
                number = number.ToString().PadRight(6, '0');
            if (year.Length > 0 & code.Length > 0 && number.Length > 0)
                caseNumber = string.Format("{0} {1} {2} {3}", year, code.ToUpper(), number, suffix.ToUpper());
            return caseNumber;
        }
        private string GetCDSPNumber()
        {
            string cdspNumber = "";
            var type = drpCDSPType.SelectedValue;
            var year = txtCDSPYear.Text;
            var number = txtCDSPNumber.Text;
            var location = drpCountyLetter.SelectedValue;
            if (type.Length > 0) { cdspNumber += "-"; } else { return null; }
            if (year.Length > 0) { cdspNumber += year + "-"; } else { return null; }
            if (number.Length > 0) { cdspNumber += number + "-"; } else { return null; }
            if (location.Length > 0)
                cdspNumber += location;
            if (cdspNumber != null && cdspNumber.EndsWith("-"))
                cdspNumber = cdspNumber.Trim('-');

            return cdspNumber;
        }
        private void NavigateToCase(GroupType selectedCaseType, int selectedCaseId)
        {
            string navigateUrl = EditUrl("cid", selectedCaseId.ToString(), selectedCaseType.ToString()); ;
            Response.Redirect(navigateUrl,true);
        }

        #endregion
    }
}