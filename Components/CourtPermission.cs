using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("court_permissions")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("CourtPermissions", CacheItemPriority.Default, 20)]
    internal class CourtPermission
    {
        public long id { get; set; }
        public long user_id { get; set; }
        public long judge_id { get; set; }
        public bool editable { get; set; }
        public bool active { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class CourtPermissionSearchResult
    {
        public List<CourtPermissionViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class CourtPermissionResult
    {
        public CourtPermission data { get; set; }
        public string error { get; set; }
    }
}
