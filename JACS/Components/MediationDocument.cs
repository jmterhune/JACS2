using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("mediation_documents")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("MediationDocuments", CacheItemPriority.Default, 20)]
    internal class MediationDocument
    {
        public long id { get; set; }
        public string d_title { get; set; }
        public DateTime d_valid_date { get; set; }
        public string d_ext { get; set; }
        public string d_original { get; set; }
        public long d_u_id { get; set; }
        public string d_fname { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}