using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components
{
    [TableName("court_counsel_actions")]
    //setup the primary key for table
    [PrimaryKey("ActionId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Actions", CacheItemPriority.Default, 20)]
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class Action:EntityBase
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public bool Active { get; set; }
    }
}
