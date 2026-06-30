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
        public int EmployeeID { get; set; }  // int
        public int EmployeeTypeID { get; set; }  // int
        public string LastName { get; set; }  // nvarchar(200)
        public string FirstName { get; set; }  // nvarchar(50)
        public string Title { get; set; }  // nvarchar(50)
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
