using System.ComponentModel;
using tjc.Modules.HearingLog.Components;
internal static class EnumExtension
{
    public static string ToDescriptionString(this StatusType val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])val
           .GetType()
           .GetField(val.ToString())
           .GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }
}