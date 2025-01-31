using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_employee")]
    [PrimaryKey("EmployeeID", AutoIncrement = true)]
    [Cacheable("Employees", CacheItemPriority.Default, 20)]
    internal class Employee:EntityBase
    {
        public int EmployeeID { get; set; }
        public int EmployeeTypeID { get; set; }
        public string EmployeeName { get; set; }
        [IgnoreColumn]
        [EnumDataType(typeof(EmployeeTypes))]
        public EmployeeTypes EmployeeType
        {
            get
            {
                return (EmployeeTypes)this.EmployeeTypeID;
            }
            set
            {
                this.EmployeeTypeID = (int)value;
            }
        }
    }
public enum EmployeeTypes
    {
        Judge = 0,
        CourtReporter = 1,
        Scopist = 2,
        Transcriptionist = 3,
        Staff = 4
    }
}
