using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
namespace tjc.Modules.jacs.Components
{
    [TableName("attorneys")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Attorneys", CacheItemPriority.Default, 20)]
    internal class Attorney
    {
        public long id { get; set; }
        public long UserId { get; set; }
        public string name { get; set; }
        public string bar_num { get; set; }
        public string phone { get; set; }
        public bool? scheduling { get; set; }
        public string notes { get; set; }
        public bool? enabled { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public List<string> emails{get;set;}
        [IgnoreColumn]
        public List<string> email_list
        {
            get{ var ctl = new EmailController();
            return ctl.GetEmails(id).Select(x => x.email).ToList();}
        }
    }
}