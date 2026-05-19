using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("roles")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Roles", CacheItemPriority.Default, 20)]
    internal class Role
    {
        public long id { get; set; }
        public string name { get; set; }
        public string guard_name { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class RoleSearchResult
    {
        public List<RoleViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }
    internal class RoleResult
    {
        public Role data { get; set; }
        public string error { get; set; }
    }

}