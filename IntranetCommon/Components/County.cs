using DotNetNuke.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.IntranetCommon.Components
{
    [TableName("tjc_gl_counties")]
    //setup the primary key for table
    [PrimaryKey("CountyId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Counties", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    public class County : EntityBase
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
    }
}
