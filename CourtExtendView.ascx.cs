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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using System;
using System.Linq;
using System.Web.UI.WebControls;
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
    public partial class CourtExtendView : JACSModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                navbar.MainViewUrl = MainViewUrl;
                navbar.AttorneyListUrl = AttorneyListUrl;
                navbar.CategoryListUrl = CategoryListUrl;
                navbar.CountyListUrl = CountyListUrl;
                navbar.CourtListUrl = CourtListUrl;
                navbar.CourtTypeListUrl = CourtTypeListUrl;
                navbar.CourtPermissionListUrl = CourtPermissionListUrl;
                navbar.DocketPrintUrl = DocketPrintUrl;
                navbar.EventListUrl = EventListUrl;
                navbar.EventStatusListUrl = EventStatusListUrl;
                navbar.EventTypeListUrl = EventTypeListUrl;
                navbar.HolidayListUrl = HolidayListUrl;
                navbar.JudgeListUrl = JudgeListUrl;
                navbar.MotionListUrl = MotionListUrl;
                navbar.TemplateListUrl = TemplateListUrl;
                navbar.TimeSlotListUrl = TimeSlotListUrl;
                navbar.QuickReferenceUrl = QuickReferenceUrl;
                navbar.UserListUrl = UserListUrl;
                navbar.RoleListUrl = RoleListUrl;
                navbar.PermissionListUrl = PermissionListUrl;
                navbar.ActiveLink = "lnkCourt";
                if (!IsPostBack)
                {
                    if (CourtId <= 0)
                    {
                        Response.Redirect(string.Format("{0}/cid/{1}", CourtCalendarUrl, CourtId));
                        return;
                    }

                    var courtController = new CourtController();
                    var court = courtController.GetCourt(CourtId);
                    if (court == null)
                    {
                        Response.Redirect(string.Format("{0}/cid/{1}", CourtCalendarUrl, CourtId));
                        return;
                    }

                    // Check permissions
                    var permissionController = new CourtPermissionController();
                    var judge = court.GetJudge();
                    bool hasPermission = permissionController.HasCourtPermission(UserInfo.UserID, judge?.id ?? 0) || IsAdmin;
                    if (!hasPermission)
                    {
                        Response.Redirect(MainViewUrl);
                        return;
                    }

                    // Set court name
                    ltCourtName.Text = court.description;

                    // Load last timeslot, template timeslot, and hearing
                    var timeslotController = new TimeslotController();
                    var courtTimeslotController = new CourtTimeslotController();
                    var templateController = new CourtTemplateController();
                    var templateOrderController = new CourtTemplateOrderController();
                    var timeslots = courtTimeslotController.GetCourtTimeslotsByCourtId(CourtId)
                        .OrderByDescending(ct => ct.Timeslot.start)
                        .ToList();

                    var lastTimeslot = timeslots.FirstOrDefault();
                    var lastTemplateTimeslot = timeslots.FirstOrDefault(ct => ct.Timeslot.template_id.HasValue);
                    var lastHearing = timeslots.FirstOrDefault(ct => ct.Timeslot.TimeslotEvents.Any());

                    if (lastTimeslot != null)
                        ltLastTimeslot.Text = $"<p>The last timeslot date in the calendar is <span class='text-primary'>{lastTimeslot.Timeslot.start:MM/dd/yyyy}</span></p>";
                    if (lastTemplateTimeslot != null)
                    {
                        var template = templateController.GetCourtTemplate(lastTemplateTimeslot.Timeslot.template_id.Value);
                        ltLastTemplateTimeslot.Text = $"<p>The last template used: <span class='text-primary'>{template?.name ?? "Unknown"}</span> on <span class='text-primary'>{lastTemplateTimeslot.Timeslot.start:MM/dd/yyyy}</span></p>";
                    }
                    if (lastHearing != null)
                        ltLastHearing.Text = $"<p>The last scheduled hearing in the calendar is on <span class='text-primary'>{lastHearing.Timeslot.start:MM/dd/yyyy}</span></p>";

                    // Populate template dropdown
                    var templates = templateOrderController.GetCourtTemplateOrdersByCourtId(CourtId)
                        .Where(t => t.auto)
                        .OrderBy(t => t.order)
                        .Select(t => new { t.order, t.template_id, Name = templateController.GetCourtTemplate(t.template_id.Value)?.name })
                        .ToList();

                    ddlStartTemplate.Items.Clear();
                    foreach (var template in templates)
                    {
                        if (template.Name != null)
                            ddlStartTemplate.Items.Add(new ListItem(template.Name, template.order.ToString()));
                    }

                    // Initialize datepicker
                    txtStartDate.Text = lastTemplateTimeslot?.Timeslot.start.ToString("MM/dd/yyyy") ?? DateTime.Now.ToString("MM/dd/yyyy");
                    hdCourtId.Value = CourtId.ToString();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}