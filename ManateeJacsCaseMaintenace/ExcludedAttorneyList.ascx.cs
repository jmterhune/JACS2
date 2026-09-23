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
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;
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
    public partial class ExcludedAttorneyList : CaseMaintBase
    {
        private readonly INavigationManager _navigationManager;
        public ExcludedAttorneyList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    BindAttorneyList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
       
        private void BindAttorneyList()
        {
            var tc = new ExcludedAttorneysController(_hostSettings);
            rptAttorneyList.DataSource = tc.GetAttorneyView();
            rptAttorneyList.DataBind();
        }

        protected void rptAttorneyList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var tc = new ExcludedAttorneysController(_hostSettings);
                tc.DeleteAttorney(Convert.ToInt32(e.CommandArgument.ToString()));
                Response.Redirect(_navigationManager.NavigateURL(),true);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            ExcludedAttorney excludedAttorney=new ExcludedAttorney { barnumber=txtBarNumber.Text.PadLeft(7,'0')};
            var ctl = new ExcludedAttorneysController(_hostSettings);
            ctl.CreateAttorney(excludedAttorney);
            BindAttorneyList();
        }

        #region Attorney Status tab
        private const string TAB_SEARCH = "search";

        public string CurrentSearch
        {
            get { return ViewState["CurrentSearch"] as string ?? string.Empty; }
            set { ViewState["CurrentSearch"] = value; }
        }

        private void BindSearchResults()
        {
            hdnActiveTab.Value = TAB_SEARCH;
            if (string.IsNullOrEmpty(CurrentSearch))
            {
                rptSearchResults.Visible = false;
                pnlNoResults.Visible = false;
                return;
            }
            var ctl = new AttorneyController(_hostSettings);
            var attorneys = ctl.GetAttorneyByBarNumber(CurrentSearch).ToList();
            rptSearchResults.DataSource = attorneys;
            rptSearchResults.DataBind();
            rptSearchResults.Visible = attorneys.Count > 0;
            pnlNoResults.Visible = attorneys.Count == 0;
            litNoResultsBarNumber.Text = HttpUtility.HtmlEncode(CurrentSearch);
        }

        protected void rptSearchResults_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var chk = (CheckBox)e.Item.FindControl("chkActive");
                var attorney = (Attorney)e.Item.DataItem;
                chk.InputAttributes["class"] = "form-check-input";
                chk.InputAttributes["role"] = "switch";
                chk.InputAttributes["aria-label"] = string.Format("Active status for {0}", attorney.BARNUM);
                chk.InputAttributes["title"] = attorney.IsActive ? "Active - click to deactivate" : "Inactive - click to activate";
            }
        }

        protected void chkActive_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = (CheckBox)sender;
                var item = (RepeaterItem)chk.NamingContainer;
                var barNumber = ((HiddenField)item.FindControl("hdnBarNumber")).Value;

                var ctl = new AttorneyController(_hostSettings);
                ctl.SetActive(barNumber, chk.Checked);

                ShowToast(string.Format("Bar number {0} set to {1}.", AttorneyController.PadBarNumber(barNumber), chk.Checked ? "active (Y)" : "inactive (N)"), "success");
                BindSearchResults();
                // ACTIVE is also displayed on the excluded list
                BindAttorneyList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            var search = txtSearchBarNumber.Text.Trim();
            CurrentSearch = search.Length == 0 ? string.Empty : AttorneyController.PadBarNumber(search);
            txtSearchBarNumber.Text = CurrentSearch;
            BindSearchResults();
        }

        protected void cmdClear_Click(object sender, EventArgs e)
        {
            txtSearchBarNumber.Text = string.Empty;
            CurrentSearch = string.Empty;
            BindSearchResults();
        }

        private void ShowToast(string message, string type)
        {
            var script = string.Format(
                "document.addEventListener('DOMContentLoaded', function () {{ if (window.Noty) {{ new Noty({{ text: '{0}', type: '{1}', theme: 'bootstrap-v4', timeout: 3000, layout: 'topRight' }}).show(); }} }});",
                HttpUtility.JavaScriptStringEncode(message), type);
            Page.ClientScript.RegisterStartupScript(GetType(), "AttorneyStatusToast", script, true);
        }
        #endregion
    }
}