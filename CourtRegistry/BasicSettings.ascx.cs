/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class BasicSettings : CourtRegistryModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    lnkCancel.NavigateUrl = Globals.NavigateURL();
                    PopulateMonths();
                    var ctl = new SettingController();
                    var setting = ctl.GetSettings().FirstOrDefault();
                    if (setting != null)
                    {
                        txtContactEmail.Text = setting.ContactEmail;
                        txtUpdateNotificationSendTo.Text = setting.UpdateNotificationSendTo;
                        txtEditAttorneyNote.Text = setting.EditAttorneyNote;
                        txtEditApplicationNote.Text = setting.EditApplicationNote;
                        txtVerificationNote.Text = setting.VerificationNote;
                        txtApplicationEmail.Text = setting.ApplicationEmail;
                        if (setting.BeginFiscalYearMonth > 0)
                        {
                            drpMonth.SelectedValue = setting.BeginFiscalYearMonth.ToString();
                            PopulateDays(setting.BeginFiscalYearMonth);
                            if (setting.BeginFiscalYearDay > 0)
                                drpDay.SelectedValue = setting.BeginFiscalYearDay.ToString();
                        }
                        else
                        {
                            PopulateDays(1);
                        }
                    }
                    else
                    {
                        PopulateDays(1);
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void PopulateMonths()
        {
            var monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            for (int i = 0; i < monthNames.Length - 1; i++)
                drpMonth.Items.Add(new ListItem(monthNames[i], (i + 1).ToString()));
        }

        private void PopulateDays(int month)
        {
            drpDay.Items.Clear();
            int days = DateTime.DaysInMonth(DateTime.Now.Year, month);
            for (int i = 1; i <= days; i++)
                drpDay.Items.Add(new ListItem(i.ToString()));
        }

        protected void drpMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse(drpMonth.SelectedValue, out int month);
            PopulateDays(month > 0 ? month : 1);
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                var ctl = new SettingController();
                var setting = ctl.GetSettings().FirstOrDefault();
                int.TryParse(drpMonth.SelectedValue, out int month);
                int.TryParse(drpDay.SelectedValue, out int day);
                if (setting == null)
                {
                    setting = new Setting
                    {
                        ID = 1,
                        ContactEmail = txtContactEmail.Text,
                        UpdateNotificationSendTo = txtUpdateNotificationSendTo.Text,
                        EditAttorneyNote = txtEditAttorneyNote.Text,
                        EditApplicationNote = txtEditApplicationNote.Text,
                        VerificationNote = txtVerificationNote.Text,
                        ApplicationEmail = txtApplicationEmail.Text,
                        BeginFiscalYearMonth = month,
                        BeginFiscalYearDay = day
                    };
                    ctl.CreateSetting(setting);
                }
                else
                {
                    setting.ContactEmail = txtContactEmail.Text;
                    setting.UpdateNotificationSendTo = txtUpdateNotificationSendTo.Text;
                    setting.EditAttorneyNote = txtEditAttorneyNote.Text;
                    setting.EditApplicationNote = txtEditApplicationNote.Text;
                    setting.VerificationNote = txtVerificationNote.Text;
                    setting.ApplicationEmail = txtApplicationEmail.Text;
                    setting.BeginFiscalYearMonth = month;
                    setting.BeginFiscalYearDay = day;
                    ctl.UpdateSetting(setting);
                }
                ltMessage.Text = "<span class='alert alert-success'><i class='fas fa-check'></i>&nbsp;Settings saved.</span>";
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
