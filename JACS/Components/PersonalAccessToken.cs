using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("personal_access_tokens")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("PersonalAccessTokens", CacheItemPriority.Default, 20)]
    internal class PersonalAccessToken
    {
        public long id { get; set; }
        public string tokenable_type { get; set; }
        public long tokenable_id { get; set; }
        public string name { get; set; }
        public string token { get; set; }
        public string abilities { get; set; }
        public DateTime? last_used_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}