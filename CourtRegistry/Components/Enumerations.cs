using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
namespace tjc.Modules.CourtRegistry.Components
{
    public static class Enumerations
    {

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
    public enum ApplicationStatus
    {
        [Description("New")]
        New = 0,
        [Description("Approved")]
        Approved = 1,
        [Description("Rejected")]
        Rejected = 2,
        [Description("Archived")]
        Archived = 3,
        [Description("Updated")]
        Updated = 4
    }
}