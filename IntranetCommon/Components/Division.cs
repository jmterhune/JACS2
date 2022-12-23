using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.IntranetCommon.Components
{
    [TableName("tjc_gl_divisions")]
    //setup the primary key for table
    [PrimaryKey("DivisionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Divisions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    public class Division :EntityBase
    {
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public bool Active { get; set; }
    }
}
