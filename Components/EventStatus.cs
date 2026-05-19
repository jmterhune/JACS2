using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("event_statuses")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("EventStatuss", CacheItemPriority.Default, 20)]
    internal class EventStatus
    {
        public long id { get; set; }
        public string name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class EventStatusSearchResult
    {
        public List<EventStatusViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class EventStatusResult
    {
        public EventStatus data { get; set; }
        public string error { get; set; }
    }


}