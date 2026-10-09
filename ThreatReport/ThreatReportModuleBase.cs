/*
' Copyright (c) 2019  jud12
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
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    public class ThreatReportModuleBase : PortalModuleBase
    {
        /// <summary>Role on this (intranet) site that may view the incidents; also notified of new ones. Setting "ViewerRole".</summary>
        public string LocalNotificationRole
        {
            get { return IncidentAccess.ViewerRole(Settings); }
        }

        /// <summary>Role on the jud12 (external) site whose members are also notified. Setting "Jud12Role".</summary>
        public string Jud12NotificationRole
        {
            get { return IncidentAccess.GetSetting(Settings, "Jud12Role", "Incident Reporter"); }
        }

        public bool CanViewIncidents
        {
            get { return IncidentAccess.CanView(UserInfo, PortalId, Settings); }
        }

        public bool CanSubmitIncidents
        {
            get { return IncidentAccess.CanSubmit(UserInfo, PortalId, Settings); }
        }

        /// <summary>
        /// A page given in a module setting, as a tab id or a URL. Empty when the setting is blank.
        /// </summary>
        protected string ResolveSettingUrl(string key)
        {
            string value = IncidentAccess.GetSetting(Settings, key, "");
            int tabId;
            if (value.Length > 0 && int.TryParse(value, out tabId))
            {
                return DependencyProvider.GetRequiredService<INavigationManager>().NavigateURL(tabId);
            }
            return value;
        }

        /// <summary>
        /// Link to one incident on the incident list page (setting ViewTabID), or null when that page is not set.
        /// The list module forwards a bare id to its detail view.
        /// </summary>
        protected string ListIncidentUrl(int incidentId)
        {
            string value = IncidentAccess.GetSetting(Settings, "ViewTabID", "");
            if (value.Length == 0) return null;
            int tabId;
            if (int.TryParse(value, out tabId))
            {
                return DependencyProvider.GetRequiredService<INavigationManager>().NavigateURL(tabId, "", "id=" + incidentId);
            }
            return value.TrimEnd('/') + "/id/" + incidentId;
        }

        /// <summary>Hides everything the control would show and explains why.</summary>
        protected void DenyAccess(string action)
        {
            foreach (Control c in Controls) c.Visible = false;
            Skin.AddModuleMessage(this, "You do not have permission to " + action + ".", ModuleMessage.ModuleMessageType.YellowWarning);
        }

        public int IncidentID
        {
            get
            {
                var qs = Request.QueryString["id"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }

        }
    }
}
