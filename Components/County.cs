using DotNetNuke.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Web.Caching;
using tjc.Modules.jacs.Services.ViewModels;
namespace tjc.Modules.jacs.Components
{
    [TableName("counties")]
    [PrimaryKey("id", AutoIncrement = true)]
    [Cacheable("Counties", CacheItemPriority.Default, 20)]
    internal class County
    {
        public long id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string auth_end_point_url { get; set; }
        public string user_name { get; set; }
        public string password { get; set; }
        public string token { get; set; }

        private DateTime? _expiration_date;
        /// <summary>
        /// When the clerk auth token stops being accepted, always held in UTC.
        ///
        /// UTC is the canonical form everywhere: it is what we compare against
        /// DateTime.UtcNow, what we write to the counties table, and — because the Kind
        /// is stamped here — what serializes as "...Z" so the browser can render it in
        /// the admin's own time zone. SQL Server hands DateTime back as Unspecified, so
        /// normalizing in the setter is what keeps a round-tripped value honest.
        /// </summary>
        public DateTime? expiration_date
        {
            get { return _expiration_date; }
            set
            {
                if (!value.HasValue)
                {
                    _expiration_date = null;
                    return;
                }

                _expiration_date = value.Value.Kind == DateTimeKind.Local
                    ? value.Value.ToUniversalTime()
                    : DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
            }
        }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        [IgnoreColumn]
        public string decrypted_password
        {
            get
            {
                if (string.IsNullOrWhiteSpace(password))
                    return password;
                try
                {
                    return EncryptionHelper.Decrypt(password);
                }
                catch
                {
                    return "Decryption Failed";
                }
            }
        }
        [IgnoreColumn]
        public string decrypted_token
        {
            get
            {
                if (string.IsNullOrWhiteSpace(token))
                    return token;
                try
                {
                    return EncryptionHelper.Decrypt(token);
                }
                catch
                {
                    return "Decryption Failed";
                }
            }
        }
    }
    internal class CountySearchResult
    {
        public List<CountyViewModel> data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public int draw { get; set; }
        public string error { get; set; }
    }

    internal class CountyResult
    {
        public County data { get; set; }
        public string error { get; set; }
    }

}