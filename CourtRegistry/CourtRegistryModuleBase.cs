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

namespace tjc.Modules.CourtRegistry
{
    public class CourtRegistryModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public CourtRegistryModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }
        public string ApproverUsername
        {
            get
            {
                if (Settings.Contains("ApproverUsername"))
                    return Settings["ApproverUsername"].ToString();

                return "Azure-KMiller@jud12.flcourts.org";
            }
        }
        public string PublicUrl
        {
            get
            {
                if (Settings.Contains("PublicUrl"))
                    return Settings["PublicUrl"].ToString();

                return "https://jud12.flcourts.org/attorney-information/application";
            }
        }
        public int RequestedYear
        {
            get
            {
                var qs = Request.QueryString["yr"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int ApplicationId
        {
            get
            {
                var qs = Request.QueryString["aic"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int RequestedLocationId
        {
            get
            {
                var qs = Request.QueryString["loc"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public int _year { get; set; }
        public int _locationId { get; set; }
        public string LocationName
        {
            get
            {
                if (ViewState["LocationName"] != null)
                    return ViewState["LocationName"].ToString();
                else
                    return "";
            }
            set
            {
                ViewState["LocationName"] = value;
            }
        }
        public string ApplicationListUrl { get { return _navigationManager.NavigateURL(); } }
        public string ManageYearsUrl { get { return EditUrl("manage"); } }
        public string ExceptionListUrl { get { return EditUrl("exceptions"); } }
        public string BasicSettingsUrl { get { return EditUrl("basic-settings"); } }
        public string AttorneyListUrl { get { return EditUrl("attorneys"); } }
        public string JacCodeListUrl { get { return EditUrl("codes"); } }
        public string UpdateJacCodeUrl { get { return EditUrl("manage-codes"); } }
        public string LocationListUrl { get { return EditUrl("locations"); } }
        public string CaseTypeListUrl { get { return EditUrl("case-types"); } }

    }
}