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
        public int RequestID { get; set; }
        public int TemplateID { get; set; }
        public int LocationID { get; set; }
        public string CaseNumber { get; set; }
    }
    [TableName("tjc_expert_request_list")]
    [PrimaryKey("RequestID", AutoIncrement = true)]
    //configure caching using PetaPoco
    internal class RequestListItem :ExpertBase
    {
        public int RequestID { get; set; }
        public int TemplateID { get; set; }
        public int LocationID { get; set; }
        public string CaseNumber { get; set; }
        public string LocationName { get; set; }
        public string TemplateName { get; set; }
    }
}
