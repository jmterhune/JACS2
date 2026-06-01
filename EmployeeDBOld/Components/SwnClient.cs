#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."

namespace tjc.Modules.EmployeeDB.Components.Services
{
    using DotNetNuke.Common.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net.Http.Headers;
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.8.2.0 (NJsonSchema v10.2.1.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class SwnClient
    {
        private string _baseUrl = "https://api.onsolve.com/v1";
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;

        public SwnClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient;
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(CreateSerializerSettings);
        }

        private Newtonsoft.Json.JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            UpdateJsonSerializerSettings(settings);
            return settings;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }

        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);

        /// <summary>Login</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="loginRequest">Login data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<TokenInformation> POSTLoginAsync(string x_Service_Identifier, string ocp_apim_subscription_key, LoginRequest loginRequest)
        {
            return POSTLoginAsync(x_Service_Identifier, ocp_apim_subscription_key, loginRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Login</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="loginRequest">Login data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<TokenInformation> POSTLoginAsync(string x_Service_Identifier, string ocp_apim_subscription_key, LoginRequest loginRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/login");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("ococp_apim_subscription_key");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));

                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(loginRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<TokenInformation>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Invalid username or password.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Contact> GETContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETContactsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Contact> GETContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<Contact>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Update contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactRequest">Contact data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Contact> PUTContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactRequest contactRequest)
        {
            return PUTContactsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, contactRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactRequest">Contact data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Contact> PUTContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactRequest contactRequest, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(contactRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<Contact>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            
                            return new ObjectResponseResult<Contact>().Object;
                            //throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Delete contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DELETEContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return DELETEContactsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DELETEContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Update contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PATCHContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray)
        {
            return PATCHContactsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, operationArray, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update contact by id</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PATCHContactsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(operationArray, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PATCH");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve contacts</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="first_name">Search string for first name. (Supported for MIR3 and SWN only)</param>
        /// <param name="last_name">Search string for last name. (Supported for MIR3 and SWN only)</param>
        /// <param name="employee_id">Search string for employee id. (Supported for MIR3 only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Contact> GETContactsAsync(int? page, int? per_page, string first_name, string last_name, string employee_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETContactsAsync(page, per_page, first_name, last_name, employee_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve contacts</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="first_name">Search string for first name. (Supported for MIR3 and SWN only)</param>
        /// <param name="last_name">Search string for last name. (Supported for MIR3 and SWN only)</param>
        /// <param name="employee_id">Search string for employee id. (Supported for MIR3 only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Contact> GETContactsAsync(int? page, int? per_page, string first_name, string last_name, string employee_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (first_name != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("first_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(first_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (last_name != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("last_name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(last_name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (employee_id != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("employee_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(employee_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<Contact>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Add new contact</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactRequest">Contact data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<Contact> POSTContactsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactRequest contactRequest)
        {
            return POSTContactsAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, contactRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add new contact</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactRequest">Contact data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<Contact> POSTContactsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactRequest contactRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(contactRequest, _settings.Value);
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(contactRequest, _settings.Value));

                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ICollection<Contact>>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object.FirstOrDefault();
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 409)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The contact id already exists.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            foreach (var item in objectResponse_.Object.Errors)
                            {
                                string process = string.Format("SWN Validation Add {0} as SWN Contact ", contactRequest.Employee_id);
                                SwnLog swnLog = new SwnLog { CreatedBy = 0, CreatedDate = DateTime.Now, Exception = item.Field.ToString(), Process = process };
                                var logCtl = new SwnLogController();
                                logCtl.CreateSwnLog(swnLog);

                            }
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>List all contact groups memberships</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Membership>> GETContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETContactsIdGroupsAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>List all contact groups memberships</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<Membership>> GETContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}/groups");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.ICollection<Membership>>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Add a member to a group</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="groupMemberGroupModel">Groups id</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PUTContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GroupMemberGroupModel groupMemberGroupModel)
        {
            return PUTContactsIdGroupsAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, groupMemberGroupModel, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add a member to a group</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="groupMemberGroupModel">Groups id</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PUTContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GroupMemberGroupModel groupMemberGroupModel, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}/groups");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(groupMemberGroupModel, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Add a member to a group</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="groupMemberGroupModel">Groups id</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task POSTContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GroupMemberGroupModel groupMemberGroupModel)
        {
            return POSTContactsIdGroupsAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, groupMemberGroupModel, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add a member to a group</summary>
        /// <param name="id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="groupMemberGroupModel">Groups id</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task POSTContactsIdGroupsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GroupMemberGroupModel groupMemberGroupModel, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}/groups");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(groupMemberGroupModel, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve contact groups</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="id">Search string for group id. (Supported for MIR3 and SWN only)</param>
        /// <param name="name">Search string for group name. (Supported for CR only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgGroup> GETGroupsAsync(int? page, int? per_page, string id, string name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETGroupsAsync(page, per_page, id, name, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve contact groups</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="id">Search string for group id. (Supported for MIR3 and SWN only)</param>
        /// <param name="name">Search string for group name. (Supported for CR only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgGroup> GETGroupsAsync(int? page, int? per_page, string id, string name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (id != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (name != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgGroup>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Add new contact group</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactGroupRequest">Group data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgGroupResponseDetails> POSTGroupsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactGroupRequest contactGroupRequest)
        {
            return POSTGroupsAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, contactGroupRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add new contact group</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactGroupRequest">Group data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgGroupResponseDetails> POSTGroupsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactGroupRequest contactGroupRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));

                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(contactGroupRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201 | status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgGroupResponseDetails>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 409)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The group id already exists.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgGroupDetails> GETGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETGroupsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgGroupDetails> GETGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));

                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgGroupDetails>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Update contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactGroupRequest">Group data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgGroupDetails> PUTGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactGroupRequest contactGroupRequest)
        {
            return PUTGroupsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, contactGroupRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="contactGroupRequest">Group data model</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgGroupDetails> PUTGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, ContactGroupRequest contactGroupRequest, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));

                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(contactGroupRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    string responseBody = await response_.Content.ReadAsStringAsync();

                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgGroupDetails>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Delete contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DELETEGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return DELETEGroupsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DELETEGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Update contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PATCHGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray)
        {
            return PATCHGroupsIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, operationArray, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update contact group by id</summary>
        /// <param name="id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PATCHGroupsIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(operationArray, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PATCH");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve statistics by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageStatistic> GETMessagesIdStatisticsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETMessagesIdStatisticsAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve statistics by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageStatistic> GETMessagesIdStatisticsAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/messages/{id}/statistics");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageStatistic>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Message not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Send ad-hoc message</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="textMessageRequest">Message data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<TextMessage> POSTMessagesSendAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, TextMessageRequest textMessageRequest)
        {
            return POSTMessagesSendAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, textMessageRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Send ad-hoc message</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="textMessageRequest">Message data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<TextMessage> POSTMessagesSendAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, TextMessageRequest textMessageRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/messages/send");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(textMessageRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<TextMessage>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Terminate in-progress message by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PATCHMessagesIdCancelAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return PATCHMessagesIdCancelAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Terminate in-progress message by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PATCHMessagesIdCancelAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/messages/{id}/cancel");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                    request_.Method = new System.Net.Http.HttpMethod("PATCH");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Message not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve message history by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="contact_id">Contact Id</param>
        /// <param name="responded_id">Response Id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageHistoryModel> GETMessagesIdHistoryAsync(string id, string contact_id, string responded_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETMessagesIdHistoryAsync(id, contact_id, responded_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve message history by id</summary>
        /// <param name="id">Message id</param>
        /// <param name="contact_id">Contact Id</param>
        /// <param name="responded_id">Response Id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageHistoryModel> GETMessagesIdHistoryAsync(string id, string contact_id, string responded_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/messages/{id}/history?");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
            if (contact_id != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("contact_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (responded_id != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("responded_id") + "=").Append(System.Uri.EscapeDataString(ConvertToString(responded_id, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageHistoryModel>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Message not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Send scenario</summary>
        /// <param name="id">Scenario id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="swgScenario">Scenario data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageScenario> POSTScenariosIdLaunchAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, SwgScenario swgScenario)
        {
            return POSTScenariosIdLaunchAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, swgScenario, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Send scenario</summary>
        /// <param name="id">Scenario id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="swgScenario">Scenario data model</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageScenario> POSTScenariosIdLaunchAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, SwgScenario swgScenario, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/scenarios/{id}/launch");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(swgScenario, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageScenario>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Scenario not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve scenario by id</summary>
        /// <param name="id">Scenario id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageScenarioDetails> GETScenariosIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETScenariosIdAsync(id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve scenario by id</summary>
        /// <param name="id">Scenario id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageScenarioDetails> GETScenariosIdAsync(string id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/scenarios/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageScenarioDetails>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Scenario not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve scenarios</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="name">Scenario name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageScenario> GETScenariosAsync(int? page, int? per_page, string name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETScenariosAsync(page, per_page, name, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve scenarios</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="name">Scenario name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageScenario> GETScenariosAsync(int? page, int? per_page, string name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/scenarios?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (name != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("name") + "=").Append(System.Uri.EscapeDataString(ConvertToString(name, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageScenario>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Scenario not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve schedules</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task GETSchedulesAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETSchedulesAsync(page, per_page, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve schedules</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task GETSchedulesAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/schedules?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            return;
                        }
                        else
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Schedule not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve address types</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgAddressTypeDetails> GETAddresstypesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETAddresstypesAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve address types</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgAddressTypeDetails> GETAddresstypesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/addresstypes");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgAddressTypeDetails>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No address type found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Add new address type</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgAddressType> POSTAddresstypesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, AddressTypeRequest addressTypeRequest)
        {
            return POSTAddresstypesAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, addressTypeRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add new address type</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgAddressType> POSTAddresstypesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, AddressTypeRequest addressTypeRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/addresstypes");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(addressTypeRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgAddressType>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 409)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The address type is already existing.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve audios</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<AudioRecording> GETAudiosAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETAudiosAsync(page, per_page, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve audios</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<AudioRecording> GETAudiosAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/audios?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<AudioRecording>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Audio not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve carriers</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgCarrier> GETCarriersAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETCarriersAsync(page, per_page, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve carriers</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgCarrier> GETCarriersAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/carriers?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgCarrier>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Carrier not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve custom fields</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgCustomField> GETCustomfieldsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETCustomfieldsAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve custom fields</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgCustomField> GETCustomfieldsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/customfields");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgCustomField>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Custom field not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Update custom fields</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PUTCustomfieldsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CustomFieldRequest customFieldRequest)
        {
            return PUTCustomfieldsAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, customFieldRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update custom fields</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PUTCustomfieldsAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CustomFieldRequest customFieldRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/customfields");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(customFieldRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Custom field not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve a list of message history summary</summary>
        /// <param name="from_date">The latest date for retrieve data (yyyy-MM-dd)</param>
        /// <param name="count">Format - int32. Number of results (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<MessageHistorySummary> GETMessagesAsync(string from_date, int? count, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETMessagesAsync(from_date, count, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve a list of message history summary</summary>
        /// <param name="from_date">The latest date for retrieve data (yyyy-MM-dd)</param>
        /// <param name="count">Format - int32. Number of results (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<MessageHistorySummary> GETMessagesAsync(string from_date, int? count, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/messages?");
            if (from_date != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("from_date") + "=").Append(System.Uri.EscapeDataString(ConvertToString(from_date, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (count != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("count") + "=").Append(System.Uri.EscapeDataString(ConvertToString(count, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<MessageHistorySummary>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve timezones</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgTimezone> GETTimezonesAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETTimezonesAsync(page, per_page, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve timezones</summary>
        /// <param name="page">Format - int32. Page number (Default = 1)</param>
        /// <param name="per_page">Format - int32. Page size (Default = 20)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgTimezone> GETTimezonesAsync(int? page, int? per_page, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/timezones?");
            if (page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            if (per_page != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("per_page") + "=").Append(System.Uri.EscapeDataString(ConvertToString(per_page, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgTimezone>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Timezone not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Set contact addresses.</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful request and there is no error.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task PUTContactaddressesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<ContactLocationRequest> contactLocationRequestArray)
        {
            return PUTContactaddressesAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, contactLocationRequestArray, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Set contact addresses.</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful request and there is no error.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task PUTContactaddressesAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<ContactLocationRequest> contactLocationRequestArray, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contactaddresses");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(contactLocationRequestArray, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 415)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Unsupported Media-Type.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve cascade profile summary</summary>
        /// <param name="id">Contact id</param>
        /// <param name="as_of_date">Date filter (yyyy-MM-ddTHH:mm:ss)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> GETContactsIdCascadeprofilesAsync(string id, string as_of_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETContactsIdCascadeprofilesAsync(id, as_of_date, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve cascade profile summary</summary>
        /// <param name="id">Contact id</param>
        /// <param name="as_of_date">Date filter (yyyy-MM-ddTHH:mm:ss)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> GETContactsIdCascadeprofilesAsync(string id, string as_of_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}/cascadeprofiles?");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
            if (as_of_date != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("as_of_date") + "=").Append(System.Uri.EscapeDataString(ConvertToString(as_of_date, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<System.Collections.Generic.ICollection<string>>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or profile not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Delete credentials cache</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task DELETECredentialscacheAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return DELETECredentialscacheAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete credentials cache</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task DELETECredentialscacheAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/credentialscache");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve Global Cascade status</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<GlobalCascadeStatus> GETGlobalcascadeStatusAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETGlobalcascadeStatusAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve Global Cascade status</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<GlobalCascadeStatus> GETGlobalcascadeStatusAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/globalcascade/status");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<GlobalCascadeStatus>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Disable or Enable the Global Cascade</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<GlobalCascadeStatus> POSTGlobalcascadeStatusAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GlobalCascadeStatusRequest globalCascadeStatusRequest)
        {
            return POSTGlobalcascadeStatusAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, globalCascadeStatusRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Disable or Enable the Global Cascade</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<GlobalCascadeStatus> POSTGlobalcascadeStatusAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, GlobalCascadeStatusRequest globalCascadeStatusRequest, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/globalcascade/status");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(globalCascadeStatusRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<GlobalCascadeStatus>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve Global Voice Cascade Order</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<string> GETGlobalcascadeVoicecascadeorderAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return GETGlobalcascadeVoicecascadeorderAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve Global Voice Cascade Order</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<string> GETGlobalcascadeVoicecascadeorderAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/globalcascade/voicecascadeorder");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<string>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No Global Cascade Order existing.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Set the Global Voice Cascade Order</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<string> POSTGlobalcascadeVoicecascadeorderAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<string> voiceCascadeOrders)
        {
            return POSTGlobalcascadeVoicecascadeorderAsync(x_Service_Identifier, ocp_apim_subscription_key, authorization, voiceCascadeOrders, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Set the Global Voice Cascade Order</summary>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<string> POSTGlobalcascadeVoicecascadeorderAsync(string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<string> voiceCascadeOrders, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/globalcascade/voicecascadeorder");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(voiceCascadeOrders, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<string>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No Global Cascade Order existing.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Remove a group association from a contact</summary>
        /// <param name="id">Contact id</param>
        /// <param name="group_id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task IdAsync(string id, string group_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return IdAsync(id, group_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Remove a group association from a contact</summary>
        /// <param name="id">Contact id</param>
        /// <param name="group_id">Group id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task IdAsync(string id, string group_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");

            if (group_id == null)
                throw new System.ArgumentNullException("group_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{id}/groups/{group_id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{group_id}", System.Uri.EscapeDataString(ConvertToString(group_id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Retrieve all members of the group</summary>
        /// <param name="group_id">Group id</param>
        /// <param name="modified_date">Filter group contacts by modified date. (Supported for SWN only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgGroupMemberResponse> IdMembersAsync(string group_id, string modified_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return IdMembersAsync(group_id, modified_date, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve all members of the group</summary>
        /// <param name="group_id">Group id</param>
        /// <param name="modified_date">Filter group contacts by modified date. (Supported for SWN only)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgGroupMemberResponse> IdMembersAsync(string group_id, string modified_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (group_id == null)
                throw new System.ArgumentNullException("group_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/groups/{group_id}/members?");
            urlBuilder_.Replace("{group_id}", System.Uri.EscapeDataString(ConvertToString(group_id, System.Globalization.CultureInfo.InvariantCulture)));
            if (modified_date != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("modified_date") + "=").Append(System.Uri.EscapeDataString(ConvertToString(modified_date, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.Authorization = authorization;
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgGroupMemberResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact group not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }
        /// <summary>Delete address type</summary>
        /// <param name="address_type">Address type</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task TypeAsync(string address_type, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return TypeAsync(address_type, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete address type</summary>
        /// <param name="address_type">Address type</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task TypeAsync(string address_type, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (address_type == null)
                throw new System.ArgumentNullException("address_type");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/addresstypes/{address_type}");
            urlBuilder_.Replace("{address_type}", System.Uri.EscapeDataString(ConvertToString(address_type, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Address type not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }
        /// <summary>Update address type</summary>
        /// <param name="address_type">Address type</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task TypeAsync(string address_type, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray)
        {
            return TypeAsync(address_type, x_Service_Identifier, ocp_apim_subscription_key, authorization, operationArray, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update address type</summary>
        /// <param name="address_type">Address type</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <param name="operationArray">Patch data model</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task TypeAsync(string address_type, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<Operation> operationArray, System.Threading.CancellationToken cancellationToken)
        {
            if (address_type == null)
                throw new System.ArgumentNullException("address_type");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/addresstypes/{address_type}");
            urlBuilder_.Replace("{address_type}", System.Uri.EscapeDataString(ConvertToString(address_type, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(operationArray, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PATCH");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Address type not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }
        /// <summary>Retrieve list of contact addresses</summary>
        /// <param name="contact_id">Contact ids (separated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<ContactLocation> IdAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return IdAsync(contact_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve list of contact addresses</summary>
        /// <param name="contact_id">Contact ids (separated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>OK.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<ContactLocation> IdAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contactaddresses/{contact_id}");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ContactLocation>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact address not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }



        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }

        public bool ReadResponseAsString { get; set; }

        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T), string.Empty);
            }
            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseText, JsonSerializerSettings);
                    return new ObjectResponseResult<T>(typedBody, responseText);
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var streamReader = new System.IO.StreamReader(responseStream))
                    using (var jsonTextReader = new Newtonsoft.Json.JsonTextReader(streamReader))
                    {
                        var serializer = Newtonsoft.Json.JsonSerializer.Create(JsonSerializerSettings);
                        var typedBody = serializer.Deserialize<T>(jsonTextReader);
                        return new ObjectResponseResult<T>(typedBody, string.Empty);
                    }
                }
                catch (Newtonsoft.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }

        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return null;
            }

            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute))
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }

                    return System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                }
            }
            else if (value is bool)
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[])value);
            }
            else if (value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array)value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }

            var result = System.Convert.ToString(value, cultureInfo);
            return (result is null) ? string.Empty : result;
        }
    }
    public partial class SwnClient
    {

        /// <summary>Retrieve cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Cascade profile names (seperated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful retrieving.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<CascadeSchedule> NameAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return NameAsync(contact_id, profile_name, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Retrieve cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Cascade profile names (seperated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful retrieving.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<CascadeSchedule> NameAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            if (profile_name == null)
                throw new System.ArgumentNullException("profile_name");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeschedules/{profile_name}");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{profile_name}", System.Uri.EscapeDataString(ConvertToString(profile_name, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<CascadeSchedule>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The contact id is not existing or there is no cascade schedule.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing cascade profile name.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        /// <summary>Delete cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="activation_date">Remove cascade schedule by activation dates (seperated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful deletion.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task NameAsync(string contact_id, string profile_name, string activation_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return NameAsync(contact_id, profile_name, activation_date, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Delete cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="activation_date">Remove cascade schedule by activation dates (seperated by commas)</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful deletion.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task NameAsync(string contact_id, string profile_name, string activation_date, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            if (profile_name == null)
                throw new System.ArgumentNullException("profile_name");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeschedules/{profile_name}?");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{profile_name}", System.Uri.EscapeDataString(ConvertToString(profile_name, System.Globalization.CultureInfo.InvariantCulture)));
            if (activation_date != null)
            {
                urlBuilder_.Append(System.Uri.EscapeDataString("activation_date") + "=").Append(System.Uri.EscapeDataString(ConvertToString(activation_date, System.Globalization.CultureInfo.InvariantCulture))).Append("&");
            }
            urlBuilder_.Length--;

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("DELETE");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or profile name not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The data type of activation dates are invalid.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

    }

    public partial class SwnClient
    {
        /// <summary>Update cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful replacement.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task IdCascadeschedulesAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<CascadeScheduleRequest> cascadeScheduleRequestArray)
        {
            return IdCascadeschedulesAsync(contact_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, cascadeScheduleRequestArray, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update cascade schedule.</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Successful replacement.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task IdCascadeschedulesAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Collections.Generic.IEnumerable<CascadeScheduleRequest> cascadeScheduleRequestArray, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeschedules");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(cascadeScheduleRequestArray, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The contact id is not existing or profile name is not existing.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Validation errors.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

    }

    public partial class SwnClient
    {
        /// <summary>Add a new cascade profiles to individual contact</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgContactCascadeProfile> IdCascadeprofilesAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CascadeProfileRequest cascadeProfileRequest)
        {
            return IdCascadeprofilesAsync(contact_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, cascadeProfileRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Add a new cascade profiles to individual contact</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgContactCascadeProfile> IdCascadeprofilesAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CascadeProfileRequest cascadeProfileRequest, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeprofiles");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(cascadeProfileRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgContactCascadeProfile>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

    }
    public partial class SwnClient
    {

        /// <summary>Update cascade profiles</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwgContactCascadeProfile> NameAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CascadeProfileRequest cascadeProfileRequest)
        {
            return NameAsync(contact_id, profile_name, x_Service_Identifier, ocp_apim_subscription_key, authorization, cascadeProfileRequest, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Update cascade profiles</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Created.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<SwgContactCascadeProfile> NameAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, CascadeProfileRequest cascadeProfileRequest, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            if (profile_name == null)
                throw new System.ArgumentNullException("profile_name");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeprofiles/{profile_name}");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{profile_name}", System.Uri.EscapeDataString(ConvertToString(profile_name, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(cascadeProfileRequest, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("PUT");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 201)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<SwgContactCascadeProfile>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 204)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Success with no content.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or profile not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 422)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ValidationResponse>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ValidationResponse>("Validation errors.", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }


        /// <summary>Set active cascade profiles from the individual contact</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task NameActiveAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return NameActiveAsync(contact_id, profile_name, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Set active cascade profiles from the individual contact</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="profile_name">Profile name</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Success with no content.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task NameActiveAsync(string contact_id, string profile_name, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            if (profile_name == null)
                throw new System.ArgumentNullException("profile_name");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/cascadeprofiles/{profile_name}/active");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));
            urlBuilder_.Replace("{profile_name}", System.Uri.EscapeDataString(ConvertToString(profile_name, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                    request_.Method = new System.Net.Http.HttpMethod("PUT");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 204)
                        {
                            return;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 404)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Contact or profile not found.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }
    }
    public partial class SwnClient
    {
        /// <summary>Is Contact In Account</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<ContactInAccount> IdIscontactinaccountAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization)
        {
            return IdIscontactinaccountAsync(contact_id, x_Service_Identifier, ocp_apim_subscription_key, authorization, System.Threading.CancellationToken.None);
        }

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <summary>Is Contact In Account</summary>
        /// <param name="contact_id">Contact id</param>
        /// <param name="x_Service_Identifier">Values: OCN/SWN/MIR3/CR</param>
        /// <param name="authorization">JWT Token</param>
        /// <returns>Ok.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task<ContactInAccount> IdIscontactinaccountAsync(string contact_id, string x_Service_Identifier, string ocp_apim_subscription_key, AuthenticationHeaderValue authorization, System.Threading.CancellationToken cancellationToken)
        {
            if (contact_id == null)
                throw new System.ArgumentNullException("contact_id");

            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append("/contacts/{contact_id}/iscontactinaccount");
            urlBuilder_.Replace("{contact_id}", System.Uri.EscapeDataString(ConvertToString(contact_id, System.Globalization.CultureInfo.InvariantCulture)));

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    if (x_Service_Identifier == null)
                        throw new System.ArgumentNullException("x_Service_Identifier");
                    request_.Headers.TryAddWithoutValidation("X-Service-Identifier", ConvertToString(x_Service_Identifier, System.Globalization.CultureInfo.InvariantCulture));
                    if (ocp_apim_subscription_key == null)
                        throw new System.ArgumentNullException("Ocp-Apim-Subscription-Key");
                    request_.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", ConvertToString(ocp_apim_subscription_key, System.Globalization.CultureInfo.InvariantCulture));
                    if (authorization == null)
                        throw new System.ArgumentNullException("authorization");
                    request_.Headers.TryAddWithoutValidation("authorization", ConvertToString(authorization, System.Globalization.CultureInfo.InvariantCulture));
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ContactInAccount>(response_, headers_).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return objectResponse_.Object;
                        }
                        else
                        if (status_ == 400)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("Missing or invalid service.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 401)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("No authentication credentials provided or authentication failed.", status_, responseText_, headers_, null);
                        }
                        else
                        if (status_ == 500)
                        {
                            string responseText_ = (response_.Content == null) ? string.Empty : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("An internal error occurred.", status_, responseText_, headers_, null);
                        }
                        else
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new ApiException("The HTTP status code of the response was not expected (" + status_ + ").", status_, responseData_, headers_, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgAddressTypeDetails
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("primary", Required = Newtonsoft.Json.Required.Always)]
        public bool Primary { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class AddressTypeRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgAddressType
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Operation
    {
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public object Value { get; set; } = new object();

        [Newtonsoft.Json.JsonProperty("path", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Path { get; set; }

        [Newtonsoft.Json.JsonProperty("op", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Op { get; set; }

        [Newtonsoft.Json.JsonProperty("from", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string From { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class AudioRecording
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        public long Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("audio_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Audio_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("file_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string File_name { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgCarrier
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("email_domain", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Email_domain { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactLocation
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Contact_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("addresses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Address> Addresses { get; set; } = new System.Collections.ObjectModel.Collection<Address>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Address
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("second_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Second_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string City { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string State { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("zip_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Zip_code { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactLocationRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Contact_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("addresses_to_remove", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Addresses_to_remove { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("addresses_to_add_update", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<AddressModel> Addresses_to_add_update { get; set; } = new System.Collections.ObjectModel.Collection<AddressModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("intersection_addresses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<AddressModel> Intersection_addresses { get; set; } = new System.Collections.ObjectModel.Collection<AddressModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class AddressModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("second_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Second_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string City { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string State { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("zip_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Zip_code { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Contact
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("employee_id", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Employee_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("full_name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Full_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("middle_name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Middle_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("last_name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Last_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_zone", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Time_zone { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("pin", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Pin { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("division", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Division { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("company", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Company { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_fields", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactCustomField> Custom_fields { get; set; } = new System.Collections.ObjectModel.Collection<ContactCustomField>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactPoint> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<ContactPoint>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("addresses", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactAddress> Addresses { get; set; } = new System.Collections.ObjectModel.Collection<ContactAddress>();

        ///// <summary>&lt;table&gt;
        /////   &lt;tr&gt;
        /////     &lt;th&gt;&lt;/th&gt;
        /////     &lt;th&gt; CR &lt;/th&gt;
        /////     &lt;th&gt; MIR3 &lt;/th&gt;
        /////     &lt;th&gt; OCN &lt;/th&gt;
        /////     &lt;th&gt; SWN &lt;/th&gt;
        /////   &lt;/tr&gt;
        /////   &lt;tr&gt;
        /////     &lt;td&gt;Supported&lt;/td&gt;
        /////     &lt;td&gt;-&lt;/td&gt;
        /////     &lt;td&gt;x&lt;/td&gt;
        /////     &lt;td&gt;-&lt;/td&gt;
        /////     &lt;td&gt;x&lt;/td&gt;
        /////   &lt;/tr&gt;
        ///// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("login", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public ContactLogin Login { get; set; } = new ContactLogin();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactCustomField
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_field_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Custom_field_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_field_value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Custom_field_value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactPoint
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country_code", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_order", Required = Newtonsoft.Json.Required.Default)]
        public int Cascade_order { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("extension", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Extension { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("carrier", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Carrier { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactAddress
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("facility_location", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Facility_location { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_address", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("second_address", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Second_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("building", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Building { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("floor", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Floor { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string City { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string State { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("zip_code", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Zip_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("province", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Province { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactLogin
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Username { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Password { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("quick_send_code", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Quick_send_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("access_group_list", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Access_group_list { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;140&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;24&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("employee_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Employee_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("full_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Full_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;50&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;30&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("middle_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Middle_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;50&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;30&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("last_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Last_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_zone", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Time_zone { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;4-8&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;24&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        //[Newtonsoft.Json.JsonProperty("pin", Required = Newtonsoft.Json.Required.AllowNull)]
        //[System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        //public string Pin { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("division", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Division { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("company", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Company { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_fields", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<CustomFieldModel> Custom_fields { get; set; } = new System.Collections.ObjectModel.Collection<CustomFieldModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactPointModel> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<ContactPointModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("addresses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactAddressModel> Addresses { get; set; } = new System.Collections.ObjectModel.Collection<ContactAddressModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        //[Newtonsoft.Json.JsonProperty("login", Required = Newtonsoft.Json.Required.Always)]
        //[System.ComponentModel.DataAnnotations.Required]
        //public ContactLoginModel Login { get; set; } = new ContactLoginModel();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CustomFieldModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;40&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_field_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Custom_field_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;80&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_field_value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Custom_field_value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactPointModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;Email, Phone, Text&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Email, Phone, Text&lt;/td&gt;
        ///     &lt;td&gt;Email, Phone, Text&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;20&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;HomePhone, WorkPhone, CellPhone, OtherPhone, HomeEmail, OtherEmail, TextNumber&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (if type = Phone)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_order", Required = Newtonsoft.Json.Required.Always)]
        public int Cascade_order { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("extension", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Extension { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (if type = SMS)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("carrier", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Carrier { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactAddressModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address_type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("facility_location", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Facility_location { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;100&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("second_address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Second_address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("building", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Building { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("floor", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Floor { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;50&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;30&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string City { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;2&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;3&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string State { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;6&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("zip_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Zip_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("province", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Province { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactLoginModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;30&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Username { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;7-15&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Password { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("quick_send_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Quick_send_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("access_group_list", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Access_group_list { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Enabled, Disabled&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ValidationResponse
    {
        [Newtonsoft.Json.JsonProperty("errors", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ValidationErrorDetail> Errors { get; set; } = new System.Collections.ObjectModel.Collection<ValidationErrorDetail>();

        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Message { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ValidationErrorDetail
    {
        [Newtonsoft.Json.JsonProperty("resource", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Resource { get; set; }

        [Newtonsoft.Json.JsonProperty("field", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Field { get; set; }

        [Newtonsoft.Json.JsonProperty("code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Code { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class GroupMemberGroupModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Membership
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CascadeSchedule
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("profile_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Profile_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("activation_date", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset Activation_date { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CascadeScheduleRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("profile_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Profile_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("activation_date", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Activation_date { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgContactCascadeProfile
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("profile_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Profile_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_profiles", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgContactCascadeProfileDetails> Cascade_profiles { get; set; } = new System.Collections.ObjectModel.Collection<SwgContactCascadeProfileDetails>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgContactCascadeProfileDetails
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgContactCascadeProfileContactPoint> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<SwgContactCascadeProfileContactPoint>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_usage", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public SwgContactCascadeProfileTimeUsage Time_usage { get; set; } = new SwgContactCascadeProfileTimeUsage();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgContactCascadeProfileContactPoint
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("rank", Required = Newtonsoft.Json.Required.Always)]
        public int Rank { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgContactCascadeProfileTimeUsage
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("days_of_week", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<int> Days_of_week { get; set; } = new System.Collections.ObjectModel.Collection<int>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("start_time", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Start_time { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("end_time", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string End_time { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CascadeProfileRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;100&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("profile_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(100, MinimumLength = 3)]
        public string Profile_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_profiles", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwnContactCascadeProfileDetailsModel> Cascade_profiles { get; set; } = new System.Collections.ObjectModel.Collection<SwnContactCascadeProfileDetailsModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("activation_dates", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Activation_dates { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwnContactCascadeProfileDetailsModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;3-30&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.StringLength(30, MinimumLength = 3)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwnContactCascadeProfileContactPointModel> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<SwnContactCascadeProfileContactPointModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_usage", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public SwnContactCascadeProfileTimeUsageModel Time_usage { get; set; } = new SwnContactCascadeProfileTimeUsageModel();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwnContactCascadeProfileContactPointModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("rank", Required = Newtonsoft.Json.Required.Always)]
        public int Rank { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwnContactCascadeProfileTimeUsageModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;1-6&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("days_of_week", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<int> Days_of_week { get; set; } = new System.Collections.ObjectModel.Collection<int>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Format&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;HH:mm:ss&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("start_time", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Start_time { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Format&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;HH:mm:ss&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("end_time", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string End_time { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactInAccount
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("is_contact_in_account", Required = Newtonsoft.Json.Required.Always)]
        public bool Is_contact_in_account { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgCustomField
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("min_instances_count", Required = Newtonsoft.Json.Required.Always)]
        public int Min_instances_count { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("max_instances_count", Required = Newtonsoft.Json.Required.Always)]
        public int Max_instances_count { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("validation_reg_ex_pattern", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Validation_reg_ex_pattern { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CustomFieldRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Name { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class GlobalCascadeStatusRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("enabled", Required = Newtonsoft.Json.Required.Always)]
        public bool Enabled { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class GlobalCascadeStatus
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("enabled", Required = Newtonsoft.Json.Required.Always)]
        public bool Enabled { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroup
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_members", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Total_members { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactGroupRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("number", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Number { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Standard, Admin&lt;/td&gt;
        ///     &lt;td&gt;Static, Dynamic&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("members", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public GroupMemberModel Members { get; set; } = new GroupMemberModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If type = Dynamic)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("dynamic_group_criteria", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public DynamicGroupModel Dynamic_group_criteria { get; set; } = new DynamicGroupModel();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class GroupMemberModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DynamicGroupModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;All, Any&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("match_filters", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Match_filters { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;AnyGroup, AllGroups&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("select_recipients", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Select_recipients { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("filters", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<DynamicGroupFilterModel> Filters { get; set; } = new System.Collections.ObjectModel.Collection<DynamicGroupFilterModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DynamicGroupFilterModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("filter_conditions", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<DynamicGroupFilterConditionModel> Filter_conditions { get; set; } = new System.Collections.ObjectModel.Collection<DynamicGroupFilterConditionModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;All, Any&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("match_conditions", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Match_conditions { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DynamicGroupFilterConditionModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("custom_field", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Custom_field { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Fixed, Custom&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("field_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Field_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;NotSet, LastName, FirstName, UniqueID, TimeZone, Address1, Address2, City, StateOrProvince, ZipOrPostalCode, Country&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("fixed_field", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Fixed_field { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Contains, DoesNotContain, Is, IsNot, StartsWith, EndsWith&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("operator", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Operator { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Value { get; set; }


    }
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroupResponseDetails
    {
        // <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgGroupDetails> Groups { get; set; } = new System.Collections.ObjectModel.Collection<SwgGroupDetails>();
        // <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public string Message { get; set; }
    }
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroupDetails
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("members", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public SwgGroupMember Members { get; set; } = new SwgGroupMember();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroupMember
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Default)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroupMemberResponse
    {
        public List<string> contacts { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class LoginRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("username", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Username { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("password", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Password { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class TokenInformation
    {
        [Newtonsoft.Json.JsonProperty("token", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Token { get; set; }

        [Newtonsoft.Json.JsonProperty("expires", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Expires { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageStatistic
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_recipients", Required = Newtonsoft.Json.Required.Always)]
        public int Total_recipients { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_devices_contacted", Required = Newtonsoft.Json.Required.Always)]
        public int Total_devices_contacted { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_phone_numbers", Required = Newtonsoft.Json.Required.Always)]
        public int Total_phone_numbers { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_emails", Required = Newtonsoft.Json.Required.Always)]
        public int Total_emails { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_smses", Required = Newtonsoft.Json.Required.Always)]
        public int Total_smses { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_tdds", Required = Newtonsoft.Json.Required.Always)]
        public int Total_tdds { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_dispossition", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public DispositionModel Total_dispossition { get; set; } = new DispositionModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("billable_minutes", Required = Newtonsoft.Json.Required.Always)]
        public int Billable_minutes { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("turbo", Required = Newtonsoft.Json.Required.Always)]
        public bool Turbo { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("tweet", Required = Newtonsoft.Json.Required.Always)]
        public bool Tweet { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("facebook", Required = Newtonsoft.Json.Required.Always)]
        public bool Facebook { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("mobile", Required = Newtonsoft.Json.Required.Always)]
        public bool Mobile { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("expiration_minutes", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Expiration_minutes { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("category", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Category { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("processed_recipients", Required = Newtonsoft.Json.Required.Always)]
        public int Processed_recipients { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_response", Required = Newtonsoft.Json.Required.Always)]
        public int Total_response { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_contacted", Required = Newtonsoft.Json.Required.Always)]
        public int Total_contacted { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("phone_seconds", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Phone_seconds { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_delivered_attempt", Required = Newtonsoft.Json.Required.Always)]
        public int Total_delivered_attempt { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_remaining", Required = Newtonsoft.Json.Required.Always)]
        public int Total_remaining { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_undelivered", Required = Newtonsoft.Json.Required.Always)]
        public int Total_undelivered { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("pct_completion", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Pct_completion { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("total_credits", Required = Newtonsoft.Json.Required.Always)]
        public float Total_credits { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DispositionModel
    {
        [Newtonsoft.Json.JsonProperty("da", Required = Newtonsoft.Json.Required.Always)]
        public int Da { get; set; }

        [Newtonsoft.Json.JsonProperty("dl", Required = Newtonsoft.Json.Required.Always)]
        public int Dl { get; set; }

        [Newtonsoft.Json.JsonProperty("oi", Required = Newtonsoft.Json.Required.Always)]
        public int Oi { get; set; }

        [Newtonsoft.Json.JsonProperty("to", Required = Newtonsoft.Json.Required.Always)]
        public int To { get; set; }

        [Newtonsoft.Json.JsonProperty("by", Required = Newtonsoft.Json.Required.Always)]
        public int By { get; set; }

        [Newtonsoft.Json.JsonProperty("nr", Required = Newtonsoft.Json.Required.Always)]
        public int Nr { get; set; }

        [Newtonsoft.Json.JsonProperty("fx", Required = Newtonsoft.Json.Required.Always)]
        public int Fx { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class TextMessageRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;0-32&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("sender_info", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public SenderInformationModel Sender_info { get; set; } = new SenderInformationModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x (If no dynamic recipient supplied)&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x (If no dynamic recipient supplied)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageRecipientModel Recipients { get; set; } = new MessageRecipientModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If no recipient supplied)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If no recipient supplied)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("dynamic_recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<DynamicRecipientModel> Dynamic_recipients { get; set; } = new System.Collections.ObjectModel.Collection<DynamicRecipientModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Broadcast, FirstResponse, Callout, BulletinBoard&lt;/td&gt;
        ///     &lt;td&gt;None, Informational, Urgent, Quota_Call, Sequenced_Quota_Call_ByMember, Sequenced_Quota_Call_ByMember_and_Phone, Sequenced_Call_ByPhone&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Message_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Message_name { get; set; }

        /// <summary>Provided for each user account
        /// &lt;table&gt;&lt;tr&gt;&lt;th&gt;&lt;/th&gt;&lt;th&gt; CR &lt;/th&gt;&lt;th&gt; MIR3 &lt;/th&gt;&lt;th&gt; OCN &lt;/th&gt;&lt;th&gt; SWN &lt;/th&gt;&lt;/tr&gt;&lt;tr&gt;&lt;td&gt;Supported&lt;/td&gt;&lt;td&gt;x&lt;/td&gt;&lt;td&gt;-&lt;/td&gt;&lt;td&gt;-&lt;/td&gt;&lt;td&gt;-&lt;/td&gt;&lt;/tr&gt;&lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("launch_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Launch_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;60-7257600&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.Always)]
        public int Duration { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If message_type = Callout)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("callout_success_total", Required = Newtonsoft.Json.Required.Always)]
        public int Callout_success_total { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("callout_window", Required = Newtonsoft.Json.Required.Always)]
        public int Callout_window { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_all", Required = Newtonsoft.Json.Required.Always)]
        public bool Contact_all { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("conference_info", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageConferenceInfoModel Conference_info { get; set; } = new MessageConferenceInfoModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascading", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageCascadingModel Cascading { get; set; } = new MessageCascadingModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("circular_shapes", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageCircularShapeModel> Circular_shapes { get; set; } = new System.Collections.ObjectModel.Collection<MessageCircularShapeModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("require_pin", Required = Newtonsoft.Json.Required.Always)]
        public bool Require_pin { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("default_language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Default_language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_content", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageContentModel> Message_content { get; set; } = new System.Collections.ObjectModel.Collection<MessageContentModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("emergency", Required = Newtonsoft.Json.Required.Always)]
        public bool Emergency { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("send_message_schedule", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageScheduleModel Send_message_schedule { get; set; } = new MessageScheduleModel();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response_options", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ScenarioMsgResponseOptionModel> Response_options { get; set; } = new System.Collections.ObjectModel.Collection<ScenarioMsgResponseOptionModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SenderInformationModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x (if device_type = Email)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("email", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Email { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;5-60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("phone", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Phone { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageRecipientModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("oncall_schedules", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Oncall_schedules { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("text_labels_filter", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Text_labels_filter { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("voice_labels_filter", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Voice_labels_filter { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DynamicRecipientModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("last_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Last_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("employee_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Employee_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("middle_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Middle_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("pin", Required = Newtonsoft.Json.Required.Always)]
        public int Pin { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<DynamicContactPointModel> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<DynamicContactPointModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageConferenceInfoModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("use_SWN_conference", Required = Newtonsoft.Json.Required.Always)]
        public bool Use_SWN_conference { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("conference_number", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Conference_number { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("conference_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Conference_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("include_conference_number", Required = Newtonsoft.Json.Required.Always)]
        public bool Include_conference_number { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("include_conference_id", Required = Newtonsoft.Json.Required.Always)]
        public bool Include_conference_id { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageCascadingModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_voice", Required = Newtonsoft.Json.Required.Always)]
        public bool Cascade_voice { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("stop_on_voice_email", Required = Newtonsoft.Json.Required.Always)]
        public bool Stop_on_voice_email { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageCircularShapeModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("lat", Required = Newtonsoft.Json.Required.Always)]
        public double Lat { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("long", Required = Newtonsoft.Json.Required.Always)]
        public double Long { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("radius", Required = Newtonsoft.Json.Required.Always)]
        public double Radius { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("unit", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Unit { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("action", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Action { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("geo_fence_options", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageGeoFenceOptionModel Geo_fence_options { get; set; } = new MessageGeoFenceOptionModel();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageContentModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("content_id", Required = Newtonsoft.Json.Required.Always)]
        public int Content_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;Email, SMS, Phone&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Email, SMS, Phone&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("device_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Device_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x (If device_type=Email)&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;3-500&lt;/td&gt;
        ///     &lt;td&gt;200&lt;/td&gt;
        ///     &lt;td&gt;1-60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("subject", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Subject { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;3-50000&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;1-1000&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("body", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Body { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("alt_pin_body", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Alt_pin_body { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("voice_recording_intro", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Voice_recording_intro { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("voice_recording_message", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Voice_recording_message { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If device_type = Phone)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;None, TextToSpeech, AudioLibrary, UploadRecording&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("phone_message_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Phone_message_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If phone_message_type = TextToSpeech or AudioLibrary)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("origination_number", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Origination_number { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If phone_message_type = TextToSpeech or AudioLibrary)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;USCanadaOnly, InternationalWithPlus, CountryCode&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("phone_number_format", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Phone_number_format { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o (If phone_message_type = TextToSpeech)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("voice", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Voice { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o (If phone_message_type = AudioLibrary)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Live_And_Machine = 84, Live_Only = 76, Live_Only_Until_500pm = 53, Live_Only_Until_530pm = 65, Live_Only_Until_600pm = 54, Live_Only_Until_630pm = 66&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("answer_machine_id", Required = Newtonsoft.Json.Required.Always)]
        public int Answer_machine_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If phone_message_type = AudioLibrary)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("audio_libary_id", Required = Newtonsoft.Json.Required.Always)]
        public int Audio_libary_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If phone_message_type = UploadRecording)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("wavfile_content", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Wavfile_content { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If phone_message_type = UploadRecording)&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("group_intro", Required = Newtonsoft.Json.Required.Always)]
        public bool Group_intro { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_defined", Required = Newtonsoft.Json.Required.Always)]
        public bool Message_defined { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageScheduleModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;yyyy-mm-dd&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("start_date", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public System.DateTimeOffset Start_date { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("start_hour", Required = Newtonsoft.Json.Required.Always)]
        public int Start_hour { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("end_hour", Required = Newtonsoft.Json.Required.Always)]
        public int End_hour { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ScenarioMsgResponseOptionModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Must pair and match with the content_id&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response_id", Required = Newtonsoft.Json.Required.Always)]
        public int Response_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;0-5&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageOptionModel> Response { get; set; } = new System.Collections.ObjectModel.Collection<MessageOptionModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("success", Required = Newtonsoft.Json.Required.Always)]
        public bool Success { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class DynamicContactPointModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Email, Text, Phone, SMS, Fax, BlackBerry, ExpressVoice, Messenger&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If type = phone)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country_code { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;6-60 (If type = Email)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If type = Text)&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;5-60&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("carrier", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Carrier { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("extension", Required = Newtonsoft.Json.Required.Always)]
        public int Extension { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade_order", Required = Newtonsoft.Json.Required.Always)]
        public int Cascade_order { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageGeoFenceOptionModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("use_shape_as_fence", Required = Newtonsoft.Json.Required.Always)]
        public bool Use_shape_as_fence { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("request_user_location", Required = Newtonsoft.Json.Required.Always)]
        public bool Request_user_location { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("prompt_user_opt_int", Required = Newtonsoft.Json.Required.Always)]
        public bool Prompt_user_opt_int { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("opt_in_prompt_message", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Opt_in_prompt_message { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("alternate_message", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Alternate_message { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageOptionModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Length&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;1-100&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("text", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Values&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;Set as Default if missed&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("device_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Device_type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class TextMessage
    {
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageHistorySummary
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Message_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("subject", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Subject { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("body", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Body { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_sent", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Time_sent { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("sender", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Sender { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageHistoryModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageContactHistoryModel> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<MessageContactHistoryModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageContactHistoryModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("last_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Last_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("middle_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Middle_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("employee_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Employee_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points_statuses", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<ContactPointStatusModel> Contact_points_statuses { get; set; } = new System.Collections.ObjectModel.Collection<ContactPointStatusModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactPointStatusModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("responded_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Responded_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status_details", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status_details { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_responded", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Time_responded { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("status_timestamp", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Status_timestamp { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("text_responded", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text_responded { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class LaunchScenarioRequest
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("additional_text", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Additional_text { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageRecipientModel> Recipients { get; set; } = new System.Collections.ObjectModel.Collection<MessageRecipientModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("dynamic_recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<DynamicRecipientModel> Dynamic_recipients { get; set; } = new System.Collections.ObjectModel.Collection<DynamicRecipientModel>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("placeholder_values", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<PlaceHolderValueModel> Placeholder_values { get; set; } = new System.Collections.ObjectModel.Collection<PlaceHolderValueModel>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PlaceHolderValueModel
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageScenario
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageScenarioDetails
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public MessageRecipients Recipients { get; set; } = new MessageRecipients();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Message_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("duration", Required = Newtonsoft.Json.Required.Always)]
        public int Duration { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("callout_success_total", Required = Newtonsoft.Json.Required.Always)]
        public int Callout_success_total { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("callout_window", Required = Newtonsoft.Json.Required.Always)]
        public int Callout_window { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_all", Required = Newtonsoft.Json.Required.Always)]
        public bool Contact_all { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("default_language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Default_language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("default_message_body", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Default_message_body { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("message_contents", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageContent> Message_contents { get; set; } = new System.Collections.ObjectModel.Collection<MessageContent>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("placeholders", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<Placeholder> Placeholders { get; set; } = new System.Collections.ObjectModel.Collection<Placeholder>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response_options", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageScenarioResponseOption> Response_options { get; set; } = new System.Collections.ObjectModel.Collection<MessageScenarioResponseOption>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageRecipients
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("oncall_schedules", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Oncall_schedules { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageContent
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("content_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Content_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("subject", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Subject { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("description", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("body", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Body { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("device_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Device_type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Placeholder
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("placeholder_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Placeholder_type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("optional", Required = Newtonsoft.Json.Required.Always)]
        public bool Optional { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("default_value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Default_value { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("options", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<PlaceholderOption> Options { get; set; } = new System.Collections.ObjectModel.Collection<PlaceholderOption>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageScenarioResponseOption
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response_id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Response_id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("response", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<MessageScenarioOption> Response { get; set; } = new System.Collections.ObjectModel.Collection<MessageScenarioOption>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("success", Required = Newtonsoft.Json.Required.Always)]
        public bool Success { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("cascade", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Cascade { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class PlaceholderOption
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("label", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Label { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MessageScenarioOption
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("language", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Language { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("text", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Text { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("device_type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Device_type { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgTimezone
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        public int Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("time_zone", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Time_zone { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("sequence", Required = Newtonsoft.Json.Required.Always)]
        public int Sequence { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgScenario
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("additional_text", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Additional_text { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgScenarioRecipient> Recipients { get; set; } = new System.Collections.ObjectModel.Collection<SwgScenarioRecipient>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("dynamic_recipients", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgScenarioDynamicRecipient> Dynamic_recipients { get; set; } = new System.Collections.ObjectModel.Collection<SwgScenarioDynamicRecipient>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("placeholder_values", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgScenarioPlaceholderValue> Placeholder_values { get; set; } = new System.Collections.ObjectModel.Collection<SwgScenarioPlaceholderValue>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgScenarioRecipient
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("on_call_schedules", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> On_call_schedules { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgScenarioDynamicRecipient
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("id", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Id { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("first_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string First_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("middle_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Middle_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("last_name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Last_name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contact_points", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<SwgScenarioDynamicContactPoint> Contact_points { get; set; } = new System.Collections.ObjectModel.Collection<SwgScenarioDynamicContactPoint>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgScenarioPlaceholderValue
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Value { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgScenarioDynamicContactPoint
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Type { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Name { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("address", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Address { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x (If type = Text)&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("carrier", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Carrier { get; set; }

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("country_code", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required(AllowEmptyStrings = true)]
        public string Country_code { get; set; }


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class SwgGroupUpdateMember
    {
        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;x&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("contacts", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Contacts { get; set; } = new System.Collections.ObjectModel.Collection<string>();

        /// <summary>&lt;table&gt;
        ///   &lt;tr&gt;
        ///     &lt;th&gt;&lt;/th&gt;
        ///     &lt;th&gt; CR &lt;/th&gt;
        ///     &lt;th&gt; MIR3 &lt;/th&gt;
        ///     &lt;th&gt; OCN &lt;/th&gt;
        ///     &lt;th&gt; SWN &lt;/th&gt;
        ///   &lt;/tr&gt;
        ///   &lt;tr&gt;
        ///     &lt;td&gt;Supported&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;o&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///     &lt;td&gt;-&lt;/td&gt;
        ///   &lt;/tr&gt;
        /// &lt;/table&gt;</summary>
        [Newtonsoft.Json.JsonProperty("groups", Required = Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DataAnnotations.Required]
        public System.Collections.Generic.ICollection<string> Groups { get; set; } = new System.Collections.ObjectModel.Collection<string>();


    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class OperationArray : System.Collections.ObjectModel.Collection<Operation>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactLocationRequestArray : System.Collections.ObjectModel.Collection<ContactLocationRequest>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class MembershipArray : System.Collections.ObjectModel.Collection<Membership>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class CascadeScheduleRequestArray : System.Collections.ObjectModel.Collection<CascadeScheduleRequest>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ContactsIdCascadeprofilesGet200ApplicationJsonResponse : System.Collections.ObjectModel.Collection<string>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.2.1.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class VoiceCascadeOrders : System.Collections.ObjectModel.Collection<string>
    {

    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.8.2.0 (NJsonSchema v10.2.1.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class ApiException : System.Exception
    {
        public int StatusCode { get; private set; }

        public string Response { get; private set; }

        public System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Exception innerException)
            : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        public override string ToString()
        {
            return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "13.8.2.0 (NJsonSchema v10.2.1.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class ApiException<TResult> : ApiException
    {
        public TResult Result { get; private set; }

        public ApiException(string message, int statusCode, string response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result, System.Exception innerException)
            : base(message, statusCode, response, headers, innerException)
        {
            Result = result;
        }
    }

}

#pragma warning restore 1591
#pragma warning restore 1573
#pragma warning restore 472
#pragma warning restore 114
#pragma warning restore 108