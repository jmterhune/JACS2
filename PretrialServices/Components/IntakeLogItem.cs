using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.PretrialServices.Components
{
    [TableName("tjc_pts_intake_log")]
    //setup the primary key for table
    [PrimaryKey("LogId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("IntakeLogItems", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class IntakeLogItem : EntityBase
    {
        public long LogId
        {
            get; set;
        }
        public int CountyId
        {
            get; set;
        }
        public DateTime? IntakeDate
        {
            get; set;
        }

        public int? Interviewed
        {
            get; set;
        }

        public int? Assessed
        {
            get; set;
        }

        public int? PtrRecommended
        {
            get; set;
        }

        public int? PtrNotRecommended
        {
            get; set;
        }

        public int? PtrOrdered
        {
            get; set;
        }

        public int? IndigentAssessed
        {
            get; set;
        }
        [IgnoreColumn]
        public int IntakeDay
        {
            get
            {
                return IntakeDate.Value.Day;
            }
        }
        [IgnoreColumn]
        public string FormattedIntakeDate
        {
            get
            {
                if (IntakeDate.HasValue)
                {
                    return IntakeDate.Value.ToShortDateString();
                }
                return "";
            }
        }
    }

    public enum ReportType
    {
        daily = 0,
        weekly = 1,
        monthly = 2,
        yearly = 3
    }
}
