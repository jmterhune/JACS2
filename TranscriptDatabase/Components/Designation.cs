using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_designation")]
    [PrimaryKey("DesignationID", AutoIncrement = true)]
    [Cacheable("Designations", CacheItemPriority.Default, 20)]
    public class Designation : EntityBase
    {
        public int DesignationID { get; set; }

        public string dLastName { get; set; }

        public string dFirstName { get; set; }

        public string dMiddleName { get; set; }

        public string County { get; set; }

        public string LowerTribunalCaseNumber { get; set; }

        public string AppellateCaseNumber { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? ReceiptDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool PublicDefenderAppointed { get; set; }

        public bool DeclaredIndigent { get; set; }

        public bool CourtAppointedCounsel { get; set; }

        public DateTime? TranscriptFiled { get; set; }

        public bool AcknowledgmentFiled { get; set; }

        public string Comment { get; set; }

        public bool Archived { get; set; }
        [IgnoreColumn]
        public string DisplayName
        {
            get
            {
                string name = "";
                if (dFirstName != null)
                    name += dFirstName;
                if (dMiddleName != null)
                    name += " " + dMiddleName;

                if (dLastName != null)
                    name += " " + dLastName;

                return name.Trim();
            }
        }
        [IgnoreColumn]
        public string CalendarName
        {
            get
            {
                string name = string.Format("{0}, {1}", dLastName, dFirstName);
                if (dLastName == null)
                    name = dFirstName;
                if (dFirstName == null)
                    name += dLastName;

                return name.Trim();
            }
        }
        [IgnoreColumn]
        public int TrialHearingDays
        {
            get
            {
                int days = 0;
                DateTime currentDate = Null.NullDate;
                foreach (var evt in Events)
                {
                    if (evt.HearingDate != currentDate)
                        days++;
                    currentDate = evt.HearingDate.Value;
                }
                return days;
            }
        }

        [IgnoreColumn]
        public IEnumerable<EventListItem> Events
        {
            get
            {
                var ctl = new EventController();
                return ctl.GetEventListItemsByDesignation(DesignationID);
            }
        }
        [IgnoreColumn]
        public int RequestedExtension
        {
            get
            {
                int extension = 0;
                if (DaysUntilComplete > 30)
                    extension = DaysUntilComplete - 30;
                else
                    extension = 0;
                return extension;
            }
        }
        [IgnoreColumn]
        public int DaysUntilComplete
        {
            get
            {
                int days = 0;
                if (days <= 0)
                {
                    if (Events.Count() > 0)
                        days = Events.Max(evt => evt.DaysUntilComplete);
                }
                return days;
            }
        }
        [IgnoreColumn]
        public DateTime EstimatedFileDate
        {
            get
            {
                DateTime fileDate = DateTime.Now;
                if (fileDate == Null.NullDate)
                    fileDate = ServiceDate.Value.AddDays(DaysUntilComplete);
                return fileDate;
            }
        }
        public int EstimatedPages(int reporterId)
        {
            int pages = 0;
            if (pages <= 0)
            {
                if (reporterId > 0)
                    pages = Events.Where(evt => evt.CourtReporterID == reporterId).Sum(evt => evt.Pages);
                else
                    pages = Events.Sum(evt => evt.Pages);
            }
            return pages;
        }
    }
    [TableName("tjc_rec_designation_list")]
    [PrimaryKey("DesignationID", AutoIncrement = true)]
    public class DesignationListItem : Designation
    {
        public string CreatedByName { get; set; }
        public string CaseNumber { get; set; }
    }
}