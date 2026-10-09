using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;

namespace tjc.Modules.EmployeeDB.Components.SWN
{
    /// <summary>
    /// Port of AWS.SWN.API.ClientFactory from
    /// D:\websites\Intranet\App_Code\EmployeeDB\SWNCustomAuthorization.vb.
    ///
    /// Builds the WCF proxy used to talk to the SWN Users service over HTTPS
    /// using UsernameToken authentication. Uses a hand-rolled CustomBinding to
    /// match the exact WS-Security shape SWN requires.
    /// </summary>
    public static class ClientFactory
    {
        public const string Url = "https://api.sendwordnow.com/webservices/v3/Users.svc";

        // Fallback test credentials. Used only when no credentials come in from
        // module settings (Swn_TestUsername / Swn_TestPassword).
        private const string DefaultTestUsername = "TJCCAPI";
        private const string DefaultTestPassword = "12CircuitAPI!";

        /// <summary>
        /// Creates a proxy using the built-in default test credentials. Kept
        /// for callers that don't have settings context.
        /// </summary>
        public static UsersClient CreateSWNOnlineProxy()
        {
            return CreateSWNOnlineProxy(null, null);
        }

        /// <summary>
        /// Creates and returns a configured <see cref="UsersClient"/> using the
        /// supplied credentials. Empty/null falls back to the default test
        /// account so existing call sites keep working.
        /// </summary>
        public static UsersClient CreateSWNOnlineProxy(string username, string password)
        {
            // 768 = Tls, 3072 = Tls12. OR'd together.
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(768 | 3072);

            var binding = new CustomBinding();

            var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
            security.IncludeTimestamp = false;
            security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
            security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

            var encoding = new TextMessageEncodingBindingElement();
            encoding.MessageVersion = MessageVersion.Soap12WSAddressing10;

            var transport = new HttpsTransportBindingElement();
            transport.MaxReceivedMessageSize = 20000000;

            binding.Elements.Add(security);
            binding.Elements.Add(encoding);
            binding.Elements.Add(transport);

            var client = new UsersClient(binding, new EndpointAddress(Url));

            client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
            client.ChannelFactory.Endpoint.Behaviors.Add(new CustomCredentials());
            client.ClientCredentials.UserName.UserName = string.IsNullOrEmpty(username) ? DefaultTestUsername : username;
            client.ClientCredentials.UserName.Password = string.IsNullOrEmpty(password) ? DefaultTestPassword : password;

            return client;
        }
    }
}
