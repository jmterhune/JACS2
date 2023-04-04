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
            var tc = new ExcludedAttorneysController();
            rptAttorneyList.DataSource = tc.GetAttorneyView();
            rptAttorneyList.DataBind();
        }

        protected void rptAttorneyList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var tc = new ExcludedAttorneysController();
                tc.DeleteAttorney(Convert.ToInt32(e.CommandArgument.ToString()));
                Response.Redirect(_navigationManager.NavigateURL(),true);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            ExcludedAttorney excludedAttorney=new ExcludedAttorney { barnumber=txtBarNumber.Text.PadLeft(7,'0')};
            var ctl = new ExcludedAttorneysController();
            ctl.CreateAttorney(excludedAttorney);
            BindAttorneyList();
        }
    }
}