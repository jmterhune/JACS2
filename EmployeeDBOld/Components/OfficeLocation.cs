using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_office_location")]
    //setup the primary key for table
    [PrimaryKey("OfficeLocationId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("OfficeLocations", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class OfficeLocation : EmployeeBase
    {
        public int OfficeLocationId { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

    }
}
