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
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web.UI;

namespace tjc.Modules.EmployeeDB
{
    public class EmployeeDBModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public EmployeeDBModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
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
        public string DepartmentUrl { get { return EditUrl("Department"); } }
        public string JobGroupUrl { get { return EditUrl("JobGroup"); } }
        public string JobClassUrl { get { return EditUrl("JobClass"); } }
        public string RaceUrl { get { return EditUrl("Race"); } }
        public string CountyUrl { get { return EditUrl("County"); } }
        public string LocationUrl { get { return EditUrl("Location"); } }
        public string PhoneUrl { get { return EditUrl("eid",EmployeeId.ToString(),"Phone"); } }
        public string EmploymentUrl { get { return EditUrl("eid", EmployeeId.ToString(),"Employment"); } }
        public string ContactUrl { get { return EditUrl("eid", EmployeeId.ToString(),"Contact"); } }
        public string DetailUrl { get { return EditUrl("eid", EmployeeId.ToString()); } }

        public string SupervisorRole
        {
            get
            {
                if (Settings.Contains("SupervisorRole"))
                    return Settings["SupervisorRole"].ToString();

                return "Supervisor";
            }
        }
    }
}