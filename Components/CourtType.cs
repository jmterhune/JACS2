using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_types")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtTypes", CacheItemPriority.Default, 20)]
    internal class CourtType
    {
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class CourtTypeSearchResult
    {
        public List<CourtTypeViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class CourtTypeResult
    {
        public CourtType data { get; set; }
        public string error { get; set; }
    }

}