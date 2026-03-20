using DotNetNuke.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace tjc.Modules.jacs.Components
{
    internal class ApiEndpointController
    {
        private const string CONN_JACS = "jacs"; //Connection
        private const string CONN_JUD12 = "Jud12"; //jud12.flcourts.org

        public void CreateApiEndpoint(ApiEndpoint t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();
                t.created_at = System.DateTime.Now;
                t.updated_at = System.DateTime.Now;
                rep.Insert(t);
            }
        }
        public void DeleteApiEndpoint(long ApiInterfaceId)
        {
            var t = GetApiEndpoint(ApiInterfaceId);
            DeleteApiEndpoint(t);
        }
        public void DeleteApiEndpoint   (ApiEndpoint t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();
                rep.Delete(t);
            }
        }
        public IEnumerable<ApiEndpoint> GetApiEndpoints()
        {
            IEnumerable<ApiEndpoint> t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();
                t = rep.Get();
            }
            return t;
        }
        public ApiEndpoint GetApiEndpoint(long apiEndpointId)
        {
            ApiEndpoint t;
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();
                t = rep.GetById(apiEndpointId);
            }
            return t;
        }

        public void UpdateApiEndpoint(ApiEndpoint t)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();
                t.updated_at = System.DateTime.Now;
                rep.Update(t);
            }
        }
        public ApiEndpoint GetApiEndpointByCountyAndType(long countyId, int type)
        {
            using (IDataContext ctx = DataContext.Instance(CONN_JACS))
            {
                var rep = ctx.GetRepository<ApiEndpoint>();

                return rep.Find("WHERE county_id = @0 AND type = @1", countyId,type)
                          .FirstOrDefault();
            }
        }
        internal async Task<string> GetJwtToken(County county)
        {
            using (var client = new HttpClient())
            {
                var authPayload = new
                {
                    username = county.user_name,
                    password = county.password  // already decrypted in CountyController.GetCounty()
                };

                var content = new StringContent(JsonConvert.SerializeObject(authPayload), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(county.auth_end_point_url, content);

                if (response.IsSuccessStatusCode)
                {
                    var respJson = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
                    return respJson["token"]?.Value<string>();
                }

                return null;
            }
        }
        internal async Task<HttpResponseMessage> CallExternalApi(ApiEndpoint api, string token, object payload, HttpMethod method)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage(method, api.end_point_url);

                if (payload != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                }

                return await client.SendAsync(request);
            }
        }
    }
}