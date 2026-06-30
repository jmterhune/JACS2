using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.PretrialServices.Sarasota.Components
{
    [TableName("tjc_pts_sarasota_intake_log")]
    //setup the primary key for table
    [PrimaryKey("LogId", AutoIncrement = true)]
    //configure caching using PetaPoco
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class IntakeLogItem : EntityBase
    {
        public long LogId // bigint
        {
            get; set;
        }

        public DateTime? IntakeDate // smalldatetime
        {
            get; set;
        }

        public int? Interviewed // int
        {
            get; set;
        }

        public int? Assessed // int
        {
            get; set;
        }

        public int? PtrRecommended // int
        {
            get; set;
        }

        public int? PtrNotRecommended // int
        {
            get; set;
        }

        public int? PtrOrdered // int
        {
            get; set;
        }

        public int? IndigentAssessed // int
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
