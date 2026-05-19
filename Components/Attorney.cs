using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("attorneys")]
    [PrimaryKey("id", AutoIncrement = true)]
    // No [Cacheable] — the attorneys table is written by multiple applications
    // (this app and the SubscriberForms registration flow on the public site)
    // running in different app pools, so a class-level cache here goes stale.
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
    internal class AttorneyDropDownItem
    {
        public long id { get; set; }
        public string bar_num { get; set; }
        public string name { get; set; }
        public string label { get; set; }
    }
    internal class AttorneySearchResult
    {
        public List<AttorneyViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class MatchingNameResult
    {
        public IEnumerable<AttorneyViewModel> data { get; set; }
        public string error { get; set; }
    }

    internal class AttorneyResult
    {
        public Attorney data { get; set; }
        public string error { get; set; }
    }
    internal class AttorneyDropDownResult
    {
        public List<AttorneyDropDownItem> data { get; set; }
        public string error { get; set; }
    }
}