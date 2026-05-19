using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("courtrooms")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Courtrooms", CacheItemPriority.Default, 20)]
    internal class Courtroom
    {
        public long id { get; set; }
        public string old_id { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }

    [TableName("courtroom_clerk_xref")]
    internal class CourtroomClerkXref
    {
        public long courtroom_id { get; set; }
        public long county_id { get; set; }
        public long clerk_courtroom_id { get; set; }
        public string clerk_courtroom_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class CourtroomClerkXrefListItem: CourtroomClerkXref
    {
        public string county_name { get; set; } = null;
        public string courtroom_name { get; set; } = null;
    }
    internal class CourtroomXrefItem
    {
        public long CourtRoomId { get; set; }
        public string CourtroomName { get; set; } = string.Empty;
    }
    internal class CourtroomClerkXrefResult
    {
        public List<CourtroomClerkXrefViewModel> data { get; set; }
        public string error { get; set; }
    }
    internal class CourtroomSearchResult
    {
        public List<CourtroomViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }
    internal class CourtroomResult
    {
        public Courtroom data { get; set; }
        public string error { get; set; }
    }
    internal class CourtroomListItemResult
    {
        public List<KeyValuePair<long, string>> data { get; set; }
        public string error { get; set; }
    }

}
