using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_emergency_contact")]
    //setup the primary key for table
    [PrimaryKey("ContactId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("EmergencyContacts", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class EmergencyContact
    {
        public int ContactId { get; set; }

        public int? EmployeeId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Relationship { get; set; }

        public string PhoneHome { get; set; }

        public string PhoneWork { get; set; }

        public string PhoneMobile { get; set; }

        public int? CallOrder { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedById { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public int LastModifiedById { get; set; }

    }
}
