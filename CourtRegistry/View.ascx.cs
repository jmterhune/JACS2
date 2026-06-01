/*
' Copyright (c) 2025  Joe Terhune
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
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtRegistryModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : CourtRegistryModuleBase
    {
        private void BindLists()
        {
            var ctl = new ApplicationController();
            drpYear.DataTextField = "PeriodYear";
            drpYear.DataValueField = "ApplicationYear";
            drpYear.DataSource = ctl.GetApplicationPeriods();
            drpYear.DataBind();
            var statusTypes = Enumerations.GetValues<ApplicationStatus>();
            foreach (var statusType in statusTypes) {
                string statusTypeId = ((int)statusType).ToString();
                drpStatus.Items.Add(new ListItem(Enumerations.GetEnumDescription(statusType), statusTypeId));
            }
            if (drpStatus.Items.Count > 0) {
                drpStatus.Items.Insert(0, new ListItem("Every Status", "-1"));
            }
            if (drpYear.Items.Count > 0)
            {
                drpYear.Items.Insert(0, new ListItem("All Periods", "-1"));
            }
            drpStatus.SelectedValue = "-1";
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack) {
                    BindLists();
                }
               
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}