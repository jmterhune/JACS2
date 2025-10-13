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

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    public partial class CourtCalendarView : JACSModuleBase
    {
        public string GetJsonCalendarItem(CalendarItem calendarItem)
        {
            // Serialize to JSON string
            string json = System.Text.Json.JsonSerializer.Serialize(calendarItem);

            // Optional: Customize serialization (e.g., ignore nulls, handle cycles if any exist in nested types, or format dates)
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = true,  // Skip null properties
                ReferenceHandler = ReferenceHandler.Preserve  // Handle potential reference loops in nested objects like Timeslot.events
            };
            return System.Text.Json.JsonSerializer.Serialize(calendarItem, options);

        }
        public string JsonCalendarItem { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Editable = false;
                JsonCalendarItem = "null";
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

                // Moved the following block outside of the !IsPostBack check to ensure fields are always populated,
                // as ViewState may not reliably preserve Literal control values in certain DNN scenarios or if modified by JS.
                var ctl = new CourtController();
                long courtIdParam = CourtId;
                if (courtIdParam <= 0)
                {
                    CalendarItem calendarItem = new CalendarItem();
                    var ctlTs = new TimeslotController();
                    Timeslot ts = new Timeslot();
                    Components.Event evt = new Components.Event();
                    if (TimeSlotId >= 0)
                    {
                        courtIdParam = ctlTs.GetCourtIdByTimeslotId(TimeSlotId);
                        calendarItem.timeslotId = TimeSlotId;
                    }
                    else if (EventId >= 0)
                    {
                        var ctlEv = new EventController();
                        courtIdParam = ctlEv.GetCourtIdByEventId(EventId);
                        evt = ctlEv.GetEvent(EventId);
                        calendarItem.eventId = EventId;
                        if (evt != null)
                        {
                            ts = ctlTs.GetTimeslotByEventId(TimeSlotId);
                            calendarItem.timeslotId = ts.id;

                        }
                    }
                    var dates = DateTimeExtensions.GetWeekStartEnd(ts.start);
                    calendarItem.start = dates.Start;
                    calendarItem.end = dates.End;
                    var jsonCalendarItem = GetJsonCalendarItem(calendarItem);
                    if (jsonCalendarItem != null)
                        JsonCalendarItem = GetJsonCalendarItem(calendarItem);
                }
                if (courtIdParam <= 0)
                {
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "No court selected. Please select a court from the Court List.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                    return;
                }
                Court court = ctl.GetCourt(courtIdParam);
                var court_types = new CourtTypeController().GetCourtTypeDropDownItems();
                string fields = string.Empty;
                if (court != null)
                {
                    if (IsAdmin)
                    {
                        Editable = true;
                    }
                    else if (IsJudge)
                    {
                        var courtJudge = court.GetJudge();
                        if (courtJudge != null && courtJudge.id == UserId)
                        {
                            Editable = true;
                        }
                    }
                    else
                    {
                        var permissionsCtl = new CourtPermissionController();
                        var courtPermissions = permissionsCtl.GetCourtPermissionByCourt(court.id, UserId);
                        if (courtPermissions != null)
                        {
                                Editable = courtPermissions.editable;
                        }
                    }

                    ltCourtName.Text = court.description;
                    ltJudgeName.Text = court.GetJudge().name;
                    var split_format = court.case_num_format.Split('-');
                    if (split_format.Length == 1)
                    {
                        fields = $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple1\" required=\"\" value=\"\" placeholder=\"{split_format[0]}\" />";
                    }
                    else if (split_format.Length == 3)
                    {
                        var options = $"<option value=\"\" {(split_format[1] == "0" ? "selected=\"selected\"" : "")}>-</option>" + string.Join("", court_types.Select(ct => $"<option value=\"{ct.Value}\" {(ct.Value == split_format[1] ? "selected=\"selected\"" : "")}>{ct.Value}</option>"));
                        if (split_format[1].Length == 2 || split_format[1] == "0")
                        {
                            fields = $"<input type=\"text\" class=\"form-control case-num-part mr-1\" maxlength=\"4\" id=\"case_num_format_multiple1\" required=\"\" value=\"\" placeholder=\"Year\" />" +
                                "<span> - </span>" +
                                $"<select class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple2\" required=\"\">" +
                                options +
                                "</select>" +
                                "<span> - </span>" +
                                $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple3\" maxlength=\"7\" required=\"\" value=\"\" placeholder=\"Case Number\" />";
                        }
                        else
                        {
                            fields = $"<input type=\"text\" class=\"form-control case-num-part mr-1\" maxlength=\"4\" id=\"case_num_format_multiple1\" required=\"\" value=\"\" placeholder=\"{split_format[0]}\" />" +
                                "<span> - </span>" +
                                $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple2\" maxlength=\"7\" required=\"\" value=\"\" placeholder=\"{split_format[1]}\" />" +
                                "<span> - </span>" +
                                $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple3\" maxlength=\"4\" required=\"\" value=\"\" placeholder=\"{split_format[2]}\" />";
                        }
                    }
                    else if (split_format.Length >= 4 && split_format.Length <= 6)
                    {
                        var options = $"<option value=\"\" {(split_format[2] == "0" ? "selected=\"selected\"" : "")}>-</option>" +
                            string.Join("", court_types.Select(ct => $"<option value=\"{ct.Value}\" {(ct.Value == split_format[2] ? "selected=\"selected\"" : "")}>{ct.Value}</option>"));
                        string disabled = split_format.Length == 6 ? "disabled=\"\"" : "";
                        fields = $"<input type=\"text\" class=\"form-control case-num-part\" id=\"case_num_format_multiple1\" style=\"max-width:3rem\" maxlength=\"2\" value=\"{split_format[0]}\" {disabled} />" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple2\" style=\"max-width:4rem\" maxlength=\"4\" required=\"\" value=\"{split_format[1]}\" placeholder=\"Year\" />" +
                            "<span> - </span>" +
                            $"<select class=\"form-control case-num-part mr-1 court_type_change_label\" style='max-width:4rem' id=\"case_num_format_multiple3\" required=\"\">" +
                            options +
                            "</select>" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple4\" maxlength=\"6\" required=\"\" value=\"{split_format[3]}\" placeholder=\"Case Number\" />" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple5\" maxlength=\"4\" style=\"max-width:6rem\" required=\"\" value=\"{split_format[4]}\" placeholder=\"xxxx\" />" +
                            (split_format.Length == 6 ? "<span> - </span>" + $"<input type=\"text\" class=\"form-control case-num-part mr-1\" style=\"max-width:4rem\" id=\"case_num_format_multiple6\" maxlength=\"2\" placeholder='xx' value=\"{split_format[5]}\" />" : "");
                    }
                    else
                    {
                        fields = "<input type=\"text\" class=\"form-control case-num-part mr-1\" required=\"\">";
                    }
                    ltCaseNumber.Text = fields;
                    var courtCtl = new CourtController();
                    var courtTimeslotController = new CourtTimeslotController();
                    var templateController = new CourtTemplateController();
                    var templateOrderController = new CourtTemplateOrderController();
                    var lastTimeslotDate = courtCtl.GetLastTimeslotDate(courtIdParam);
                    var lastTemplateTimeslot = courtTimeslotController.GetLastTemplateTimeslot(courtIdParam);

                    var lastHearingDate = courtCtl.GetLastHearingDate(courtIdParam);
                    if (lastTimeslotDate != null)
                        ltLastTimeslot.Text = $"<p>The last timeslot date in the calendar is <span class='text-primary'>{lastTimeslotDate:MM/dd/yyyy}</span></p>";
                    if (lastTemplateTimeslot != null)
                    {
                        var template = templateController.GetCourtTemplate(lastTemplateTimeslot.template_id.Value);
                        ltLastTemplateTimeslot.Text = $"<p>The last template used: <span class='text-primary'>{template?.name ?? "Unknown"}</span> on <span class='text-primary'>{lastTemplateTimeslot.start:MM/dd/yyyy}</span></p>";
                    }
                    if (lastHearingDate != null)
                        ltLastHearing.Text = $"<p>The last scheduled hearing in the calendar is on <span class='text-primary'>{lastHearingDate:MM/dd/yyyy}</span></p>";

                    // Populate template dropdown
                    var templates = templateOrderController.GetCourtTemplateOrdersByCourtId(courtIdParam, court.auto_extension)
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
                    txtStartDate.Text = lastTemplateTimeslot?.start.ToString("MM/dd/yyyy") ?? DateTime.Now.ToString("MM/dd/yyyy");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}