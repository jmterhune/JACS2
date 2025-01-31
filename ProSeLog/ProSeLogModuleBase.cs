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
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework.JavaScriptLibraries;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.ProSeLog
{
    public class ProSeLogModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public ProSeLogModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }
        public int HistoryId
        {
            get
            {
                var qs = Request.QueryString["hid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public bool IsCopy
        {
            get
            {
                var qs = Request.QueryString["copy"];
                if (qs != null)
                    return true;
                return false;
            }
        }
        public int CaseTypeId
        {
            get
            {
                var qs = Request.QueryString["ctid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string CaseNumber
        {
            get
            {
                var qs = Request.QueryString["case"];
                if (qs != null)
                    return qs;
                return "";
            }
        }
        public int ContactId
        {
            get
            {
                var qs = Request.QueryString["cid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
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
        public string CaseTypeListUrl { get { return EditUrl("casetype"); } }
        public string ContactListUrl { get { return EditUrl("contact"); } }
        public string LogListUrl { get { return _navigationManager.NavigateURL(); } }
        public string FormUrl { get { return EditUrl("form"); } }
        public string StatsUrl { get { return EditUrl("stats"); } }

    }
}