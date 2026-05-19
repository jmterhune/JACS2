using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tjc.Modules.jacs.Components;
using tjc.Modules.jacs.Services.Mappers;
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class EndpointAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetApiEndpoints(int p1)
        {
            List<ApiEndpointViewModel> apiEndpoints = new List<ApiEndpointViewModel>();
            try
            {
                var ctl = new ApiEndpointController();
                var apis = ctl.GetApiEndpoints().Select(t => new ApiEndpointViewModel(t)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new ApiEndpointSearchResult
                {
                    data = apis,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to retrieve API Endpoints: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetTypeDropDownItems()
        {
            List<AttorneyDropDownItem> attorneys = new List<AttorneyDropDownItem>();
            var query = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
            string searchTerm = query.ContainsKey("q") ? query["q"].ToString() : "";

            try
            {
                var ctl = new AttorneyController();
                attorneys = ctl.GetAttorneyDropDownItems(searchTerm);
                return Request.CreateResponse(HttpStatusCode.OK, new AttorneyDropDownResult { data = attorneys, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new AttorneyDropDownResult { data = attorneys, error = $"Failed to retrieve attorney dropdown items: {ex.Message}" });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteApiEndpoint(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid API Endpoint ID." });
                }
                var ctl = new ApiEndpointController();
                var apiEndpoint = ctl.GetApiEndpoint(p1);
                if (apiEndpoint == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "API Endpoint not found." });
                }
                ctl.DeleteApiEndpoint(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "API Endpoint deleted successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to delete API Endpoint: {ex.Message}" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetApiEndpoint(long p1)
        {
            try
            {
                if (p1 <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new ApiEndpointResult { data = null, error = "Invalid API Endpoint ID." });
                }
                var ctl = new ApiEndpointController();
                var apiEndpoint = ctl.GetApiEndpoint(p1);
                if (apiEndpoint == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new ApiEndpointResult { data = null, error = "API Endpoint not found." });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new ApiEndpointResult { data = apiEndpoint, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiEndpointResult { data = null, error = $"Failed to retrieve API Endpoint: {ex.Message}" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateApiEndpoint(JObject p1)
        {
            try
            {
                var apiEndpoint = p1.ToObject<ApiEndpoint>();
                apiEndpoint.id = -1;
                if (string.IsNullOrWhiteSpace(apiEndpoint.end_point_url) || apiEndpoint.county_id < 0 || apiEndpoint.type < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Endpoint URL, Valid County ID, and Valid Endpoint type are required." });
                }
                var ctl = new ApiEndpointController();
                apiEndpoint.created_at = DateTime.Now;
                apiEndpoint.updated_at = DateTime.Now;
                ctl.CreateApiEndpoint(apiEndpoint);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "API Endpoint created successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to create API Endpoint: {ex.Message}" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateApiEndpoint(JObject p1)
        {
            try
            {
                var apiEndpoint = p1.ToObject<ApiEndpoint>();
                if (apiEndpoint.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "API Endpoint ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(apiEndpoint.end_point_url) || apiEndpoint.county_id < 0 || apiEndpoint.type < 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Endpoint URL, Valid County ID, and Valid Endpoint type are required." });
                }
                var ctl = new ApiEndpointController();
                var existingApiEndpoint = ctl.GetApiEndpoint(apiEndpoint.id);
                if (existingApiEndpoint == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "API Endpoint not found." });
                }
                apiEndpoint.updated_at = DateTime.Now;
                ctl.UpdateApiEndpoint(apiEndpoint);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "API Endpoint updated successfully." });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = $"Failed to update API Endpoint: {ex.Message}" });
            }
        }


       

        private string GetSortColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 2:
                    return "enabled";
                case 3:
                    return "name";
                case 4:
                    return "bar_num";
                default:
                    return "name";
            }
        }
    }
}