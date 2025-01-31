using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace tjc.Modules.CourtReporting.Components
{
    public static class Helper
    {
        public static string GetEnumDescription(Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DescriptionAttribute>()
                    ?.Description
                ?? value.ToString();
        }
    }
    public class RemotePost
    {
        private readonly System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
        public string Url = "";
        public string Method = "post";
        public string FormName = "form1";
        public void Add(string name, string value)
        {
            Inputs.Add(name, value);
        }
        public void Post()
        {
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.Write(string.Format("", FormName));
            System.Web.HttpContext.Current.Response.Write(string.Format("", FormName, Method, Url));
            for (int i = 0; i < Inputs.Keys.Count; i++)
            {
                System.Web.HttpContext.Current.Response.Write(string.Format("", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
            }
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.Write("");
            System.Web.HttpContext.Current.Response.End();
        }
    }

}