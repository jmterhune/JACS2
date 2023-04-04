/*
' Copyright (c) 2023  jterhune
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
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.JacsCaseMaint.Components;

namespace tjc.Modules.JacsCaseMaint
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ManateeJacsCaseMaintenaceModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : CaseMaintBase
    {
        public int CurrentCaseCycleId
        {
            get
            {
                if (ViewState["CurrentCaseCycleId"] == null)
                    return 0;
                return Convert.ToInt32(ViewState["CurrentCaseCycleId"]);
            }
            set { ViewState["CurrentCaseCycleId"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        private void BindCaseCycleList(int caseId)
        {
            var tc = new CaseCycleController();
            List<CaseCycle> caseCycles = new List<CaseCycle>();
            if (caseId <= 0)
            {
                caseCycles = tc.GetCaseCyles(txtYear.Text, txtCaseType.Text, txtSequence.Text).ToList();
            }
            else
            {
                CaseCycle caseCycle = tc.GetCaseCycleByCaseId(caseId);
                caseCycles.Add(caseCycle);
            }
            rptCaseCycle.DataSource = caseCycles;
            rptCaseCycle.DataBind();
        }
        private void BindCaseList()
        {
            var tc = new InterfaceMessageController();
            rptInterfaceList.DataSource = tc.GetMessages(txtYear.Text, txtCaseType.Text, txtSequence.Text);
            rptInterfaceList.DataBind();
            var cl = new CaseController();
            IEnumerable<Cases> cases = cl.GetCases(txtYear.Text, txtCaseType.Text, txtSequence.Text);
            rptCaseList.DataSource = cases;
            rptCaseList.DataBind();
        }

        protected void cmdSubmit_Click(object sender, EventArgs e)
        {
            BindCaseList();
        }

        protected void rptCaseCycle_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var tc = new CaseCycleController();
                tc.DeleteCaseCycle(Convert.ToInt32(e.CommandArgument));
                BindCaseCycleList(CurrentCaseCycleId);
            }
        }

        protected void rptCaseList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var tc = new CaseController();
                tc.DeleteCase(e.CommandArgument.ToString());
                BindCaseList();
            }
        }

        protected void rptInterfaceList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "scc")
            {
                CurrentCaseCycleId = Convert.ToInt32(e.CommandArgument);
                BindCaseCycleList(Convert.ToInt32(e.CommandArgument));
            }
        }
    }
}