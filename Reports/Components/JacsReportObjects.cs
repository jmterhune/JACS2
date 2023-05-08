
using DotNetNuke.ComponentModel.DataAnnotations;
using System.Globalization;
namespace tjc.Modules.Reports.Components
{
    //scope the objects to the ModuleId of a module on a page (or copy of a module on a page)
    internal class JacsJudge
    {
        public string Userid { get; set; }
        public string JudgeName { get; set; }

        public string CourtCode { get; set; }
        [IgnoreColumn]
        public string FormattedJudgeName
        {
            get
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                return textInfo.ToTitleCase(JudgeName.ToLower());
            }
        }
    }
    internal class WeekdayHearing
    {
        public string WeekDayName { get; set; }

        public int NumberOfHearings { get; set; }
    }
}
