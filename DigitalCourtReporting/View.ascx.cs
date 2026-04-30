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

using DotNetNuke.Abstractions;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using tjc.Modules.DigitalCourtReporting.Components;

namespace tjc.Modules.DigitalCourtReporting
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from DigitalCourtReportingModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : DigitalCourtReportingModuleBase
    {

        #region Properties
        private readonly INavigationManager _navigationManager;
        #endregion

        #region Methods
        public View()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        public string GetProceedingUrl(string proceedingIdString)
        {
            string navString = "";
            switch (ListType)
            {
                case ListTypes.cdCreation:
                    {
                        navString = EditUrl("proceedingId", proceedingIdString, "EditDCR");
                        break;
                    }

                case ListTypes.completed:
                    {
                        navString = EditUrl("proceedingId", proceedingIdString, "Complete");
                        break;
                    }

                case ListTypes.notification:
                    {
                        navString = EditUrl("proceedingId", proceedingIdString, "EditNotification");
                        break;
                    }

                case ListTypes.payment:
                    {
                        navString = EditUrl("proceedingId", proceedingIdString, "EditAccounting");
                        break;
                    }

                case ListTypes.inquiry:
                    {
                        navString = EditUrl("proceedingId", proceedingIdString, "EditInquiry");
                        break;
                    }
            }
            if (SearchText != "")
                navString = EditUrl("proceedingId", proceedingIdString, "Complete");
            return navString;
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {



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