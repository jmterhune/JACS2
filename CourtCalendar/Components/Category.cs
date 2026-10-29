/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.ComponentModel.DataAnnotations;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// A record category for a division's calendar entries. Every division is
    /// seeded with four built-ins - default, vacation, no court, holiday - and the
    /// judge/JA may add custom categories and assign each a color. The default
    /// category carries no color and renders as a very light grey.
    /// </summary>
    [TableName("tjc_cc_categories")]
    [PrimaryKey("CategoryId", AutoIncrement = true)]
    public class Category
    {
        public int CategoryId { get; set; }

        public int DivisionId { get; set; }

        public string Name { get; set; }

        /// <summary>Hex color (e.g. "#cfe8ff"); null/empty for the default category.</summary>
        public string Color { get; set; }

        /// <summary>True for the four seeded built-ins (name is fixed; may still be recolored except default).</summary>
        public bool IsSystem { get; set; }

        /// <summary>True for the single "default" category (no color).</summary>
        public bool IsDefault { get; set; }

        public int SortOrder { get; set; }

        public bool Active { get; set; }
    }
}
