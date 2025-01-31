using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;
using tjc.Modules.ProSeLog.Components;
namespace tjc.Modules.ProSeLog.Components
{
    [TableName("tjc_gl_counties")]
    internal class County
    {
        public int CountyId { get; set; }
        public string CountyName { get; set; }
    }
}
