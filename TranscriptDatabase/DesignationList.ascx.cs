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
using DotNetNuke.Common.Lists;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.TranscriptDatabase.Components;

namespace tjc.Modules.TranscriptDatabase
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from TranscriptDatabaseModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class DesignationList : TranscriptDatabaseModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;

        #endregion
        #region Methods
        public DesignationList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        private void BindDropDowns()
        {
            var hCtl = new HearingTypeController();
            var jCtl = new EmployeeController();
            var oCtl = new OfficeController();
            drpOffice.DataTextField = "Description";
            drpOffice.DataValueField = "OfficeID";
            drpOffice.DataSource = oCtl.GetOffices().OrderBy(x => x.Description);
            drpOffice.DataBind();
            drpHearingType.DataTextField = "HearingTypeName";
            drpHearingType.DataValueField = "HearingTypeName";
            drpHearingType.DataSource = hCtl.GetHearingTypes().OrderBy(x => x.HearingTypeName);
            drpHearingType.DataBind();
            var ctl = new ListController();
            IEnumerable<ListEntryInfo> states = ctl.GetListEntryInfoItems("Region", "Country.US");
            drpState.DataSource = states;
            drpState.DataTextField = "Text";
            drpState.DataValueField = "Value";
            drpState.DataBind();
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                JavaScript.RequestRegistration(CommonJs.DnnPlugins);
                if (!Page.IsPostBack)
                {
                    BindDropDowns();
                    txtReceiptDate.Text= DateTime.Now.ToShortDateString();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}