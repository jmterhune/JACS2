/*
' Copyright (c) 2022  Joe Terhune
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
using tjc.Modules.CourtCounsel.Components;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Framework.JavaScriptLibraries;

namespace tjc.Modules.CourtCounsel
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtCounselModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class CaseView : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public CaseView()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void BindList()
        {
            var tc = new LogEntryController();
            LogEntry log = tc.GetLogEntry(LogId);
            string caseNumber = log.CaseNumber.Substring(0, 16);
            var lc = new LogEntryListController();
            IEnumerable<LogEntryListItem> logEntries = lc.GetLogListItemsBySearchText(log.CaseNumber, SearchType.caseNumber);
            var caseNames = logEntries.OrderBy(x => x.Description).Select(x => x.Description).Distinct();
            rptLogEntries.DataSource = logEntries;
            rptLogEntries.DataBind();
            ltCaseHeading.Text = String.Format("<h4>Case Number: {0}</h4>", caseNumber);

        }
        protected void rptLogEntries_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int assignmentId = Convert.ToInt32(e.CommandArgument);
            var ctl = new AssignmentController();
            if (e.CommandName == "delete")
            {
                ctl.DeleteAssignment(assignmentId);
                BindList();
            }
        }

        protected void rptLogEntries_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                LinkButton cmdDelete = (LinkButton)e.Item.FindControl("cmdDelete");
                scriptMan.RegisterAsyncPostBackControl(cmdDelete);
                if (!IsAdmin) { cmdDelete.Visible = false; }
            }
        }

        protected void cmdDuplicate_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl("lid",LogId.ToString(),"LogEdit"),true);
        }
    }
}