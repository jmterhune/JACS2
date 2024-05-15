using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace tjc.Modules.Purchasing.Components
{
    internal static class SessionVariables
    {
        private static HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current is null)
                    return null;
                return HttpContext.Current.Session;
            }
        }

        public static List<SupplyOrderItem> SessionItemList
        {
            get
            {
                if (Session is null)
                    return null;

                if (Session["SupplyOrderItems"] is null)
                {
                    Session["SupplyOrderItems"] = new List<SupplyOrderItem>();
                }

                return Session["SupplyOrderItems"] as List<SupplyOrderItem>;
            }
            set
            {
                if (Session is null)
                    return;
                Session["SupplyOrderItems"] = value;
            }
        }

    }
}