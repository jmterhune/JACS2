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
using tjc.Modules.jacs.Services.ViewModels;

namespace tjc.Modules.jacs.Services
{
    [DnnAuthorize]
    public class CountyAPIController : DnnApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCounties(int p1)
        {
            List<CountyViewModel> counties = new List<CountyViewModel>();
            try
            {
                var ctl = new CountyController();
                var allCounties = ctl.GetCountys().Select(t => new CountyViewModel(t)).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new CountySearchResult
                {
                    data = allCounties,
                    error = null
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CountySearchResult
                {
                    data = counties,
                    error = $"Failed to retrieve Counties: {ex.Message}"
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCountyDropDownItems()
        {
            List<KeyValuePair<long, string>> counties = new List<KeyValuePair<long, string>>();

            try
            {
                var ctl = new CountyController();
                counties = ctl.GetCountyDropDownItems();
                return Request.CreateResponse(new ListItemOptionResult { data = counties, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(new ListItemOptionResult { data = counties, error = ex.Message });
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                ctl.DeleteCounty(p1);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County deleted successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCounty(long p1)
        {
            try
            {
                var ctl = new CountyController();
                County county = ctl.GetCounty(p1);
                if (county == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new CountyResult { data = null, error = "County not found" });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new CountyResult { data = county, error = null });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new CountyResult { data = null, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage CreateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                if (string.IsNullOrWhiteSpace(county.name) || string.IsNullOrWhiteSpace(county.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Code are required." });
                }
                county.created_at = DateTime.Now;
                county.updated_at = DateTime.Now;
                ctl.CreateCounty(county);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County created successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public HttpResponseMessage UpdateCounty(JObject p1)
        {
            try
            {
                var ctl = new CountyController();
                var county = p1.ToObject<County>();
                if (county.id <= 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "County ID is required for update." });
                }
                if (string.IsNullOrWhiteSpace(county.name) || string.IsNullOrWhiteSpace(county.code))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Name and Code are required." });
                }
                var existingCounty = ctl.GetCounty(county.id);
                if (existingCounty == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "County not found." });
                }
                if (string.IsNullOrWhiteSpace(county.password))
                {
                    county.password = existingCounty.password;
                }
                if (string.IsNullOrWhiteSpace(county.token))
                {
                    // The form posts a token only when one was just fetched via "Test Auth
                    // Endpoint". Without one, keep what is stored so a routine edit never
                    // wipes a working token (and its expiration).
                    county.token = existingCounty.token;
                    county.expiration_date = existingCounty.expiration_date;
                }
                ctl.UpdateCounty(county);
                return Request.CreateResponse(HttpStatusCode.OK, new { status = 200, message = "County updated successfully" });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = 500, message = ex.Message });
            }
        }

        /// <summary>
        /// Tests the auth endpoint using the values currently ENTERED on the county form
        /// rather than what is stored, so credentials can be validated before the county
        /// is saved — including for a county that does not exist yet. On success the token
        /// + expiration are written straight to the county record (when it already
        /// exists) and also returned so the form can display them; for a county that has
        /// not been created yet, Save persists them instead.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<HttpResponseMessage> TestAuthToken(JObject p1)
        {
            try
            {
                var candidate = new County
                {
                    id = p1 != null && p1["id"] != null ? p1["id"].Value<long>() : 0,
                    auth_end_point_url = p1 != null && p1["auth_end_point_url"] != null ? p1["auth_end_point_url"].Value<string>() : null,
                    user_name = p1 != null && p1["user_name"] != null ? p1["user_name"].Value<string>() : null,
                    // GetJwtToken expects the PLAINTEXT password, which is what the form posts.
                    password = p1 != null && p1["password"] != null ? p1["password"].Value<string>() : null
                };

                if (string.IsNullOrWhiteSpace(candidate.auth_end_point_url))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Enter an Auth Endpoint URL before testing." });
                }

                if (string.IsNullOrWhiteSpace(candidate.user_name) || string.IsNullOrWhiteSpace(candidate.password))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,
                        new { status = 400, message = "Enter a user name and password before testing." });
                }

                // RefreshAuthTokenAsync calls the endpoint and, when the county already
                // exists, writes the token + expiration to its record.
                var result = await new ApiEndpointController().RefreshAuthTokenAsync(candidate);
                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                {
                    // Prefer the auth endpoint's own "error" text so the admin sees the
                    // real reason rather than a generic message.
                    string failure = result != null && !string.IsNullOrWhiteSpace(result.Error)
                        ? result.Error
                        : "The auth endpoint did not return a token. Check the URL and credentials — the response details are in the DNN event log.";

                    return Request.CreateResponse(HttpStatusCode.BadGateway,
                        new { status = 502, message = failure });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    status = 200,
                    message = candidate.id > 0
                        ? "Auth succeeded. Token and expiration saved to the county."
                        : "Auth succeeded. Save the county to store the token.",
                    token = result.Token,
                    expiration_date = result.Expiration
                });
            }
            catch (Exception ex)
            {
                Exceptions.LogException(ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError,
                    new { status = 500, message = ex.Message });
            }
        }

    }
}