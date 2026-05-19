using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("event_types")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("EventTypes", CacheItemPriority.Default, 20)]
    internal class EventType
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class EventTypeSearchResult
    {
        public List<EventTypeViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }
    internal class EventTypeListItemResult
    {
        public List<KeyValuePair<long, string>> data { get; set; }
        public string error { get; set; }
    }

    internal class EventTypeResult
    {
        public EventType data { get; set; }
        public string error { get; set; }
    }


}