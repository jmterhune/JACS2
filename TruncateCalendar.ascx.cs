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
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using tjc.Modules.jacs.Components;

namespace tjc.Modules.jacs
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from JACSModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class TruncateCalendar : JACSModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public TruncateCalendar()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.ModuleContext = this;
                navbar.ActiveLink = "lnkCourt";
                if (UserId <= 0 || !UserInfo.IsAdmin)
                {
                    Response.Redirect(_navigationManager.NavigateURL(), true);
                }
                if (!IsPostBack)
                {
                    hdCourtId.Value = CourtId.ToString();

                    var courtCtl = new CourtController();
                    var court = courtCtl.GetCourt(CourtId);
                    if (court != null)
                    {
                        ltCourtName.Text = court.description;
                    }
                    else
                    {
                        ltCourtName.Text = "Unknown Court";
                    }


                    var lastTimeslotDate = courtCtl.GetLastTimeslotDate(CourtId);
                    if (lastTimeslotDate != null) 
                    ltLastTimeslot.Text = $"<p>The last timeslot date in the calendar is {lastTimeslotDate?.ToString("MM/dd/yyyy") ?? "N/A"}</p>";
                    var lastHearingDate = courtCtl.GetLastHearingDate(CourtId);
                    if (lastHearingDate != null)
                        ltLastTimeslot.Text += $"<p>The last scheduled hearing in the calendar is on: {lastHearingDate?.ToString("MM/dd/yyyy") ?? "N/A"}</p>";
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}