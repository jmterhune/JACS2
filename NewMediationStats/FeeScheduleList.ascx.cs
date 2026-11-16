using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using tjc.Modules.NewMediationStats.Components;

namespace tjc.Modules.NewMediationStats
{
    /// <summary>
    /// Lists the fee schedules and lets an administrator set when each takes effect. The tiers (fee amounts and
    /// income ranges) are shown for reference; they are seeded by the install script.
    /// </summary>
    public partial class FeeScheduleList : NewMediationStatsModuleBase
    {
        #region Members
        private readonly INavigationManager _navigationManager;
        private readonly IHostSettings _hostSettings;
        #endregion

        public FeeScheduleList()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        #region Methods
        private static string GroupName(int groupId)
        {
            if (groupId == FeeGroups.Family) return "Family";
            if (groupId == FeeGroups.County) return "County";
            return "Group " + groupId;
        }

        private static string TypeName(string tierType)
        {
            switch (tierType)
            {
                case FeeTierTypes.FeeAmount: return "Fee range / amount";
                case FeeTierTypes.PartyIncome: return "Party income range";
                case FeeTierTypes.FeePaid: return "Fee paid";
                case FeeTierTypes.FeeOwed: return "Fee owed";
                default: return tierType;
            }
        }

        private void BindList()
        {
            var ctl = new FeeScheduleController(_hostSettings);
            List<FeeSchedule> schedules = ctl.GetSchedules().ToList();
            // The schedule in effect today is the latest one whose date has arrived.
            FeeSchedule current = schedules.Where(s => s.EffectiveDate.HasValue && s.EffectiveDate.Value.Date <= DateTime.Today)
                                           .OrderByDescending(s => s.EffectiveDate).FirstOrDefault();
            rptSchedule.DataSource = schedules.Select(s => new
            {
                s.FeeScheduleId,
                s.Name,
                s.Notes,
                EffectiveText = s.EffectiveDate.HasValue ? s.EffectiveDate.Value.ToString("MM/dd/yyyy") : "Not set",
                Status = !s.EffectiveDate.HasValue ? "Not in effect"
                       : (current != null && s.FeeScheduleId == current.FeeScheduleId) ? "In effect now"
                       : s.EffectiveDate.Value.Date > DateTime.Today ? "Starts " + s.EffectiveDate.Value.ToString("MM/dd/yyyy")
                       : "Replaced by a later schedule"
            }).ToList();
            rptSchedule.DataBind();

            var names = schedules.ToDictionary(s => s.FeeScheduleId, s => s.Name);
            rptTiers.DataSource = ctl.GetAllTiers().Select(t => new
            {
                ScheduleName = names.ContainsKey(t.FeeScheduleId) ? names[t.FeeScheduleId] : t.FeeScheduleId.ToString(),
                GroupName = GroupName(t.GroupId),
                TypeName = TypeName(t.TierType),
                t.Value,
                t.Label,
                AmountText = t.Amount.HasValue ? t.Amount.Value.ToString("C") : string.Empty
            }).ToList();
            rptTiers.DataBind();
        }

        private void ClearForm()
        {
            hdFeeScheduleId.Value = string.Empty;
            txtName.Text = string.Empty;
            txtEffectiveDate.Text = string.Empty;
            txtNotes.Text = string.Empty;
        }

        private void ShowMessage(string type, string title, string text, string icon)
        {
            var stringValue = DotNetNuke.Services.Localization.Localization.GetString("Alert.Text", LocalResourceFile.Replace("FeeScheduleList", "Edit"));
            ltMessage.Text = string.Format(stringValue, type, title, Server.HtmlEncode(text), icon);
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (!IsAdmin)
                        Response.Redirect(_navigationManager.NavigateURL());
                    _jsLibraryHelper.RequestRegistration(CommonJs.DnnPlugins);
                    BindList();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            if (!IsAdmin)
                return;
            var ctl = new FeeScheduleController(_hostSettings);
            if (string.IsNullOrEmpty(hdFeeScheduleId.Value))
                return;
            FeeSchedule schedule = ctl.GetSchedule(Convert.ToInt32(hdFeeScheduleId.Value));
            if (schedule == null)
                return;
            string name = (txtName.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                ShowMessage("danger", "Not saved", "A fee schedule needs a name.", "fas fa-exclamation-triangle");
                BindList();
                return;
            }
            DateTime? effective = null;
            if (!string.IsNullOrWhiteSpace(txtEffectiveDate.Text))
            {
                if (!DateTime.TryParse(txtEffectiveDate.Text, out DateTime parsed))
                {
                    ShowMessage("danger", "Not saved", "The effective date is not a valid date.", "fas fa-exclamation-triangle");
                    BindList();
                    return;
                }
                effective = parsed.Date;
            }
            // Two schedules cannot start on the same day, or sessions on that day could not tell which applies.
            if (effective.HasValue && ctl.GetSchedules().Any(s => s.FeeScheduleId != schedule.FeeScheduleId && s.EffectiveDate.HasValue && s.EffectiveDate.Value.Date == effective.Value))
            {
                ShowMessage("danger", "Not saved", "Another fee schedule already starts on that date.", "fas fa-exclamation-triangle");
                BindList();
                return;
            }
            schedule.Name = name;
            schedule.EffectiveDate = effective;
            schedule.Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim();
            schedule.LastModifiedDate = DateTime.Now;
            schedule.LastModifiedById = UserId;
            ctl.UpdateSchedule(schedule);
            ClearForm();
            BindList();
            ShowMessage("success", "Saved", "Fee schedule updated.", "fas fa-thumbs-up");
        }

        protected void pnlFeeSchedules_Unload(object sender, EventArgs e)
        {
            MethodInfo methodInfo = typeof(ScriptManager).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(i => i.Name.Equals("System.Web.UI.IScriptManagerInternal.RegisterUpdatePanel")).First();
            methodInfo.Invoke(ScriptManager.GetCurrent(Page),
                new object[] { sender as UpdatePanel });
        }

        protected void rptSchedule_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "edit")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                FeeSchedule schedule = new FeeScheduleController(_hostSettings).GetSchedule(id);
                if (schedule == null)
                    return;
                hdFeeScheduleId.Value = id.ToString();
                txtName.Text = schedule.Name;
                txtEffectiveDate.Text = schedule.EffectiveDate.HasValue ? schedule.EffectiveDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                txtNotes.Text = schedule.Notes;
                ScriptManager.RegisterStartupScript(rptSchedule, rptSchedule.GetType(), "ToggleForm", "ToggleEditForm(true)", true);
            }
        }

        protected void rptSchedule_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ScriptManager scriptMan = ScriptManager.GetCurrent(this.Page);
                scriptMan.RegisterAsyncPostBackControl((LinkButton)e.Item.FindControl("cmdEdit"));
            }
        }
        #endregion
    }
}
