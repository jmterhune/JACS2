using System;
using System.ComponentModel;
using System.Reflection;

namespace tjc.Modules.jacs.Components
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the Description attribute value for an enum value.
        /// Returns the enum name if no description is found.
        /// </summary>
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field == null) return value.ToString();

            DescriptionAttribute attribute =
                (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}