/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using System;
using System.Text.RegularExpressions;

namespace tjc.Modules.CourtCalendar.Components
{
    /// <summary>
    /// The accessible color palette offered for calendar categories, and the
    /// WCAG contrast math behind it. Category colors tint day-cell entries that
    /// carry black text, so every offered color must meet WCAG 2.1 AA for normal
    /// text (contrast ratio >= 4.5:1) against black. The palette below is curated
    /// to pass; the same check also guards a value posted to the server.
    /// </summary>
    public static class CategoryPalette
    {
        /// <summary>WCAG AA minimum contrast ratio for normal-size text.</summary>
        public const double AaNormal = 4.5;

        private static readonly Regex HexColor = new Regex("^#[0-9a-fA-F]{6}$", RegexOptions.Compiled);

        /// <summary>
        /// Offered colors, grouped light-then-medium across hues. Each value has
        /// been verified to clear AaNormal against black text.
        /// </summary>
        public static readonly string[] Colors =
        {
            "#d9eafc", "#74c0fc", // blue
            "#c5efe8", "#38d9a9", // teal
            "#d7f0d0", "#69db7c", // green
            "#fff3b0", "#ffd43b", // yellow
            "#ffe0b3", "#ffa94d", // orange
            "#ffd1d1", "#ff8787", // red
            "#f7c9d8", "#f783ac", // pink
            "#e5d4f7", "#da77f2", // purple
            "#e9ecef", "#ced4da", // grey
        };

        /// <summary>Relative luminance of an sRGB hex color per WCAG 2.1.</summary>
        public static double RelativeLuminance(string hex)
        {
            if (!HexColor.IsMatch(hex ?? string.Empty))
                throw new ArgumentException("Expected a #rrggbb hex color.", nameof(hex));
            double r = Channel(Convert.ToInt32(hex.Substring(1, 2), 16));
            double g = Channel(Convert.ToInt32(hex.Substring(3, 2), 16));
            double b = Channel(Convert.ToInt32(hex.Substring(5, 2), 16));
            return 0.2126 * r + 0.7152 * g + 0.0722 * b;
        }

        private static double Channel(int v)
        {
            double c = v / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        /// <summary>Contrast ratio of the color against black (#000000) text.</summary>
        public static double ContrastWithBlack(string hex)
        {
            return (RelativeLuminance(hex) + 0.05) / 0.05;
        }

        /// <summary>True when black text on this color meets WCAG AA for normal text.</summary>
        public static bool PassesAaForBlackText(string hex)
        {
            return HexColor.IsMatch(hex ?? string.Empty) && ContrastWithBlack(hex) >= AaNormal;
        }
    }
}
