using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_request")]
    //setup the primary key for table
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Requests", CacheItemPriority.Default, 20)]
    internal class Request : ExpertBase
    {
        // Column sizes below reflect production (intranet.jud12.local.dbo.tjc_expert_request).
        public int RequestID { get; set; }  // int (identity PK)
        public int TemplateID { get; set; }  // int
        public int LocationID { get; set; }  // int
        public string CaseNumber { get; set; }  // nvarchar(50)
    }
    [TableName("tjc_expert_request_list")]
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class RequestListItem :ExpertBase
    {
        // Maps to the tjc_expert_request_list view (sizes per production).
        public int RequestID { get; set; }  // int
        public int TemplateID { get; set; }  // int
        public int LocationID { get; set; }  // int
        public string CaseNumber { get; set; }  // nvarchar(50)
        public string LocationName { get; set; }  // nvarchar(250)
        public string TemplateName { get; set; }  // nvarchar(200)
    }
}
