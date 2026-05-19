using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("users")]
    [PrimaryKey("id", AutoIncrement = false)]
    [Cacheable("Users", CacheItemPriority.Default, 20)]
    internal class User
    {
        public long id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public DateTime? email_verified_at { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
    internal class SiteUserResult
    {
        public SiteUserViewModel data { get; set; }
        public string error { get; set; }
    }
    internal class UserSearchResult
    {
        public List<UserViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }
    internal class UserResult
    {
        public User data { get; set; }
        public string error { get; set; }
    }
    internal class UserListResult
    {
        public List<UserViewModel> data { get; set; }
        public string error { get; set; }
    }
}