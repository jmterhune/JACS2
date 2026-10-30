/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using tjc.Modules.CourtCalendar.Components;
using Calendar = tjc.Modules.CourtCalendar.Components.Calendar;

namespace tjc.Modules.CourtCalendar
{
    public partial class EditCalendar : CourtCalendarModuleBase
    {
        private CalendarController Calendars { get { return new CalendarController(_hostSettings); } }
        private CategoryController Categories { get { return new CategoryController(_hostSettings); } }
        private DivisionController Divisions { get { return new DivisionController(_hostSettings); } }

        private Calendar _calendar;
        private Division _division;
        private Dictionary<int, string> _categoryNames;

        private int CurrentCalendarId
        {
            get
            {
                if (CalendarIdParam > 0) return CalendarIdParam;
                int id; int.TryParse(hfCalendarId.Value, out id); return id;
            }
        }

        private Calendar CurrentCalendar
        {
            get
            {
                if (_calendar == null && CurrentCalendarId > 0)
                    _calendar = Calendars.GetCalendar(CurrentCalendarId);
                return _calendar;
            }
        }

        private int CurrentDivisionId
        {
            get
            {
                var cal = CurrentCalendar;
                if (cal != null) return cal.DivisionId;
                return DivisionIdParam;
            }
        }

        private Division CurrentDivision
        {
            get
            {
                if (_division == null && CurrentDivisionId > 0)
                    _division = Divisions.GetDivision(CurrentDivisionId);
                return _division;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lnkHome.NavigateUrl = HomeUrl;

            var division = CurrentDivision;
            if (division == null || !CanManageDivision(division))
            {
                pnlMain.Visible = false;
                pnlDenied.Visible = true;
                return;
            }

            // Week row headers only apply to the Weekday-headers layout.
            pnlWeekHeaders.Visible = division.CalendarTypeId == (int)CalendarType.WeekdayHeader;

            LoadCategoryNames();

            if (!IsPostBack)
            {
                var cal = CurrentCalendar;
                if (cal == null)
                {
                    // New calendar for a division.
                    hfCalendarId.Value = "0";
                    pnlNewPeriod.Visible = true;
                    pnlChildren.Visible = false;
                    btnSendNotification.Visible = false;
                    btnDeleteCalendar.Visible = false;
                    ltTitle.Text = HttpUtility.HtmlEncode("New calendar – " + division.Name);
                    BindPeriodDropdowns();
                    BindCategoryDropdown();
                }
                else
                {
                    hfCalendarId.Value = cal.CalendarId.ToString();
                    pnlNewPeriod.Visible = false;
                    pnlChildren.Visible = true;
                    ltTitle.Text = HttpUtility.HtmlEncode(division.Name + " – " + cal.MonthName + " " + cal.Year);
                    txtTitleOverride.Text = cal.TitleOverride;
                    txtNotes.Text = cal.Notes;
                    chkPublished.Checked = cal.Published;
                    BindCategoryDropdown();
                    BindWeeks();
                    BindEntries();
                }
            }
        }

        protected string CategoryName(int categoryId)
        {
            if (categoryId <= 0) return "Default";
            string name;
            return (_categoryNames != null && _categoryNames.TryGetValue(categoryId, out name)) ? name : "Default";
        }

        private void LoadCategoryNames()
        {
            _categoryNames = Categories.GetCategories(CurrentDivisionId).ToDictionary(c => c.CategoryId, c => c.Name);
        }

        private void BindPeriodDropdowns()
        {
            ddlMonth.Items.Clear();
            for (int m = 1; m <= 12; m++)
                ddlMonth.Items.Add(new ListItem(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m), m.ToString()));
            ddlYear.Items.Clear();
            int thisYear = DateTime.Now.Year;
            for (int y = thisYear - 1; y <= thisYear + 2; y++)
                ddlYear.Items.Add(new ListItem(y.ToString(), y.ToString()));
            ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            ddlYear.SelectedValue = thisYear.ToString();
        }

        private void BindCategoryDropdown()
        {
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("Default", "0"));
            foreach (var c in Categories.GetCategories(CurrentDivisionId))
                if (!c.IsDefault)
                    ddlCategory.Items.Add(new ListItem(c.Name, c.CategoryId.ToString()));
        }

