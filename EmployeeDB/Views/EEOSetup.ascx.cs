/*
' Copyright (c) 2026 Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tjc.Modules.EmployeeDB.Components.Controllers;

namespace tjc.Modules.EmployeeDB.Views
{
    public partial class EEOSetup : EmployeeDBModuleBase
    {
        private readonly EeoController _eeo = new EeoController();
        private readonly JobGroupController _jobGroups = new JobGroupController();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsHrAdmin)
                {
                    Response.Redirect(HomeUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Emit the DNN ServicesFramework AntiForgery token so the JS
                // layer can post to the Web API. Adds a hidden
                // __RequestVerificationToken input to the form.
                DotNetNuke.Framework.ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

                if (!IsPostBack)
                {
                    var jan1 = new DateTime(DateTime.Now.Year, 1, 1);
                    dpStartDate.Text = jan1.ToString("yyyy-MM-dd");
                    dpEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    txtYear.Text = DateTime.Now.Year.ToString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>Renders the &lt;option&gt; tags for the EEO modal's
        /// Job Category &lt;select&gt;. Inlined into the markup so the modal
        /// never needs an extra round-trip to populate the dropdown.</summary>
        protected string GetJobGroupOptions()
        {
            var sb = new StringBuilder();
            foreach (var jg in _jobGroups.GetAll().OrderBy(j => j.Description))
            {
                sb.Append("<option value=\"")
                  .Append(jg.JobGroupId)
                  .Append("\">")
                  .Append(HttpUtility.HtmlEncode(jg.Description ?? ""))
                  .Append("</option>");
            }
            return sb.ToString();
        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate, endDate;
                if (!DateTime.TryParse(dpStartDate.Text, out startDate) ||
                    !DateTime.TryParse(dpEndDate.Text, out endDate))
                {
                    return;
                }

                var preview = new List<object>();
                foreach (var jg in _jobGroups.GetAll().OrderBy(j => j.Description))
                {
                    preview.Add(new
                    {
                        JobGroupName = jg.Description,
                        PopulationMale = _eeo.GetGenderCount(jg.JobGroupId, "Male", startDate, endDate),
                        PopulationFemale = _eeo.GetGenderCount(jg.JobGroupId, "Female", startDate, endDate),
                        PopulationWhite = _eeo.GetRaceCount(jg.JobGroupId, "White", startDate, endDate),
                        PopulationBlack = _eeo.GetRaceCount(jg.JobGroupId, "Black", startDate, endDate),
                        PopulationHispanic = _eeo.GetRaceCount(jg.JobGroupId, "Hispanic", startDate, endDate),
                        PopulationAsian = _eeo.GetRaceCount(jg.JobGroupId, "Asian", startDate, endDate),
                        PopulationIndian = _eeo.GetRaceCount(jg.JobGroupId, "Indian", startDate, endDate),
                        PopulationOther = _eeo.GetRaceCount(jg.JobGroupId, "Other", startDate, endDate),
                        HireMale = _eeo.GetGenderHireCount(jg.JobGroupId, "Male", startDate, endDate),
                        HireFemale = _eeo.GetGenderHireCount(jg.JobGroupId, "Female", startDate, endDate),
                        PromoMale = _eeo.GetGenderPromoTransferCount(jg.JobGroupId, "Male", "Promotion", startDate, endDate),
                        PromoFemale = _eeo.GetGenderPromoTransferCount(jg.JobGroupId, "Female", "Promotion", startDate, endDate),
                        TermMale = _eeo.GetGenderTerminationCount(jg.JobGroupId, "Male", startDate, endDate),
                        TermFemale = _eeo.GetGenderTerminationCount(jg.JobGroupId, "Female", startDate, endDate)
                    });
                }

                rptPreview.DataSource = preview;
                rptPreview.DataBind();
                pnlPreview.Visible = true;
                btnAccept.Visible = true;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate, endDate;
                int year;
                if (!DateTime.TryParse(dpStartDate.Text, out startDate) ||
                    !DateTime.TryParse(dpEndDate.Text, out endDate) ||
                    !int.TryParse(txtYear.Text, out year))
                {
                    return;
                }

                _eeo.SaveYearStats(year, startDate, endDate, UserId);

                pnlPreview.Visible = false;
                btnAccept.Visible = false;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
