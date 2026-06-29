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
        public int ExpertID { get; set; }
        public string Description { get; set; }
        public DateTime? ContractEnds { get; set; }
        public string Comments { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
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
        public int ExpertID { get; set; }
        public int TypeID { get; set; }
        public int Sequence { get; set; }

    }
    [TableName("tjc_expert_expert_by_template")]
    //setup the primary key for table
    internal class ExpertTemplate
    {
        public int ExpertID { get; set; }
        public int TemplateID { get; set; }
        public int Position { get; set; }
    }
    [TableName("tjc_expert_expert_by_location")]
    //setup the primary key for table
    internal class ExpertLocation
    {
        public int ExpertID { get; set; }
        public int LocationID { get; set; }
    }
    [TableName("tjc_expert_expert_by_request")]
    //setup the primary key for table
    internal class ExpertRequest
    {
        public int ExpertID { get; set; }
        public int RequestID { get; set; }
        public int Sequence { get; set; }
    }
    [TableName("tjc_expert_expert_request")]
    //setup the primary key for table
    internal class ExpertRequestListItem
    {
        public int ExpertID { get; set; }
        public int RequestID { get; set; }
        public int Sequence { get; set; }
        public string Description { get; set; }
    }
}
