using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    [TableName("tjc_expert_expert")]
    //setup the primary key for table
    [PrimaryKey("ExpertID", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Experts", CacheItemPriority.Default, 20)]
    internal class Expert : ExpertBase
    {
        // Column sizes below reflect production (intranet.jud12.local.dbo.tjc_expert_expert).
        public int ExpertID { get; set; }  // int (identity PK)
        public string Description { get; set; }  // nvarchar(50)
        public DateTime? ContractEnds { get; set; }  // datetime
        public string Comments { get; set; }  // nvarchar(max)
        public string Email { get; set; }  // nvarchar(255)
        public string Phone { get; set; }  // nvarchar(50)
        [IgnoreColumn]
        public string LocationDisplay
        {
            get
            {
                string locationDisplay = "";
                var ctl = new ExpertController();
                foreach (Location location in ctl.GetExpertLocationLocations(ExpertID))
                {
                    locationDisplay += string.Format("{0}, ", location.LocationName);
                }
                return locationDisplay.Trim().TrimEnd(',');
            }
        }
        [IgnoreColumn]
        public string TypeDisplay
        {
            get
            {
                string typeDisplay = "";
                var ctl = new ExpertController();
                foreach (Type type in ctl.GetExpertTypeTypes(ExpertID))
                {
                    typeDisplay += string.Format("{0}, ", type.TypeName);
                }
                return typeDisplay.Trim().TrimEnd(',');
            }
        }
        [IgnoreColumn]
        public string CommentDisplay
        {
            get
            {
                if (string.IsNullOrEmpty(Comments))
                    return "";
                return string.Format("<i class=\"fas fa-comment-alt\" data-html=\"true\" title=\"{0}\" data-toggle=\"tooltip\"></i>", Comments); 
            }
        }
    }
    [TableName("tjc_expert_expert_by_type")]
    //setup the primary key for table
    internal class ExpertType
    {
        public int ExpertID { get; set; }  // int
        public int TypeID { get; set; }  // int
        public int Sequence { get; set; }  // int

    }
    [TableName("tjc_expert_expert_by_template")]
    //setup the primary key for table
    internal class ExpertTemplate
    {
        public int ExpertID { get; set; }  // int
        public int TemplateID { get; set; }  // int
        public int Position { get; set; }  // int
    }
    [TableName("tjc_expert_expert_by_location")]
    //setup the primary key for table
    internal class ExpertLocation
    {
        public int ExpertID { get; set; }  // int
        public int LocationID { get; set; }  // int
    }
    [TableName("tjc_expert_expert_by_request")]
    //setup the primary key for table
    internal class ExpertRequest
    {
        public int ExpertID { get; set; }  // int
        public int RequestID { get; set; }  // int
        public int Sequence { get; set; }  // int
    }
    [TableName("tjc_expert_expert_request")]
    //setup the primary key for table
    internal class ExpertRequestListItem
    {
        public int ExpertID { get; set; }  // int
        public int RequestID { get; set; }  // int
        public int Sequence { get; set; }  // int
        public string Description { get; set; }  // nvarchar(50)
    }
}
