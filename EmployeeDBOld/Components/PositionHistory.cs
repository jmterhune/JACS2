using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Runtime.Remoting.Channels;
using System.Web.Caching;

namespace tjc.Modules.EmployeeDB.Components
{
    [TableName("tjc_employee_position_history")]
    //setup the primary key for table
    [PrimaryKey("PositionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Positions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class PositionHistory : EmployeeBase
    {
        public int PositionId { get; set; }

        public string SocialSecurityNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Description { get; set; }

        public bool IsInternal { get; set; }

        public string EntryType { get; set; }
        [IgnoreColumn]
        public string EntryName
        {
            get
            {
                switch (EntryType)
                {
                    case "O":
                        return "Other";
                    case "T":
                        return "Transfer";
                        case "P":
                        return "Promotion";
                    default:
                        break;
                }
                return "";
            }
        }
    }
}
