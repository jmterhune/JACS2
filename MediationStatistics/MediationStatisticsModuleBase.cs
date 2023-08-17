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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace tjc.Modules.MediationStatistics
{
    public class MediationStatisticsModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public MediationStatisticsModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }

        public int CaseID
        {
            get
            {
                var qs = Request.QueryString["cid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public int GroupID
        {
            get
            {
                var qs = Request.QueryString["gid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public int SessionID
        {
            get
            {
                var qs = Request.QueryString["sid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public int SessionIndex
        {
            get
            {
                var qs = Request.QueryString["sidx"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public int RegionID
        {
            get
            {
                var qs = Request.QueryString["rid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
        public string CookieName { get; set; }
        public string CaseListUrl { get { return _navigationManager.NavigateURL(); } }
        public string CDSPUrl { get { return EditUrl("CDSP"); } }
        public string AttorneyListUrl { get { return EditUrl("Attorney"); } }
        public string ReportUrl { get { return EditUrl("Report"); } }
        public string RegionListUrl { get { return EditUrl("Region"); } }
        public string GroupListUrl { get { return EditUrl("Group"); } }
        public string CaseTypeListUrl { get { return EditUrl("CaseType"); } }
        public string AppearanceListUrl { get { return EditUrl("Appearance"); } }
        public string IssueListUrl { get { return EditUrl("Issue"); } }
        public string ActionListUrl { get { return EditUrl("Action"); } }
        public string GroupRelationUrl { get { return EditUrl("GroupRelation"); } }

        public bool IsAdmin
        {
            get
            {
                if (UserInfo.IsInRole(AdminRole))
                    return true;
                return false;
            }
        }
        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();

                return "Mediation Manager";
            }
        }
        public enum CookieItems
        {
            SelectedCaseId,
            RegionId,
            CaseGroupId,
            CurrentPageIndex,
            SortOrder,
            SortDirection,
        }
    }
}