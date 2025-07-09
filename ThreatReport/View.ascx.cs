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

using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from ThreatReportModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : ThreatReportModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    IncidentController ctl = new IncidentController();
                    rptIncidentList.DataSource = ctl.GetIncidents().Where(x => x.Location != null);
                    rptIncidentList.DataBind();
                    if (Settings.Contains("ViewerRole"))
                    {
                        var rctl = new RoleController();
                        rptViewers.DataSource = rctl.GetUsersByRole(PortalId, Settings["ViewerRole"].ToString());
                        rptViewers.DataBind();
                    }

                    IEnumerable<Incident> incidents = ctl.GetIncidents().Where(x => x.Location == null);
                    //if(incidents.Count() > 0)
                    //{
                    //    foreach (Incident incident in incidents)
                    //    {
                    //        ctl.DeleteIncident(incident);
                    //    }
                    //}
                    if (Settings.Contains("EditTabID"))
                    {
                        string editTab = Settings["EditTabID"].ToString();
                        lnkEdit.Visible = true;
                        lnkEdit.NavigateUrl = editTab;
                    }


                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}