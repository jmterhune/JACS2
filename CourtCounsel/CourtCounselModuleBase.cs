/*
' Copyright (c) 2026 Joe Terhune
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

namespace tjc.Modules.CourtCounsel
{
    public class CourtCounselModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;

        public CourtCounselModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }

        public string AdminRole
        {
            get
            {
                if (Settings.Contains("AdminRole"))
                    return Settings["AdminRole"].ToString();
                return "";
            }
        }

        public string TemplateText
        {
            get
            {
                if (Settings.Contains("template"))
                    return Settings["template"].ToString();
                return "";
            }
        }

        public int LogId
        {
            get
            {
                var qs = Request.QueryString["lid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }

        public string CaseNumber
        {
            get
            {
                var qs = Request.QueryString["cn"];
                return qs ?? string.Empty;
            }
        }

        public string PartyName
        {
            get
            {
                var qs = Request.QueryString["pn"];
                return qs ?? string.Empty;
            }
        }

        public string AttorneyName
        {
            get
            {
                var qs = Request.QueryString["att"];
                return qs ?? string.Empty;
            }
        }

        public string StatusFilter
        {
            get
            {
                var qs = Request.QueryString["sf"];
                return qs ?? string.Empty;
            }
        }

        public bool IsFutureAction
        {
            get
            {
                var qs = Request.QueryString["fa"];
                return qs != null;
            }
        }

        public bool IsAdmin
        {
            get
            {
                if (UserId > 0)
                    return UserInfo.IsInRole(AdminRole);
                return false;
            }
        }

        public string SearchUrl { get { return _navigationManager.NavigateURL(); } }
        public string DataEntryUrl { get { return EditUrl("EditHistory"); } }
        public string ReportsUrl { get { return EditUrl("Reports"); } }
        public string DataSheetUrl { get { return EditUrl("DataSheet"); } }
        public string AdminUrl { get { return EditUrl("Admin"); } }
        public string UpdateCaseNameUrl { get { return EditUrl("UpdateCaseName"); } }
    }
}
