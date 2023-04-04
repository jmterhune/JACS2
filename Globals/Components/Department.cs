using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.Globals
{
    [TableName("tjc_gl_divisions")]
    //setup the primary key for table
    [PrimaryKey("DivisionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Departments", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    public class Department:EntityBase
    {
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }
        public bool Active { get; set; }
    }
}
