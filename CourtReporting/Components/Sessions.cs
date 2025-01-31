using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace tjc.Modules.CourtReporting.Components
{
    public static class Sessions
    {
        private static HttpSessionState session
        {
            get
            {
                if (HttpContext.Current == null) return null;

                return HttpContext.Current.Session;
            }
        }
        public static List<ProceedingInfo> proceedings
        {
            get
            {
                if (session == null) return null;

                if (session["Proceedings"] == null)
                {
                    session["Proceedings"] = new List<ProceedingInfo>();
                }

                return session["Proceedings"] as List<ProceedingInfo>;
            }
            set
            {
                if (session == null) return;

                session["Proceedings"] = value;
            }
        }

        public static RequestInfo request
        {
            get
            {
                if (session == null) return null;

                if (session["Request"] == null)
                {
                    session["Request"] = new RequestInfo();
                }

                return session["Request"] as RequestInfo;
            }
            set
            {
                if (session == null) return;

                session["Request"] = value;
            }

        }
    }
}