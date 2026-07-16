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

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using tjc.Modules.ThreatReport.Components;

namespace tjc.Modules.ThreatReport
{
    /// <summary>
    /// View-only list of incidents for the intranet site. Permissions are controlled via
    /// DNN; this module never accepts input or sends mail.
    /// </summary>
    public partial class View : ThreatReportModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    // If the request carries an "id" query parameter (e.g. a link from the
                    // internet site's email of the form [IntranetAppUrl]/id/N), forward to
                    // the detail control instead of rendering the list. This is what makes
                    // bare URLs like /Judiciary/Threat-Report/id/164 land on the incident view.
                    string idParam = Request.QueryString["id"];
                    int incidentId;
                    if (int.TryParse(idParam, out incidentId) && incidentId > 0)
                    {
                        Response.Redirect(EditUrl("id", incidentId.ToString(), "incident"));
                        return;
                    }

                    IncidentController ctl = new IncidentController();
                    rptIncidentList.DataSource = ctl.GetIncidents().Where(x => x.Location != null);
                    rptIncidentList.DataBind();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
