/*
' Copyright (c) 2022  Joe Terhune
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
using tjc.Modules.CourtCounsel.Components;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace tjc.Modules.CourtCounsel
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from CourtCounselModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Library : CourtCounselModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public Library()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    lnkSearch.NavigateUrl = _navigationManager.NavigateURL();
                    
                    var tc = new LogEntryListController();
                    rptLogEntries.DataSource = tc.GetLogListItemsByUsername(UserInfo.Email);
                    rptLogEntries.DataBind();
                    if (UserInfo.IsInRole(AdminRole))
                        li1.Visible = true;
                    chkActive.InputAttributes.Add("class", "custom-control-input");
                    chkPending.InputAttributes.Add("class", "custom-control-input");
                    chkClosed.InputAttributes.Add("class", "custom-control-input");
                    PopulateDropDowns();
                    GetCookie();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        protected void PopulateDropDowns()
        {
            var ac = new MemberController();
            drpAttorney.DataValueField = "MemberId";
            drpAttorney.DataTextField = "ListName";
            IEnumerable<Member> activeMembers = ac.GetMembersByType(1, true);
            IEnumerable<Member> inActiveMembers = ac.GetMembersByType(1, false);
            drpAttorney.Items.Add(new ListItem("< Select Attorney >", "0"));
            foreach (Member member in activeMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                drpAttorney.Items.Add(li);
            }
            drpAttorney.Items.Add(new ListItem("Inactive Members", "<"));
            foreach (Member member in inActiveMembers)
            {
                ListItem li = new ListItem(member.ListName, member.MemberId.ToString());
                li.Attributes.Add("class", "inactive");
                drpAttorney.Items.Add(li);
            }
            drpAttorney.Items.Add(new ListItem("Inactive Members", ">"));
        }
        protected void rptLogEntries_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

        }

        protected void rptLogEntries_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }
        /// Stores multiple values in a Cookie using a key-value dictionary, creating the cookie (and/or the key) if it doesn't exists yet.
        /// </summary>
        /// <param name="cookieName">Cookie name</param>
        /// <param name="cookieDomain">Cookie domain (or NULL to use default domain value)</param>
        /// <param name="keyName">Cookie key name (if the cookie is a keyvalue pair): if NULL or EMPTY, this method will raise an exception since it's required when inserting multiple values.</param>
        /// <param name="values">Values to store into the cookie</param>
        /// <param name="expirationDate">Expiration Date (set it to NULL to leave default expiration date)</param>
        /// <param name="httpOnly">set it to TRUE to enable HttpOnly, FALSE otherwise (default: false)</param>
        /// <param name="sameSite">set it to 'None', 'Lax', 'Strict' or '(-1)' to not add it (default: '(-1)').</param>
        /// <param name="secure">set it to TRUE to enable Secure (HTTPS only), FALSE otherwise</param>
        public static void StoreInCookie(string cookieName, string cookieDomain, Dictionary<string, string> keyValueDictionary, DateTime? expirationDate, bool httpOnly = false, SameSiteMode sameSite = (SameSiteMode)(-1), bool secure = false)
        {
            // NOTE: we have to look first in the response, and then in the request.
            // This is required when we update multiple keys inside the cookie.
            HttpCookie cookie = HttpContext.Current.Response.Cookies.AllKeys.Contains(cookieName)
                ? HttpContext.Current.Response.Cookies[cookieName]
                : HttpContext.Current.Request.Cookies[cookieName];

            if (cookie == null) cookie = new HttpCookie(cookieName);
            if (keyValueDictionary == null || keyValueDictionary.Count == 0)
                cookie.Value = null;
            else
                foreach (var kvp in keyValueDictionary)
                    cookie.Values.Set(kvp.Key, kvp.Value);
            if (expirationDate.HasValue) cookie.Expires = expirationDate.Value;
            if (!String.IsNullOrEmpty(cookieDomain)) cookie.Domain = cookieDomain;
            cookie.HttpOnly = httpOnly;
            cookie.Secure = secure;
            cookie.SameSite = sameSite;
            HttpContext.Current.Response.Cookies.Set(cookie);
        }

        public void GetCookie()
        {
            if (Request.Cookies["SearchCookie"] != null)
            {
                var SearchCookie = Request.Cookies["SearchCookie"];
                var active = SearchCookie["Active"];
                var pending = SearchCookie["Pending"];
                var closed = SearchCookie["Closed"];
                var searchTerm = SearchCookie["SearchText"];
                var attorneyId = SearchCookie["AttorneyId"];
                var searchTypeText = SearchCookie["SearchType"];
                chkActive.Checked = bool.Parse(active);
                chkPending.Checked = bool.Parse(pending);
                chkClosed.Checked = bool.Parse(closed);
                txtSearchTerm.Text = searchTerm;
                drpAttorney.SelectedValue = attorneyId;
                hdSearchType.Value= searchTypeText;
                SearchType searchType = (SearchType)Int32.Parse(searchTypeText);
                BindData(searchType);
            }
        }
        protected void cmdSearch_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> keyValueDictionary = new Dictionary<string, string>();
            keyValueDictionary.Add("Active", chkActive.Checked.ToString());
            keyValueDictionary.Add("Pending", chkPending.Checked.ToString());
            keyValueDictionary.Add("Closed", chkClosed.Checked.ToString());
            keyValueDictionary.Add("SearchText", txtSearchTerm.Text);
            keyValueDictionary.Add("AttorneyId", drpAttorney.SelectedValue);
            keyValueDictionary.Add("SearchType", hdSearchType.Value);
            StoreInCookie("SearchCookie", null, keyValueDictionary, DateTime.Now.AddDays(30), false, SameSiteMode.Strict);
            SearchType searchType = (SearchType)Int32.Parse(hdSearchType.Value);
            BindData(searchType);

        }
        protected void BindData(SearchType searchType)
        {
            var tc = new LogEntryListController();

            switch (searchType)
            {
                case SearchType.recent:

                    rptLogEntries.DataSource = tc.GetLogListItemsByUsername(UserInfo.Email);

                    break;
                case SearchType.caseName:
                    rptLogEntries.DataSource = tc.GetLogListItemsBySearchText(txtSearchTerm.Text, SearchType.caseName);

                    break;
                case SearchType.caseNumber:
                    rptLogEntries.DataSource = tc.GetLogListItemsBySearchText(txtSearchTerm.Text, SearchType.caseNumber);

                    break;
                case SearchType.attorney:
                    rptLogEntries.DataSource = tc.GetLogListItemsByAttorney(Int32.Parse(drpAttorney.SelectedValue), chkActive.Checked, chkPending.Checked, chkClosed.Checked);

                    break;
                default:
                    break;
            }
            rptLogEntries.DataBind();
        }
    }
}