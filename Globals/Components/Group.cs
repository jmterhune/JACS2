using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.Globals
{
    [TableName("tjc_gl_group")]
    //setup the primary key for table
    [PrimaryKey("GroupId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Groups", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Group : EntityBase
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int GroupType { get; set; }

        public bool IsSwnGroup { get; set; }
        [IgnoreColumn]
        public GroupTypes GroupTypeName { get; set; }
        public enum GroupTypes
        {
            Internal =0,
            External=1,
            SWN=2
        }
    }
}
