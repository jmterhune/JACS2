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
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;
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
    public partial class JacExceptions : CourtRegistryModuleBase
    {
        #region Methods
        private readonly INavigationManager _navigationManager;
        private readonly IHostSettings _hostSettings;
        public JacExceptions()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }
        private void BindDropDowns()
        {
            // Bind dropdowns here if needed
            var ctl = new JacCodeController(_hostSettings);
            var ctlP = new ApplicationController(_hostSettings);
            var ctlC = new CaseTypeController(_hostSettings);
            var ctlL = new LocationController(_hostSettings);
            drpPeriod.DataTextField = "PeriodYear";
            drpPeriod.DataValueField = "ApplicationYear";
            drpPeriod.DataSource = ctlP.GetApplicationPeriods().OrderByDescending(x => x.PeriodYear);
            drpPeriod.DataBind();
            drpCategory.DataTextField = "CaseTypeName";
            drpCategory.DataValueField = "CaseTypeID";
            drpCategory.DataSource = ctlC.GetCaseTypes().OrderBy(x => x.CaseTypeName);
            drpCategory.DataBind();
            drpLocation.DataTextField = "LocationName";
            drpLocation.DataValueField = "LocationID";
            drpLocation.DataSource = ctlL.GetLocations().OrderBy(x => x.LocationName);
            drpLocation.DataBind();
            drpCode.DataTextField = "JacCodeListName";
            drpCode.DataValueField = "JacCodeID";
            drpCode.DataSource = ctl.GetJacCodes().OrderBy(x => x.JacCodeID);
            drpCode.DataBind();
        }
        private void BindDropDowns(int categorId)
        {
            var ctl = new JacCodeController(_hostSettings);
            drpCode.DataTextField = "JacCodeListName";
            drpCode.DataValueField = "JacCodeID";
            drpCode.Items.Clear();
            drpCode.DataSource = ctl.GetJacCodesByCaseType(categorId).OrderBy(x => x.JacCodeID);
            drpCode.DataBind();
            drpCode.Items.Insert(0, new ListItem("ALL", ""));
        }
        private void BindList()
        {
            var ctl = new JacCodeController(_hostSettings);
            if (drpPeriod.Items.Count > 0)
            {
                int year = int.Parse(drpPeriod.SelectedValue);
                var exceptions = ctl.GetJacExceptions(year);
                rptExclusions.DataSource = exceptions.OrderBy(x => x.JacCodeID);
                rptExclusions.DataBind();
            }
            else
            {
                rptExclusions.DataSource = null;
                rptExclusions.DataBind();
            }
        }
        private void AddJacCodes(int jacCodeId,int year)
        {
            var ctl = new LocationController(_hostSettings);
            var ctlJac = new JacCodeController(_hostSettings);

            if (drpLocation.SelectedIndex == 0)
            {
                IEnumerable<Location> locations = ctl.GetLocations();
                foreach (Location loc in locations)
                {
                    JacCodeConfig setting = ctlJac.GetJacCodeConfig(jacCodeId, loc.LocationID, year);
                    if (setting != null)
                    {
                        setting.Exclude = chkExclude.Checked;
                        setting.OnlyRenewals = chkRenewal.Checked;
                        ctlJac.UpdateJacCode(setting);
                    }
                    else
                    {
                        setting = new JacCodeConfig
                        {
                            JacCodeID = jacCodeId,
                            LocationID = loc.LocationID,
                            Year = year,
                            Exclude = chkExclude.Checked,
                            OnlyRenewals = chkRenewal.Checked
                        };
                        ctlJac.CreateJacCodeConfig(setting);
                    }
                }
            }
            else
            {
                int locationId = int.Parse(drpLocation.SelectedValue);
                JacCodeConfig setting = ctlJac.GetJacCodeConfig(jacCodeId, locationId, year);
                if (setting != null)
                {
                    setting.Exclude = chkExclude.Checked;
                    setting.OnlyRenewals = chkRenewal.Checked;
                    ctlJac.UpdateJacCode(setting);
                }
                else
                {
                    setting = new JacCodeConfig
                    {
                        JacCodeID = jacCodeId,
                        LocationID = locationId,
                        Year = year,
                        Exclude = chkExclude.Checked,
                        OnlyRenewals = chkRenewal.Checked
                    };
                    ctlJac.CreateJacCodeConfig(setting);
                }
            }
        }
        #endregion
        #region Events
     
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {                    
                _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
                if (!Page.IsPostBack)
                {
                    BindDropDowns();
                    BindList();
                    lnkReturn.NavigateUrl = _navigationManager.NavigateURL();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdAddCodes_Click(object sender, EventArgs e)
        {
            if (drpPeriod.Items.Count > 0) { 
             int.TryParse(drpPeriod.SelectedValue, out int year);
                if (drpCode.SelectedIndex == 0)
                {
                   foreach (ListItem item in drpCode.Items)
                    {
                        if (item.Value!="")
                        {
                            int.TryParse(item.Value, out int jacCodeId);
                            AddJacCodes(jacCodeId, year);
                        }
                    }
                }
                else { 
                    int.TryParse(drpCode.SelectedValue, out int jacCodeId);
                    AddJacCodes(jacCodeId, year);
                }
            }
            BindList();
        }

        protected void cmdClearExtensions_Click(object sender, EventArgs e)
        {
            string year = drpPeriod.SelectedValue;
            if (int.TryParse(year, out int appYear) && appYear > 0)
            {
                var ctl = new JacCodeController(_hostSettings);
                ctl.ClearExceptions(appYear);
                BindList();
            }
        }

        protected void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string categoryId = drpCategory.SelectedValue;
            if (int.TryParse(categoryId, out int catId) && catId > 0)
            {
                BindDropDowns(catId);
            }
        }

        protected void pnlExceptions_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });

        }

        protected void valCheckedOne_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            if (chkExclude.Checked || chkRenewal.Checked)
                args.IsValid = true;
        }

        protected void rptExclusions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                var keys = e.CommandArgument.ToString().Split('|');
                int jacCodeId = int.Parse(keys[0]);
                int applicationYear = int.Parse(keys[2]);
                int locationId = int.Parse(keys[1]);
                var ctl = new JacCodeController(_hostSettings);
                ctl.DeleteException(jacCodeId, locationId, applicationYear);
                BindList();
            }
        }

        protected void drpPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindList();
        }
       #endregion
    }
}