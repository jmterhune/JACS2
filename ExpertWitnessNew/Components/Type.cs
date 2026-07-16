using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_type")]
    //setup the primary key for table
    [PrimaryKey("TypeID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("ExperWitnessTypes", CacheItemPriority.Default, 20)]
    internal class Type : ExpertBase
    {
        // Column sizes below reflect production (intranet.jud12.local.dbo.tjc_expert_type).
        public int TypeID { get; set; }  // int (identity PK)
        public string TypeName { get; set; }  // nvarchar(50)
    }
}