        private void BindWeeks()
        {
            rptWeeks.DataSource = Calendars.GetWeekHeaders(CurrentCalendarId);
            rptWeeks.DataBind();
        }

        private void BindEntries()
        {
            rptEntries.DataSource = Calendars.GetEntries(CurrentCalendarId);
            rptEntries.DataBind();
        }

        // ---- Calendar settings ----

        protected void btnSaveCalendar_Click(object sender, EventArgs e)
        {
            var division = CurrentDivision;
            if (division == null || !CanManageDivision(division)) return;

            var cal = CurrentCalendar;
            if (cal == null)
            {
                int year, month;
                int.TryParse(ddlYear.SelectedValue, out year);
                int.TryParse(ddlMonth.SelectedValue, out month);
                if (Calendars.GetCalendar(CurrentDivisionId, year, month) != null)
                {
                    ltMessage.Text = Alert("A calendar for that month already exists for this division.", "danger");
                    return;
                }
                cal = new Calendar
                {
                    DivisionId = CurrentDivisionId,
                    Year = year,
                    Month = month,
                    TitleOverride = txtTitleOverride.Text.Trim(),
                    Notes = txtNotes.Text.Trim(),
                    Published = chkPublished.Checked,
                    CreatedByUserId = UserId,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedByUserId = UserId,
                    LastModifiedOnDate = DateTime.Now,
                };
                int newId = Calendars.CreateCalendar(cal);
                Response.Redirect(EditCalendarUrl(newId), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            cal.TitleOverride = txtTitleOverride.Text.Trim();
            cal.Notes = txtNotes.Text.Trim();
            cal.Published = chkPublished.Checked;
            cal.LastModifiedByUserId = UserId;
            Calendars.UpdateCalendar(cal);
            ltMessage.Text = Alert("Calendar saved.", "success");
        }

        protected void btnDeleteCalendar_Click(object sender, EventArgs e)
        {
            var division = CurrentDivision;
            if (division == null || !CanManageDivision(division)) return;
            if (CurrentCalendarId > 0) Calendars.DeleteCalendar(CurrentCalendarId);
            Response.Redirect(HomeUrl, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnSendNotification_Click(object sender, EventArgs e)
        {
            var division = CurrentDivision;
            var cal = CurrentCalendar;
            if (division == null || cal == null || !CanManageDivision(division)) return;

            var from = string.IsNullOrWhiteSpace(division.DivisionEmail) ? PortalSettings.Email : division.DivisionEmail;
            var senderName = UserInfo != null ? UserInfo.DisplayName : string.Empty;
            string publicUrl = string.Empty; // Public URL is hand-entered on the requirements page; left blank here.

            int sent = new NotificationController(_hostSettings)
                .SendUpdateNotification(cal, division, from, publicUrl, senderName);
            ltMessage.Text = sent > 0
                ? Alert(string.Format("Update notification sent to {0} subscriber(s).", sent), "success")
                : Alert("No subscribers to notify yet.", "info");
        }

        // ---- Week headers ----

        protected void btnAddWeek_Click(object sender, EventArgs e) { ShowWeekForm(0); }

        protected void rptWeeks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id; if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            if (e.CommandName == "edit") ShowWeekForm(id);
            else if (e.CommandName == "delete")
            {
                Calendars.DeleteWeekHeader(id);
                BindWeeks();
                ltMessage.Text = Alert("Week header deleted.", "success");
            }
        }

        private void ShowWeekForm(int weekId)
        {
            pnlWeekForm.Visible = true;
            hfWeekId.Value = weekId.ToString();
            if (weekId > 0)
            {
                var w = Calendars.GetWeekHeader(weekId);
                if (w == null) { pnlWeekForm.Visible = false; return; }
                txtWeekStart.Text = w.WeekStartDate.ToString("yyyy-MM-dd");
                txtWeekLabel.Text = w.Label;
                txtWeekSub.Text = w.SubLabel;
                txtWeekSort.Text = w.SortOrder.ToString();
            }
            else
            {
                txtWeekStart.Text = txtWeekLabel.Text = txtWeekSub.Text = string.Empty;
                txtWeekSort.Text = "0";
            }
            BindWeeks();
        }

        protected void btnSaveWeek_Click(object sender, EventArgs e)
        {
            if (!CanManageDivision(CurrentDivision)) return;
            DateTime start;
            if (!DateTime.TryParse(txtWeekStart.Text, out start))
            {
                ltMessage.Text = Alert("Enter a valid week start date.", "danger");
                ShowWeekForm(ParseId(hfWeekId.Value));
                return;
            }
            int weekId = ParseId(hfWeekId.Value);
            var w = weekId > 0 ? Calendars.GetWeekHeader(weekId) : new WeekHeader { CalendarId = CurrentCalendarId };
            if (w == null) w = new WeekHeader { CalendarId = CurrentCalendarId };
            w.CalendarId = CurrentCalendarId;
            w.WeekStartDate = start.Date;
            w.Label = txtWeekLabel.Text.Trim();
            w.SubLabel = txtWeekSub.Text.Trim();
            w.SortOrder = ParseId(txtWeekSort.Text);
            Calendars.SaveWeekHeader(w);
            pnlWeekForm.Visible = false;
            BindWeeks();
            ltMessage.Text = Alert("Week header saved.", "success");
        }

        protected void btnCancelWeek_Click(object sender, EventArgs e)
        {
            pnlWeekForm.Visible = false;
            BindWeeks();
        }

        // ---- Entries ----

        protected void btnAddEntry_Click(object sender, EventArgs e) { ShowEntryForm(0); }

        protected void rptEntries_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id; if (!int.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
            switch (e.CommandName)
            {
                case "edit": ShowEntryForm(id); break;
                case "delete":
                    Calendars.DeleteEntry(id);
                    BindEntries();
                    ltMessage.Text = Alert("Entry deleted.", "success");
                    break;
                case "strike":
                    var entry = Calendars.GetEntry(id);
                    if (entry != null)
                    {
                        Calendars.SetStricken(id, !entry.IsStricken, UserId);
                        BindEntries();
                        ltMessage.Text = Alert(entry.IsStricken ? "Entry restored." : "Entry struck.", "success");
                    }
                    break;
                case "copynext":
                    CopyEntryToNextDay(id);
                    break;
            }
        }

        private void CopyEntryToNextDay(int entryId)
        {
            if (!CanManageDivision(CurrentDivision)) return;
            var src = Calendars.GetEntry(entryId);
            if (src == null) return;
            var next = src.EntryDate.AddDays(1);
            var cal = CurrentCalendar;
            if (cal != null && (next.Year != cal.Year || next.Month != cal.Month))
            {
                ltMessage.Text = Alert("The next day falls in another month. Add it on that month's calendar instead.", "warning");
                return;
            }
            Calendars.CopyEntryToNextDay(entryId, UserId);
            BindEntries();
            ltMessage.Text = Alert(string.Format("Copied to {0:MM/dd}.", next), "success");
        }

        private void ShowEntryForm(int entryId)
        {
            pnlEntryForm.Visible = true;
            BindCategoryDropdown();
            hfEntryId.Value = entryId.ToString();
            txtRepeatThrough.Text = string.Empty;
            chkRepeatWeekdaysOnly.Checked = true;
            // Repeat-across-a-range only applies when adding a new entry.
            pnlRepeat.Visible = entryId <= 0;
            if (entryId > 0)
            {
                var en = Calendars.GetEntry(entryId);
                if (en == null) { pnlEntryForm.Visible = false; return; }
                txtEntryDate.Text = en.EntryDate.ToString("yyyy-MM-dd");
                txtTimespan.Text = en.Timespan;
                chkAllDay.Checked = en.AllDay;
                txtShortTitle.Text = en.ShortTitle;
                txtDetail.Text = en.Detail;
                var sel = ddlCategory.Items.FindByValue(en.CategoryId.ToString());
                ddlCategory.SelectedValue = sel != null ? en.CategoryId.ToString() : "0";
                txtEntrySort.Text = en.SortOrder.ToString();
            }
            else
            {
                txtEntryDate.Text = txtTimespan.Text = txtShortTitle.Text = txtDetail.Text = string.Empty;
                chkAllDay.Checked = false;
                ddlCategory.SelectedValue = "0";
                txtEntrySort.Text = "0";
            }
            BindEntries();
        }

        protected void btnSaveEntry_Click(object sender, EventArgs e)
        {
            if (!CanManageDivision(CurrentDivision)) return;
            DateTime date;
            if (!DateTime.TryParse(txtEntryDate.Text, out date))
            {
                ltMessage.Text = Alert("Enter a valid entry date.", "danger");
                ShowEntryForm(ParseId(hfEntryId.Value));
                return;
            }
            if (string.IsNullOrWhiteSpace(txtShortTitle.Text))
            {
                ltMessage.Text = Alert("Short title is required.", "danger");
                ShowEntryForm(ParseId(hfEntryId.Value));
                return;
            }
            int entryId = ParseId(hfEntryId.Value);
            var en = entryId > 0 ? Calendars.GetEntry(entryId) : new CalendarEntry { CalendarId = CurrentCalendarId, CreatedByUserId = UserId, CreatedOnDate = DateTime.Now };
            if (en == null) en = new CalendarEntry { CalendarId = CurrentCalendarId, CreatedByUserId = UserId, CreatedOnDate = DateTime.Now };
            en.CalendarId = CurrentCalendarId;
            en.EntryDate = date.Date;
            en.AllDay = chkAllDay.Checked;
            en.Timespan = chkAllDay.Checked ? string.Empty : txtTimespan.Text.Trim();
            en.ShortTitle = txtShortTitle.Text.Trim();
            en.Detail = txtDetail.Text.Trim();
            en.CategoryId = ParseId(ddlCategory.SelectedValue);
            en.SortOrder = ParseId(txtEntrySort.Text);
            Calendars.SaveEntry(en);

            int repeated = 0;
            string rangeWarning = null;
            if (entryId <= 0)
                repeated = ReplicateAcrossRange(en, date, out rangeWarning);

            pnlEntryForm.Visible = false;
            BindEntries();
            if (!string.IsNullOrEmpty(rangeWarning))
                ltMessage.Text = Alert(rangeWarning, "warning");
            else if (repeated > 0)
                ltMessage.Text = Alert(string.Format("Entry saved and added to {0} more day(s).", repeated), "success");
            else
                ltMessage.Text = Alert("Entry saved.", "success");
        }

        /// <summary>
        /// When a "repeat through" end date is set, copies the just-saved entry onto
        /// each following day up to that date (optionally weekdays only), staying
        /// within the calendar's own month. Returns how many copies were added and,
        /// via <paramref name="warning"/>, any reason the range was rejected.
        /// </summary>
        private int ReplicateAcrossRange(CalendarEntry template, DateTime startDate, out string warning)
        {
            warning = null;
            DateTime through;
            if (!DateTime.TryParse(txtRepeatThrough.Text, out through)) return 0;
            through = through.Date;
            if (through <= startDate.Date) return 0;

            var cal = CurrentCalendar;
            if (cal != null && (through.Year != cal.Year || through.Month != cal.Month))
            {
                warning = "Entry saved. The repeat-through date is outside this month, so no extra days were added.";
                return 0;
            }

            bool weekdaysOnly = chkRepeatWeekdaysOnly.Checked;
            var dates = new List<DateTime>();
            for (var d = startDate.Date.AddDays(1); d <= through; d = d.AddDays(1))
            {
                if (weekdaysOnly && (d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday))
                    continue;
                dates.Add(d);
            }
            if (dates.Count == 0) return 0;
            return Calendars.ReplicateEntry(template, dates, UserId);
        }

        protected void btnCancelEntry_Click(object sender, EventArgs e)
        {
            pnlEntryForm.Visible = false;
            BindEntries();
        }

        private static int ParseId(string value)
        {
            int v; int.TryParse(value, out v); return v;
        }

        private static string Alert(string message, string type)
        {
            return string.Format("<div class=\"alert alert-{0}\">{1}</div>", type, HttpUtility.HtmlEncode(message));
        }
    }
}
