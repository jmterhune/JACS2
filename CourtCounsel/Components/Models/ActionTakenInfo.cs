using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.CourtCounsel.Components.Models
{
    [TableName("tjc_cc_action_taken")]
    [PrimaryKey("ActionId", AutoIncrement = true)]
    [Cacheable("tjc_cc_action_taken", CacheItemPriority.Default, 20)]
    public class ActionTakenInfo
    {
        public int ActionId { get; set; }
        public string Action { get; set; }
    }
}
