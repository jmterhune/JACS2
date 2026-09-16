using DotNetNuke.Data;
using System.Collections.Generic;
using System.Linq;
namespace tjc.Modules.jacs.Components
{
    internal class CountyController
    {
        private const string CONN_JACS = "jacs"; //Connection

        public void CreateCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                if (!string.IsNullOrWhiteSpace(t.password))
                {
                    t.password = EncryptionHelper.Encrypt(t.password);
                }
                // The form may supply a token + expiration obtained from the "Test Auth
                // Endpoint" button; store the token encrypted, like the password.
                if (!string.IsNullOrWhiteSpace(t.token))
                {
                    t.token = EncryptionHelper.Encrypt(t.token);
                }
                rep.Insert(t);
            }
        }
        public void DeleteCounty(long countyId)
        {
            var t = GetCounty(countyId);
            DeleteCounty(t);
        }
        public void DeleteCounty(County t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                rep.Delete(t);
            }
        }
        public IEnumerable<County> GetCountys()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t;
        }
        public List<KeyValuePair<long,string>> GetCountyDropDownItems()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t.Select(c=>new KeyValuePair<long, string>(c.id,c.name)).OrderBy(c=>c.Value).ToList();
        }
        public List<KeyValuePair<string, string>> GetCountyCodeDropDownItems()
        {
            IEnumerable<County> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.Get();
            }
            return t.Select(c => new KeyValuePair<string, string>(c.code, c.name)).OrderBy(c => c.Value).ToList();
        }
        public County GetCounty(long countyId)
        {
            County t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<County>();
                t = rep.GetById(countyId);
                if (t != null)
                {
                    if (!string.IsNullOrWhiteSpace(t.password))
                    {
                        t.password = EncryptionHelper.Decrypt(t.password);
                    }
                    if (!string.IsNullOrWhiteSpace(t.token))
                    {
                        t.token = EncryptionHelper.Decrypt(t.token);
                    }
                }
            }
            return t;
        }
        public void UpdateCounty(County t)
        {
            string password = string.IsNullOrWhiteSpace(t.password)
                ? t.password
                : EncryptionHelper.Encrypt(t.password);
            string token = string.IsNullOrWhiteSpace(t.token)
                ? t.token
                : EncryptionHelper.Encrypt(t.token);

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                // token + expiration_date are saved along with the rest of the form.
                // CountyAPIController substitutes the stored values when the form posts
                // no token, so a routine edit never wipes an existing one.
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE counties SET name = @0, code = @1, auth_end_point_url = @2, " +
                    "user_name = @3, password = @4, token = @5, expiration_date = @6, updated_at = @7 WHERE id = @8",
                    t.name, t.code, t.auth_end_point_url, t.user_name, password,
                    token, t.expiration_date, System.DateTime.Now, t.id);
            }

            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Counties");
        }
        /// <summary>
        /// Persists a freshly issued auth token (and its expiration) for a county.
        /// The token is encrypted at rest, matching CreateCounty/GetCounty. Only the
        /// token/expiration/updated_at columns are touched so the already-encrypted
        /// password is never re-encrypted or clobbered. The County entity is
        /// [Cacheable], so the cached copy is dropped afterwards — otherwise the next
        /// GetCounty would return the stale expiration and re-authenticate on every
        /// call.
        /// </summary>
        public void SaveAuthToken(long countyId, string plaintextToken, System.DateTime? expiration)
        {
            string encrypted = string.IsNullOrWhiteSpace(plaintextToken)
                ? plaintextToken
                : EncryptionHelper.Encrypt(plaintextToken);

            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                ctx.Execute(System.Data.CommandType.Text,
                    "UPDATE counties SET token = @0, expiration_date = @1, updated_at = @2 WHERE id = @3",
                    encrypted, expiration, System.DateTime.Now, countyId);
            }

            DotNetNuke.Common.Utilities.DataCache.RemoveCache("Counties");
        }
        public IEnumerable<County> GetCountiesPaged(string searchTerm, int rowOffset, int pageSize, string sortOrder, string sortDesc)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteQuery<County>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_county_paged",
                    searchTerm ?? string.Empty,
                    rowOffset,
                    pageSize,
                    sortOrder ?? "description",
                    sortDesc ?? "asc"
                );
            }
        }
        public int GetCountiesCount(string searchTerm)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                return ctx.ExecuteScalar<int>(
                    System.Data.CommandType.StoredProcedure,
                    "tjc_jacs_get_county_count",
                    searchTerm ?? string.Empty
                );
            }
        }
    }
}