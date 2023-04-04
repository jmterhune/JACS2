using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_position_history")]
    //setup the primary key for table
    [PrimaryKey("PositionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Positions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class PositionHistory:EmployeeBase
    {
        public int PositionId { get; set; }

        public string SocialSecurityNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public int? Rating { get; set; }

        public bool IsInternal { get; set; }

        public string EntryType { get; set; }

    }
}
