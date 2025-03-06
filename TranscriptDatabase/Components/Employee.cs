using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;
namespace tjc.Modules.TranscriptDatabase.Components
{
    [TableName("tjc_rec_employee")]
    [PrimaryKey("EmployeeID", AutoIncrement = true)]
    [Cacheable("Employees", CacheItemPriority.Default, 20)]
    internal class Employee : EntityBase
    {
        public int EmployeeID { get; set; }
        public int EmployeeTypeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        [IgnoreColumn]
        public string EmployeeName { get { return string.Format("{0}, {1}", LastName, FirstName); } }
        [IgnoreColumn]
        public string EmployeeTypeName
        {
            get
            {
                if (EmployeeTypeID > 0)
                    return Enumerations.GetEnumDescription(EmployeeType);
                return "";
            }
        }
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
}
