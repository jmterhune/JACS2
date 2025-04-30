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
using iText.Html2pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public partial class LocationList : CourtRegistryModuleBase
    {
        #region Methods
        private void BindData()
        {
            LocationName = drpLocations.SelectedItem.Text;
            ltSubHead.Text = string.Empty;
            if (drpJacCode.SelectedIndex > 0)
                ltSubHead.Text = string.Format("<h4 class='text-center'>Filtered for JAC Code: {0}</h4>", drpJacCode.SelectedItem.Text);
            else if (drpCategory.SelectedIndex > 0)
                ltSubHead.Text = string.Format("<h4 class='text-center'>Filtered for {0} Case Type</h4>", drpCategory.SelectedItem.Text);
            if (drpYear.Items.Count > 0)
                _year = Int32.Parse(drpYear.SelectedValue);
            var ctl = new AttorneyController();
            IEnumerable<RegistryListItem> registryList = new List<RegistryListItem>();
            Int32.TryParse(drpJacCode.SelectedValue, out int jacCode);
            Int32.TryParse(drpCategory.SelectedValue, out int caseTypeId);
            if (drpJacCode.SelectedIndex > 0)
                registryList = ctl.GetAttorneyRegistry(_locationId, _year, 0, jacCode);
            else if (drpCategory.SelectedIndex > 0)
                registryList = ctl.GetAttorneyRegistry(_locationId, _year, caseTypeId, 0);
            else
                registryList = ctl.GetAttorneyRegistry(_locationId, _year, 0, 0);
            rptAttorney.DataSource = registryList;
            rptAttorney.DataBind();
            ltHeader.Text = string.Format("{0} - {1} JAC Registry Contract Attorneys", _year - 1, _year);
        }
        private void BindDropdownLists()
        {
            var cCtl = new CaseTypeController();
            IEnumerable<CaseType> categories = cCtl.GetCaseTypes().OrderBy(c => c.CaseTypeName);
            var jCtl = new JacCodeController();

            IEnumerable<JacCode> jacCodes = jCtl.GetJacCodes().OrderBy(x => x.JacCodeID);
            foreach (CaseType c in categories)
                drpCategory.Items.Add(new ListItem(c.CaseTypeName, c.CaseTypeID.ToString()));
            drpCategory.Items.Insert(0, new ListItem("ALL", ""));

            foreach (JacCode j in jacCodes)
                drpJacCode.Items.Add(new ListItem(string.Format("{0} ({1})", j.JacCodeID, j.Category), j.JacCodeID.ToString()));
            drpJacCode.Items.Insert(0, new ListItem("ALL", ""));
            if (drpYear.Items.Count <= 0)
            {
                drpYear.Items.Add(new ListItem(DateTime.Now.AddYears(-1).Year.ToString()));
                drpYear.Items.Add(new ListItem(DateTime.Now.Year.ToString()));
                drpYear.Items.Add(new ListItem(DateTime.Now.AddYears(1).Year.ToString()));
            }
        }
        private void BindDropdownLists(int casetypeId)
        {
            drpJacCode.Items.Clear();
            var jCtl = new JacCodeController();
            IEnumerable<JacCode> jacCodes = jCtl.GetJacCodesByCaseType(casetypeId).OrderBy(x => x.JacCodeID);
            foreach (var j in jacCodes)
                drpJacCode.Items.Add(new ListItem(string.Format("{0} ({1})", j.JacCodeID, j.Category), j.JacCodeID.ToString()));
            drpJacCode.Items.Insert(0, new ListItem("ALL", ""));
            if (drpYear.Items.Count <= 0)
            {
                drpYear.Items.Add(new ListItem(DateTime.Now.AddYears(-1).Year.ToString()));
                drpYear.Items.Add(new ListItem(DateTime.Now.Year.ToString()));
                drpYear.Items.Add(new ListItem(DateTime.Now.AddYears(1).Year.ToString()));
            }
        }

        #endregion
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                bool hasLocation = false;
                if (!IsPostBack)
                {
                    var ctl = new LocationController();
                    IEnumerable<Location> locations = ctl.GetLocations();
                    foreach (Location location in locations)
                        drpLocations.Items.Add(new ListItem(location.LocationName, location.LocationID.ToString()));
                    BindDropdownLists();
                    drpYear.SelectedValue = _year.ToString();
                }
                if (RequestedYear > 0)
                    _year = RequestedYear;
                else
                {
                    var sCtl = new SettingController();
                    Setting setting = sCtl.GetSettings().FirstOrDefault();
                    var beginFiscalYear = new DateTime(DateTime.Now.Year, setting.BeginFiscalYearMonth, setting.BeginFiscalYearDay);
                    if (beginFiscalYear < DateTime.Now)
                        _year = DateTime.Now.Year + 1;
                    else
                        _year = DateTime.Now.Year;
                }
                if (drpLocations.Items.Count > 0)
                    hasLocation = true;
                if (RequestedLocationId > 0 & hasLocation == false)
                {
                    _locationId = RequestedLocationId;
                    drpLocations.SelectedValue = _locationId.ToString();
                }
                else if (hasLocation)
                    _locationId = Int32.Parse(drpLocations.SelectedValue);
                if (!IsPostBack)
                    BindData();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void drpCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(drpCategory.SelectedValue, out int caseTypeId))
                BindDropdownLists(caseTypeId);
            else
                BindDropdownLists();
        }
        protected void rptAttorney_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            RepeaterItem item = e.Item;
            if ((item.ItemType == ListItemType.Item) || (item.ItemType == ListItemType.AlternatingItem))
            {
                var rptJacCodes = (Repeater)item.FindControl("rptJacCodes");
                RegistryListItem registryItem = (RegistryListItem)item.DataItem;
                var ctl = new AttorneyController();
                IEnumerable<JacCode> jacCodes = ctl.GetAttorneyJacCode(registryItem.AttorneyID, _locationId, _year);
                if (Int32.TryParse(drpJacCode.SelectedValue, out int jacCodeId))
                    jacCodes = jacCodes.Where(j => j.JacCodeID == jacCodeId);
                else if (Int32.TryParse(drpCategory.SelectedValue, out int caseTypeId))
                    jacCodes = jacCodes.Where(j => j.CaseTypeID == caseTypeId);
                rptJacCodes.DataSource = jacCodes.OrderBy(j => j.Category);
                rptJacCodes.DataBind();
            }
        }
        protected void cmdShow_Click(object sender, EventArgs e)
        {
            BindData();
        }
        protected void cmdPrint_Click(object sender, EventArgs e)
        {
            try
            {
                string html = "";
                string fileName = ltHeader.Text + " - " + LocationName;
                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                pnlList.RenderControl(hw);
                html = sw.ToString();
                Response.Clear();
                Response.ContentType = "Application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".pdf");
                HtmlConverter.ConvertToPdf(html, Response.OutputStream);
                Response.Flush();
                Response.Close();
                Response.End();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion


    }
}