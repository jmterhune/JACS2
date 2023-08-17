/*
' Copyright (c) 2023  Joe Terhune
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

namespace tjc.Modules.EmployeeDB
{
    public class EmployeeDBModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EmployeeDBModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            JavaScript.RequestRegistration(CommonJs.DnnPlugins);
        }

        public int EmployeeId
        {
            get
            {
                var qs = Request.QueryString["eid"];
                if (qs != null)
                    return Convert.ToInt32(qs);
                return -1;
            }
        }
        public string EmployeeUrl { get { return _navigationManager.NavigateURL(); } }
        public string ContactUrl { get { return EditUrl("Contact"); } }
        public string DepartmentUrl { get { return EditUrl("Department"); } }
        public string JobGroupUrl { get { return EditUrl("JobGroup"); } }
        public string JobClassUrl { get { return EditUrl("JobClass"); } }
        public string RaceUrl { get { return EditUrl("Race"); } }
        public string EEOUrl { get { return EditUrl("eeo"); } }
        public string CountyUrl { get { return EditUrl("County"); } }
        public string LocationUrl { get { return EditUrl("Location"); } }
        public string SwnLogUrl { get { return EditUrl("Log"); } }
        public string PhoneUrl { get { return EditUrl("eid",EmployeeId.ToString(),"Phone"); } }
        public string EmploymentUrl { get { return EditUrl("eid", EmployeeId.ToString(),"Employment"); } }
        public string EmergencyContactUrl { get { return EditUrl("eid", EmployeeId.ToString(),"EmergencyContact"); } }
        public string DetailUrl { get { return EditUrl("eid", EmployeeId.ToString(),"Employee"); } }
        public string ContactDetailUrl { get { return EditUrl("eid", EmployeeId.ToString(), "EditContact"); } }
        public string SupervisorRole
        {
            get
            {
                if (Settings.Contains("SupervisorRole"))
                    return Settings["SupervisorRole"].ToString();

                return "Supervisor";
            }
        }
        public string SwnUsername
        {
            get
            {
                if (Settings.Contains("SwnUsername"))
                    return Settings["SwnUsername"].ToString();

                return "TJCCAPI";
            }
        }
        public string SwnPassword
        {
            get
            {
                if (Settings.Contains("SwnPassword"))
                    return Settings["SwnPassword"].ToString();

                return "12CircuitAPI!";
            }
        }
        public string SwnServiceIdentifier
        {
            get
            {
                if (Settings.Contains("SwnServiceIdentifier"))
                    return Settings["SwnServiceIdentifier"].ToString();

                return "SWN";
            }
        }
        public string SwnSubscriptionKey
        {
            get
            {
                if (Settings.Contains("SwnSubscriptionKey"))
                    return Settings["SwnSubscriptionKey"].ToString();

                return "57951d5a16604e97a764a9d84df7628c";
            }
        }
        public string SwnBaseUrl
        {
            get
            {
                if (Settings.Contains("SwnBaseUrl"))
                    return Settings["SwnBaseUrl"].ToString();

                return "https://api.onsolve.com/v1/";
            }
        }
    }
}