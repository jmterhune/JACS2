using DotNetNuke.ComponentModel.DataAnnotations;
using Microsoft.SqlServer.Server;
using System;
using System.Web.Caching;
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
}