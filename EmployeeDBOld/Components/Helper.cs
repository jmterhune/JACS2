using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using tjc.Modules.EmployeeDB.Components.Services;

namespace tjc.Modules.EmployeeDB.Components
{
    public static class Helper
    {
        private static Regex digitsOnly = new Regex(@"[^\d]");

        public static string CleanPhone(string phone)
        {
            return digitsOnly.Replace(phone, "");
        }
        public static string CleanDecimal(string input)
        {
            return Regex.Replace(input, "[^.0-9]", "");
        }
        public static TokenInformation CreateSwnToken(string swnServiceIdentifier, string swnSubscriptionKey, LoginRequest loginRequest)
        {
            SwnClient client = new SwnClient(new HttpClient());
            var tokenTask = client.POSTLoginAsync(swnServiceIdentifier, swnSubscriptionKey, loginRequest);
            tokenTask.Wait();
            return tokenTask.Result;

        }
    }
    //Session Handling
    public static class SessionVariables
    {
        private static HttpRequest request
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Request;
            }
        }

        private static HttpResponse response
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Response;
            }
        }

        private static HttpSessionState session
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Session;
            }
        }
        internal static IEnumerable<EmployeeListItem> MissingContacts
        {
            get
            {
                if (session == null) return null;
                if (session["MissingContacts"] == null)
                {
                    return null;
                }
                IEnumerable<EmployeeListItem> missingContacts = (IEnumerable<EmployeeListItem>)session["MissingContacts"];

                return missingContacts;
            }
            set
            {
                if (session == null) return;
                session["MissingContacts"] = value;
            }
        }
        public static TokenInformation SwnToken
        {
            get
            {
                if (session == null) return null;
                if (session["SwnToken"] == null)
                {
                    return null;
                }
                TokenInformation tokenInformation = (TokenInformation)session["SwnToken"];
                if (!string.IsNullOrEmpty(tokenInformation.Expires))
                {
                    DateTime tokenExpireDate = DateTime.Parse(tokenInformation.Expires, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    if (tokenExpireDate <= DateTime.UtcNow.AddMinutes(60))
                    {
                        return null;
                    }

                }
                return tokenInformation;
            }
            set
            {
                if (session == null) return;
                session["SwnToken"] = value;
            }
        }
    }
}