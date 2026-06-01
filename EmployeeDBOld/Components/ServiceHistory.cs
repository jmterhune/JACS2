using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_service_history")]
    //setup the primary key for table
    [PrimaryKey("ServiceId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ServiceHistories", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class ServiceHistory:EmployeeBase
    {
        public int ServiceId { get; set; }

        public string SocialSecurityNumber { get; set; }

        public DateTime? HireDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public decimal? LastPayRate { get; set; }

        public string CompanyName { get; set; }
    }
}
