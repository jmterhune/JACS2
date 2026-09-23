using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.API.CustomCredentials from
    /// D:\websites\Intranet\App_Code\EmployeeDB\SWNCustomAuthorization.vb.
    /// </summary>
    public class CustomCredentials : ClientCredentials
    {
        public CustomCredentials()
        {
        }

        protected CustomCredentials(CustomCredentials cc)
            : base(cc)
        {
        }

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new CustomSecurityTokenManager(this);
        }

        protected override ClientCredentials CloneCore()
        {
            return new CustomCredentials(this);
        }
    }

    /// <summary>
    /// Port of AWS.SWN.API.CustomSecurityTokenManager. Returns a
    /// CustomTokenSerializer pinned to WSSecurity11.
    /// </summary>
    public class CustomSecurityTokenManager : ClientCredentialsSecurityTokenManager
    {
        public CustomSecurityTokenManager(CustomCredentials cred)
            : base(cred)
        {
        }

        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
        {
            return new CustomTokenSerializer(SecurityVersion.WSSecurity11);
        }
    }

    /// <summary>
    /// Port of AWS.SWN.API.CustomTokenSerializer. Emits a WS-Security
    /// UsernameToken envelope with a Nonce and Created timestamp; SWN is picky
    /// about this exact XML shape so we hand-roll it.
    /// </summary>
    public class CustomTokenSerializer : WSSecurityTokenSerializer
    {
        public CustomTokenSerializer(SecurityVersion sv)
            : base(sv)
        {
        }

        protected override void WriteTokenCore(System.Xml.XmlWriter writer, SecurityToken token)
        {
            var userToken = token as UserNameSecurityToken;
            const string tokennamespace = "o";
            var created = DateTime.Now;
            var createdStr = created.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
            var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var password = userToken != null ? userToken.Password : string.Empty;
            var userName = userToken != null ? userToken.UserName : string.Empty;

            writer.WriteRaw(string.Format(
                "<{0}:UsernameToken u:Id=\"" + token.Id + "\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
                "<{0}:Username>" + userName + "</{0}:Username>" +
                "<{0}:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">" + password + "</{0}:Password>" +
                "<{0}:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">" + nonce + "</{0}:Nonce>" +
                "<u:Created>" + createdStr + "</u:Created></{0}:UsernameToken>",
                tokennamespace));
        }

        protected string GetSHA1String(string phrase)
        {
            using (var sha1Hasher = new SHA1CryptoServiceProvider())
            {
                var hashedDataBytes = sha1Hasher.ComputeHash(Encoding.UTF32.GetBytes(phrase));
                return Convert.ToBase64String(hashedDataBytes);
            }
        }

        protected string GetBase64(string phrase)
        {
            var byt = Encoding.UTF8.GetBytes(phrase);
            return Convert.ToBase64String(byt);
        }
    }

    /// <summary>
    /// Port of AWS.SWN.API.HttpHeaderMessageInspector. Adds HTTP headers to
    /// outbound SOAP requests.
    /// </summary>
    public class HttpHeaderMessageInspector : IClientMessageInspector
    {
        private readonly Dictionary<string, string> _httpHeaders;

        public HttpHeaderMessageInspector(Dictionary<string, string> httpHeaders)
        {
            _httpHeaders = httpHeaders;
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;

            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;

                if (httpRequestMessage != null)
                {
                    foreach (var httpHeader in _httpHeaders)
                    {
                        httpRequestMessage.Headers[httpHeader.Key] = httpHeader.Value;
                    }
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                foreach (var httpHeader in _httpHeaders)
                {
                    httpRequestMessage.Headers.Add(httpHeader.Key, httpHeader.Value);
                }
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            // Intentionally empty - matches the VB implementation.
        }
    }
}
