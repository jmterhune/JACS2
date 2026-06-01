using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_race")]
    //setup the primary key for table
    [PrimaryKey("RaceId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Races", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Race:EmployeeBase
    {
        public int RaceId { get; set; }

        public string RaceCode { get; set; }

        public string Description { get; set; }
    }
}
