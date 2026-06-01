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
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.RecordDestruction.Components;

namespace tjc.Modules.RecordDestruction 
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
    public partial class GroupList : RecordDestructionModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public GroupList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        private void BindList()
        {
            var ctl = new GroupController();
            rptGroups.DataSource = ctl.GetGroups().OrderBy(x=>x.GroupName);
            rptGroups.DataBind();
        }
        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
    }
}