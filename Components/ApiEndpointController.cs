using DotNetNuke.Data;
using DotNetNuke.Services.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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
        /// <summary>
        /// Calls the county's auth endpoint (counties.auth_end_point_url) and returns
        /// the issued token together with its expiration. The auth endpoint is a
        /// property of the county — it is deliberately NOT an api_endpoints entry, so
        /// it never goes through CallExternalApi.
        /// </summary>
        internal async Task<AuthTokenResult> GetJwtToken(County county)
        {
            if (county == null || string.IsNullOrWhiteSpace(county.auth_end_point_url))
            {
                Exceptions.LogException(new Exception(
                    $"No auth_end_point_url configured for county {county?.id}."));
                return new AuthTokenResult { Error = "No Auth Endpoint URL is configured for this county." };
            }

            // Contract: { "Username": "...", "Password": "..." } — PascalCase per the
            // auth endpoint spec.
            var authPayload = new
            {
                Username = county.user_name,
                Password = county.password  // already decrypted in CountyController.GetCounty()
            };

            var content = new StringContent(JsonConvert.SerializeObject(authPayload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(county.auth_end_point_url, content).ConfigureAwait(false);
            string body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // Failures come back as { "error": "Descriptive error message" }.
                string apiError = null;
                try { apiError = JObject.Parse(body)["error"]?.Value<string>(); }
                catch { /* not JSON — fall back to the raw body below */ }

                Exceptions.LogException(new Exception(
                    $"Auth token request failed for county {county.id}. URL: {county.auth_end_point_url} | " +
                    $"Status: HTTP {(int)response.StatusCode} {response.StatusCode} | " +
                    $"Error: {(string.IsNullOrWhiteSpace(apiError) ? body : apiError)}"));
                return new AuthTokenResult
                {
                    Error = string.IsNullOrWhiteSpace(apiError)
                        ? $"The auth endpoint returned HTTP {(int)response.StatusCode} {response.StatusCode}."
                        : apiError
                };
            }

            JObject respJson;
            try
            {
                respJson = JObject.Parse(body);
            }
            catch (Exception parseEx)
            {
                Exceptions.LogException(new Exception(
                    $"Auth token response for county {county.id} was not valid JSON: {body}", parseEx));
                return new AuthTokenResult { Error = "The auth endpoint returned a response that was not valid JSON." };
            }

            // A 200 can still carry an error message; treat that as a failure.
            string responseError = respJson["error"]?.Value<string>();
            if (!string.IsNullOrWhiteSpace(responseError))
            {
                Exceptions.LogException(new Exception(
                    $"Auth token request for county {county.id} returned HTTP {(int)response.StatusCode} " +
                    $"but reported an error: {responseError}"));
                // Surface the endpoint's own wording — the admin UI shows this verbatim.
                return new AuthTokenResult { Error = responseError };
            }

            // Contract: { "data": [ { "token": "...", "expiration": "yyyy-MM-dd HH:mm:ss" } ], "error": "" }
            // Only these two fields are read — any extras are ignored. "data" is an array
            // per the spec; a bare object is accepted too rather than failing outright.
            var entry = respJson["data"] as JObject
                ?? (respJson["data"] as JArray)?.FirstOrDefault() as JObject;

            if (entry == null)
            {
                Exceptions.LogException(new Exception(
                    $"Auth token response for county {county.id} contained no data entry: {body}"));
                return new AuthTokenResult { Error = "The auth endpoint response contained no token." };
            }

            string issuedToken = entry["token"]?.Value<string>();

            // Prefer the token's own "exp" claim over the reported "expiration" string.
            // Spec 5.0 labels that string EST, so while Florida is on EDT it reads an
            // hour early and would send us re-authenticating on nearly every call.
            return new AuthTokenResult
            {
                Token = issuedToken,
                Expiration = ReadJwtExpiration(issuedToken) ?? ReadExpirationValue(entry)
            };
        }

        /// <summary>
        /// Reads the "exp" claim out of a JWT, as UTC. That is the moment the clerk's
        /// resource server itself stops accepting the token, so it is authoritative and
        /// carries no timezone ambiguity at all. Returns null for an opaque (non-JWT)
        /// token so the caller can fall back to the reported expiration string.
        /// </summary>
        private static DateTime? ReadJwtExpiration(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var parts = token.Split('.');
                if (parts.Length < 2)
                    return null;

                // base64url -> base64, then restore the stripped padding.
                string payload = parts[1].Replace('-', '+').Replace('_', '/');
                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                    case 1: return null;
                }

                var claims = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
                var exp = claims["exp"];
                if (exp == null || exp.Type == JTokenType.Null)
                    return null;

                return DateTimeOffset.FromUnixTimeSeconds(exp.Value<long>()).UtcDateTime;
            }
            catch
            {
                // Not a readable JWT — the reported expiration is all we have.
                return null;
            }
        }

        /// <summary>
        /// The zone a zone-less clerk timestamp is expressed in. Spec 5.0 calls it "EST";
        /// read as Eastern wall-clock time so it stays correct once daylight time starts.
        /// </summary>
        private static readonly TimeZoneInfo _clerkTimeZone = ResolveClerkTimeZone();

        private static TimeZoneInfo ResolveClerkTimeZone()
        {
            foreach (var id in new[] { "Eastern Standard Time", "America/New_York" })
            {
                try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
                catch { /* try the next spelling */ }
            }

            // Both web servers run in Eastern anyway, so this is a safe last resort.
            return TimeZoneInfo.Local;
        }

        /// <summary>
        /// Converts an Eastern wall-clock reading to UTC. During the two DST changeover
        /// hours a wall-clock time is either ambiguous or never happened; both resolve to
        /// the EARLIEST instant it could mean, so we never overstate the token's life.
        /// </summary>
        private static DateTime EasternToUtc(DateTime wallClock)
        {
            var local = DateTime.SpecifyKind(wallClock, DateTimeKind.Unspecified);
            TimeSpan offset;

            if (_clerkTimeZone.IsAmbiguousTime(local))
            {
                // The repeated hour when DST ends. The largest offset (daylight, -04:00)
                // maps to the earlier UTC instant.
                offset = TimeSpan.MinValue;
                foreach (var candidate in _clerkTimeZone.GetAmbiguousTimeOffsets(local))
                {
                    if (candidate > offset)
                        offset = candidate;
                }
            }
            else if (_clerkTimeZone.IsInvalidTime(local))
            {
                // The skipped hour when DST starts — read it as daylight time.
                offset = _clerkTimeZone.BaseUtcOffset + TimeSpan.FromHours(1);
            }
            else
            {
                offset = _clerkTimeZone.GetUtcOffset(local);
            }

            return DateTime.SpecifyKind(local - offset, DateTimeKind.Utc);
        }

        /// <summary>
        /// True when a timestamp string names its own zone — a trailing Z, or a +/-HH:mm
        /// (or +/-HHmm) offset. Anything else is a bare wall-clock reading.
        /// </summary>
        private static bool HasExplicitZone(string value)
        {
            string s = value.TrimEnd();
            if (s.Length == 0)
                return false;

            char last = s[s.Length - 1];
            if (last == 'Z' || last == 'z')
                return true;

            // An offset occupies the last 5-6 characters; a '-' any earlier than that is
            // part of the date itself (yyyy-MM-dd).
            for (int i = s.Length - 1; i >= 0 && s.Length - i <= 6; i--)
            {
                if (s[i] == '+' || s[i] == '-')
                    return i > 7;
            }

            return false;
        }

        /// <summary>
        /// Returns a usable clerk auth token for the county, refreshing it first when
        /// the cached token is missing, has no expiration, or expires within the next
        /// five minutes. A refreshed token and its expiration are written back to the
        /// counties table.
        ///
        /// Note: the returned value is the PLAINTEXT token for the Bearer header.
        /// CountyController.GetCounty already decrypts county.token in place, so we
        /// read county.token directly — county.decrypted_token would double-decrypt.
        /// </summary>
        internal async Task<string> EnsureValidTokenAsync(County county)
        {
            if (county == null)
                return null;

            // county.expiration_date is always UTC (the County setter guarantees it), so
            // the comparison is against UtcNow — no timezone or DST maths involved.
            bool needsRefresh =
                string.IsNullOrWhiteSpace(county.token)
                || !county.expiration_date.HasValue
                || county.expiration_date.Value <= DateTime.UtcNow.AddMinutes(5);

            if (!needsRefresh)
                return county.token;

            var refreshed = await RefreshAuthTokenAsync(county).ConfigureAwait(false);

            // Refresh failed — fall back to whatever token we already hold so a
            // transient auth outage doesn't block the call outright. Any auth error
            // will surface from the clerk call itself.
            return refreshed?.Token ?? county.token;
        }

        /// <summary>
        /// Unconditionally calls the county's auth endpoint, persists the returned token
        /// and expiration to the counties table, and updates the in-memory county.
        /// Returns what was stored, or null when the endpoint could not be reached or
        /// returned no token. Used by <see cref="EnsureValidTokenAsync"/> and by the
        /// county admin "Test Auth Endpoint" action, which always wants a live call.
        /// </summary>
        internal async Task<AuthTokenResult> RefreshAuthTokenAsync(County county)
        {
            if (county == null)
                return null;

            var auth = await GetJwtToken(county).ConfigureAwait(false);
            if (auth == null || string.IsNullOrWhiteSpace(auth.Token))
                return auth;   // carries Error when the endpoint supplied one

            // If the endpoint returned a token but no parseable expiration, keep a
            // conservative window so we don't re-authenticate on every single call.
            DateTime expiration = auth.Expiration ?? DateTime.UtcNow.AddMinutes(55);
            if (!auth.Expiration.HasValue)
            {
                Exceptions.LogException(new Exception(
                    $"AuthToken response for county {county.id} contained no parseable expiration; " +
                    "defaulting to 55 minutes."));
            }

            try
            {
                // A county that has not been created yet has no row to write to — its
                // token is persisted when the form is saved.
                if (county.id > 0)
                    new CountyController().SaveAuthToken(county.id, auth.Token, expiration);
            }
            catch (Exception saveEx)
            {
                // A failed save is not fatal — the token is still valid for this
                // request; it will simply be re-requested next time.
                Exceptions.LogException(new Exception(
                    $"Failed to persist refreshed auth token for county {county.id}.", saveEx));
            }

            // Update the in-memory county so the rest of this request uses the new values.
            county.token = auth.Token;
            county.expiration_date = expiration;

            return new AuthTokenResult { Token = auth.Token, Expiration = expiration };
        }

        /// <summary>
        /// Reads the "expiration" value from an auth response data entry and returns it
        /// as UTC, whatever shape the clerk sent it in:
        ///
        ///   "2026-09-16 13:29:44"        bare wall clock  -> read as Eastern
        ///   "2026-09-16T13:29:44"        bare ISO         -> read as Eastern
        ///   "2026-09-16T18:29:44Z"       UTC marker       -> used as-is
        ///   "2026-09-16T14:29:44-04:00"  explicit offset  -> converted
        ///   1789583384                   epoch seconds    -> converted
        ///   1789583384000                epoch millis     -> converted
        ///
        /// Only a value that names its own zone is trusted to name it; everything else is
        /// Eastern wall-clock per spec 5.0. Returns null when nothing can be made of it,
        /// which the caller treats as "no expiration".
        /// </summary>
        private static DateTime? ReadExpirationValue(JObject json)
        {
            var tok = json?["expiration"];
            if (tok == null || tok.Type == JTokenType.Null)
                return null;

            // Numeric: epoch seconds, or milliseconds once the value gets too large to
            // be a plausible second count (that threshold is the year 5138).
            if (tok.Type == JTokenType.Integer || tok.Type == JTokenType.Float)
            {
                long epoch = tok.Value<long>();
                if (epoch <= 0)
                    return null;

                return epoch > 100000000000L
                    ? DateTimeOffset.FromUnixTimeMilliseconds(epoch).UtcDateTime
                    : DateTimeOffset.FromUnixTimeSeconds(epoch).UtcDateTime;
            }

            // Json.NET may have already recognised it as a date while parsing.
            if (tok.Type == JTokenType.Date)
            {
                var jv = tok as JValue;
                if (jv != null && jv.Value is DateTimeOffset)
                    return ((DateTimeOffset)jv.Value).UtcDateTime;

                var dateValue = tok.Value<DateTime>();
                switch (dateValue.Kind)
                {
                    case DateTimeKind.Utc: return dateValue;
                    case DateTimeKind.Local: return dateValue.ToUniversalTime();
                    default: return EasternToUtc(dateValue);
                }
            }

            var raw = tok.Value<string>();
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            raw = raw.Trim();

            // A string that names its own zone is authoritative about it.
            if (HasExplicitZone(raw))
            {
                DateTimeOffset dto;
                if (DateTimeOffset.TryParse(raw, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.RoundtripKind, out dto))
                {
                    return dto.UtcDateTime;
                }
            }

            // No zone marker: an Eastern wall-clock reading. The documented shape is
            // "yyyy-MM-dd HH:mm:ss"; the rest are accepted so a change of format on the
            // clerk's side doesn't silently cost us the expiration.
            string[] wallClockFormats =
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd'T'HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss.FFFFFFF",
                "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF",
                "yyyy-MM-dd HH:mm",
                "yyyy-MM-dd'T'HH:mm",
                "MM/dd/yyyy HH:mm:ss",
                "M/d/yyyy h:mm:ss tt",
                "M/d/yyyy h:mm tt"
            };

            DateTime parsed;
            if (DateTime.TryParseExact(raw, wallClockFormats,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out parsed))
            {
                return EasternToUtc(parsed);
            }

            // Last resort — let the framework have a go, then place the result.
            if (DateTime.TryParse(raw, System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind, out parsed))
            {
                switch (parsed.Kind)
                {
                    case DateTimeKind.Utc: return parsed;
                    case DateTimeKind.Local: return parsed.ToUniversalTime();
                    default: return EasternToUtc(parsed);
                }
            }

            Exceptions.LogException(new Exception(
                $"Auth token expiration could not be parsed from value '{raw}'."));
            return null;
        }

        /// <summary>
        /// Unwraps a clerk list response into <typeparamref name="T"/> items. The clerk
        /// wraps payloads as { "data": [ ... ], "error": "" }; a bare array is accepted
        /// too. A non-empty "error" is reported via <paramref name="error"/>, and any
        /// extra fields are ignored. Property matching is case-insensitive.
        /// </summary>
        internal static List<T> ParseClerkListResponse<T>(string json, out string error)
        {
            error = null;
            var items = new List<T>();

            if (string.IsNullOrWhiteSpace(json))
                return items;

            JToken root;
            try
            {
                root = JToken.Parse(json);
            }
            catch
            {
                error = "The clerk returned a response that was not valid JSON.";
                return items;
            }

            JToken payload = root;
            if (root.Type == JTokenType.Object)
            {
                string errText = root["error"]?.Value<string>();
                if (!string.IsNullOrWhiteSpace(errText))
                {
                    error = errText;
                    return items;
                }

                payload = root["data"] ?? root;
            }

            if (payload.Type == JTokenType.Array)
            {
                items = payload.ToObject<List<T>>() ?? new List<T>();
            }
            else if (payload.Type == JTokenType.Object)
            {
                var single = payload.ToObject<T>();
                if (single != null)
                    items.Add(single);
            }

            return items;
        }

        internal class AuthTokenResult
        {
            public string Token { get; set; }
            public DateTime? Expiration { get; set; }
            /// <summary>
            /// Why the call failed; null on success. Carries the auth endpoint's own
            /// "error" text when it supplied one, so the admin UI can show it verbatim.
            /// </summary>
            public string Error { get; set; }
        }

        private static readonly HttpClient _httpClient = new HttpClient(
            new HttpClientHandler
            {
                // ONLY for internal/dev mock API with self-signed cert (required for your 10.212.72.186:8080)
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
        /// <summary>
        /// Issues one clerk request. Kept separate from CallExternalApi so the call can
        /// be replayed with a new bearer token (an HttpRequestMessage is single-use).
        /// </summary>
        private async Task<HttpResponseMessage> SendClerkRequestAsync(
            ApiEndpoint api, HttpMethod method, string requestJson, string bearer)
        {
            using (var request = new HttpRequestMessage(method, api.end_point_url))
            {
                if (!string.IsNullOrWhiteSpace(bearer))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
                }

                if (requestJson != null)
                {
                    request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                }

                return await _httpClient.SendAsync(request).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// True when the clerk's answer means "your token is no good" — a 401/403, or a
        /// body that names an invalid or expired token even though the status said 200.
        /// </summary>
        private static bool IsInvalidTokenResponse(HttpResponseMessage response, string body)
        {
            if (response == null)
                return false;

            int status = (int)response.StatusCode;
            if (status == 401 || status == 403)
                return true;

            if (string.IsNullOrWhiteSpace(body))
                return false;

            return body.IndexOf("invalid token", StringComparison.OrdinalIgnoreCase) >= 0
                || body.IndexOf("token is invalid", StringComparison.OrdinalIgnoreCase) >= 0
                || body.IndexOf("invalid or expired", StringComparison.OrdinalIgnoreCase) >= 0
                || body.IndexOf("expired token", StringComparison.OrdinalIgnoreCase) >= 0
                || body.IndexOf("token has expired", StringComparison.OrdinalIgnoreCase) >= 0
                || body.IndexOf("token expired", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // Replace your existing CallExternalApi method with this version:
        //
        // When `logContext` is supplied the request, response, and any error are
        // persisted to the api_log table automatically — callers don't need to
        // write any logging code themselves. Passing null keeps the old behaviour
        // (no logging) for backward compatibility with unrelated lookup calls
        // we don't care to log.
        internal async Task<HttpResponseMessage> CallExternalApi(
            ApiEndpoint api,
            string token,
            object payload,
            HttpMethod method,
            ApiLogContext logContext = null)
        {
            if (api == null || string.IsNullOrWhiteSpace(api.end_point_url))
            {
                Exceptions.LogException(new Exception("CallExternalApi called with invalid ApiEndpoint"));
                throw new ArgumentNullException(nameof(api));
            }

            // Every api_endpoints call is authorized with the token issued by the
            // county's auth endpoint (counties.auth_end_point_url). Check the stored
            // expiration here and re-authenticate when it is missing or lands within
            // the next five minutes, so no clerk call goes out on a stale token even
            // when the caller resolved its context a long time ago (e.g. bulk cancel).
            County tokenCounty = null;
            try
            {
                tokenCounty = new CountyController().GetCounty(api.county_id);
                string freshToken = await EnsureValidTokenAsync(tokenCounty).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(freshToken))
                    token = freshToken;
            }
            catch (Exception tokenEx)
            {
                // Never let a refresh problem mask the call itself — log it and
                // fall back to the token the caller supplied.
                Exceptions.LogException(new Exception(
                    $"CallExternalApi: auth token refresh failed for county {api.county_id}; " +
                    "proceeding with the caller-supplied token.", tokenEx));
            }

            string requestJson = payload != null ? JsonConvert.SerializeObject(payload) : null;
            string responseBody = null;
            string errorText = null;
            HttpResponseMessage response = null;

            try
            {
                response = await SendClerkRequestAsync(api, method, requestJson, token).ConfigureAwait(false);
                responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // The clerk can reject a token we believed was current — it may have been
                // revoked, or their expiration may not match the one we stored. Get a new
                // token straight from the auth endpoint and replay the call once.
                if (tokenCounty != null && IsInvalidTokenResponse(response, responseBody))
                {
                    Exceptions.LogException(new Exception(
                        $"Clerk API rejected the token for county {api.county_id} ({tokenCounty.name}). " +
                        $"URL: {api.end_point_url} | Status: {(int)response.StatusCode} {response.StatusCode} | " +
                        $"Response: {responseBody} | Re-authenticating and retrying once."));

                    var reauth = await RefreshAuthTokenAsync(tokenCounty).ConfigureAwait(false);
                    if (reauth != null && !string.IsNullOrWhiteSpace(reauth.Token))
                    {
                        response.Dispose();
                        token = reauth.Token;
                        response = await SendClerkRequestAsync(api, method, requestJson, token).ConfigureAwait(false);
                        responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    errorText = $"HTTP {(int)response.StatusCode} {response.StatusCode}";
                    Exceptions.LogException(new Exception(
                        $"Clerk API call failed. URL: {api.end_point_url} | Status: {errorText} | Response: {responseBody}"));
                }

                // Re-hydrate the response body so callers that read Content
                // still get the bytes we already consumed.
                response.Content = new StringContent(responseBody ?? string.Empty, Encoding.UTF8,
                    response.Content?.Headers?.ContentType?.MediaType ?? "application/json");

                return response;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                errorText = $"Timeout after 60s: {ex.Message}";
                Exceptions.LogException(new Exception($"Clerk API TIMED OUT (60s) → URL: {api.end_point_url}", ex));
                throw;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
                Exceptions.LogException(new Exception($"Error in CallExternalApi → URL: {api.end_point_url}", ex));
                throw;
            }
            finally
            {
                // Only the event lifecycle and case/event read endpoints are
                // logged. Cross-reference sync calls (GetClerkJudges,
                // GetClerkCourtrooms) are operational plumbing and not worth
                // persisting to api_log.
                if (ShouldLog(api.type))
                {
                    new ApiLogController().Log(
                        apiEndpointUrl: api.end_point_url,
                        requestPayload: requestJson,
                        responsePayload: responseBody,
                        error: errorText,
                        countyId: api.county_id,
                        eventId: logContext?.EventId,
                        caseId: logContext?.CaseId,
                        userId: logContext?.UserId,
                        action: logContext?.Action ?? api.type.ToString(),
                        application: logContext?.Application ?? ApiLogApplication.JACS);
                }
            }
        }

        /// <summary>
        /// Allow-list of <see cref="ApiEndpointType"/> values that get written to
        /// the api_log table. Keep this list in sync with what the support team
        /// actually needs to audit.
        /// </summary>
        private static bool ShouldLog(ApiEndpointType type)
        {
            switch (type)
            {
                case ApiEndpointType.AddEvent:
                case ApiEndpointType.CancelEvent:
                case ApiEndpointType.GetCase:
                case ApiEndpointType.GetEvent:
                case ApiEndpointType.RescheduleEvent:
                case ApiEndpointType.UpdateEvent:
                    return true;
                default:
                    return false;
            }
        }
    }
}