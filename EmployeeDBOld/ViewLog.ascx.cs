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

using DotNetNuke.Abstractions;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using tjc.Modules.EmployeeDB.Components;

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
    public partial class ViewLog : EmployeeDBModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public ViewLog()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindLog(DateTime startDate, DateTime endDate)
        {

            var ctl = new SwnLogController();
            rptLog.DataSource = ctl.GetSwnLogList(startDate, endDate);
            rptLog.DataBind();
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    JavaScript.RequestRegistration(CommonJs.jQuery);

                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);

                    DateTime startDate = DateTime.Now.AddDays(-30);
                    DateTime endDate = DateTime.Now;
                    txtEndDate.Text = endDate.ToShortDateString();
                    txtStartDate.Text = startDate.ToShortDateString();
                    BindLog(startDate, endDate);
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion

        protected void cmdFilter_Click(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtEndDate.Text, out DateTime endDate) & DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
            {
                BindLog(startDate, endDate);

            }
            else { DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Unable to Filter Log. Please ensure that the Start and End Dates are entered correctly", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError); }

        }

        protected void cmdClearLog_Click(object sender, EventArgs e)
        {
            var ctl =new SwnLogController();
            ctl.ClearLog();
            if (DateTime.TryParse(txtEndDate.Text, out DateTime endDate) & DateTime.TryParse(txtStartDate.Text, out DateTime startDate))
            {
                BindLog(startDate, endDate);

            }
        }
    }
}