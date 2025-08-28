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

using DocumentFormat.OpenXml.Vml.Spreadsheet;
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
    public partial class CourtCalendarView : JACSModuleBase
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

                // Moved the following block outside of the !IsPostBack check to ensure fields are always populated,
                // as ViewState may not reliably preserve Literal control values in certain DNN scenarios or if modified by JS.
                var ctl = new CourtController();
                Court court = ctl.GetCourt(CourtId);
                var court_types = new CourtTypeController().GetCourtTypeDropDownItems();
                string fields = string.Empty;
                if (court != null)
                {
                    ltCourtName.Text = court.description;
                    ltJudgeName.Text = court.GetJudge().name;
                    var split_format = court.case_num_format.Split('-');
                    if (split_format.Length == 1)
                    {
                        fields = $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple1\" required=\"\" value=\"\" placeholder=\"{split_format[0]}\" />";
                    }
                    else if (split_format.Length == 3)
                    {
                        var options = string.Join("", court_types.Select(ct => $"<option value=\"{ct.Value}\" {(ct.Value == split_format[1] ? "selected=\"selected\"" : "")}>{ct.Value}</option>"));
                        if (split_format[1].Length == 2 || split_format[1] == "0")
                        {
                            fields = $"<input type=\"text\" class=\"form-control case-num-part mr-1\" maxlength=\"4\" id=\"case_num_format_multiple1\" required=\"\" value=\"\" placeholder=\"Complete Year\" />" +
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
                        var options = $"<option value=\"\" {(split_format[2] == "0" ? "selected=\"selected\"" : "")}></option>" +
                            string.Join("", court_types.Select(ct => $"<option value=\"{ct.Value}\" {(ct.Value == split_format[2] ? "selected=\"selected\"" : "")}>{ct.Value}</option>"));
                        string disabled = split_format.Length == 6 ? "disabled=\"\"" : "";
                        fields = $"<input type=\"text\" class=\"form-control case-num-part\" id=\"case_num_format_multiple1\" style=\"max-width:3rem\" maxlength=\"2\" value=\"{split_format[0]}\" {disabled} />" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple2\" style=\"max-width:4rem\" maxlength=\"4\" required=\"\" value=\"\" placeholder=\"Complete Year\" />" +
                            "<span> - </span>" +
                            $"<select class=\"form-control case-num-part mr-1 court_type_change_label\" style='max-width:4rem' id=\"case_num_format_multiple3\" required=\"\">" +
                            options +
                            "</select>" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple4\" maxlength=\"6\" required=\"\" value=\"\" placeholder=\"Case Number\" />" +
                            "<span> - </span>" +
                            $"<input type=\"text\" class=\"form-control case-num-part mr-1\" id=\"case_num_format_multiple5\" maxlength=\"4\" style=\"max-width:6rem\" required=\"\" value=\"\" placeholder=\"{(split_format.Length > 4 ? split_format[4] : "")}\" />" +
                            (split_format.Length == 6 ? "<span> - </span>" + $"<input type=\"text\" class=\"form-control case-num-part mr-1\" style=\"max-width:4rem\" id=\"case_num_format_multiple6\" maxlength=\"2\" value=\"{split_format[5]}\" {disabled} />" : "");
                    }
                    else
                    {
                        fields = "<input type=\"text\" class=\"form-control case-num-part mr-1\" required=\"\">";
                    }
                    ltCaseNumber.Text = fields;
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}