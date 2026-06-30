using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_location")]
    //setup the primary key for table
    [PrimaryKey("LocationID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Locations", CacheItemPriority.Default, 20)]
    internal class Location: ExpertBase
    {
        // Column sizes below reflect production (intranet.jud12.local.dbo.tjc_expert_location).
        public int LocationID { get; set; }  // int (identity PK)
        public string LocationName { get; set; }  // nvarchar(250)
    }
}
