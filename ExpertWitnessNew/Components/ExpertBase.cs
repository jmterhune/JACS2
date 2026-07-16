using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.ExpertWitness.Components
{
    internal class ExpertBase
    {
        // Audit columns shared by the tjc_expert_* tables (sizes per production intranet.jud12.local).
        public DateTime? CreatedDate { get; set; }  // datetime
        public DateTime? ModifiedDate { get; set; }  // datetime
        public string CreatedBy { get; set; }  // nvarchar(50)
        public string ModifiedBy { get; set; }  // nvarchar(50)
    }
}
