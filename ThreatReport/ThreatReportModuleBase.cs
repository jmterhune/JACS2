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

using DotNetNuke.Entities.Modules;
using System;

namespace tjc.Modules.ThreatReport
{
    public class ThreatReportModuleBase : PortalModuleBase
    {
        /// <summary>Role on this (intranet) site whose members are notified. Setting "ViewerRole".</summary>
        public string LocalNotificationRole
        {
            get { return GetRoleSetting("ViewerRole", "Incident Viewer"); }
        }

        /// <summary>Role on the jud12 (external) site whose members are notified. Setting "Jud12Role".</summary>
        public string Jud12NotificationRole
        {
            get { return GetRoleSetting("Jud12Role", "Incident Reporter"); }
        }

        private string GetRoleSetting(string key, string fallback)
        {
            if (Settings != null && Settings.Contains(key) && Settings[key] != null)
            {
                string value = Settings[key].ToString().Trim();
                if (value.Length > 0) return value;
            }
            return fallback;
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