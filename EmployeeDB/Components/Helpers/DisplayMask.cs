using System.Text.RegularExpressions;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Server-side counterparts to the JS phoneMask / ssnMask functions in
    /// Scripts/empdb-edit.js. The data layer stores raw digits; views call
    /// these to render the visible (999) 999-9999 / 999-99-9999 format.
    /// </summary>
    public static class DisplayMask
    {
        private static readonly Regex _nonDigit = new Regex(@"\D", RegexOptions.Compiled);

        /// <summary>Format a phone number string as <c>(999) 999-9999</c>.
        /// Strips any existing formatting first, so legacy mixed-format
        /// data renders the same as freshly-saved digits-only data.</summary>
        public static string Phone(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var digits = _nonDigit.Replace(value, string.Empty);
            // Take only the first 10 digits — anything past that is malformed
            // and we don't want to silently swallow it into the display string.
            if (digits.Length > 10) digits = digits.Substring(0, 10);
            switch (digits.Length)
            {
                case 0: return string.Empty;
                case 1:
                case 2:
                case 3:
                    return "(" + digits + (digits.Length == 3 ? ") " : string.Empty);
                case 4:
                case 5:
                case 6:
                    return "(" + digits.Substring(0, 3) + ") " + digits.Substring(3);
                default:
                    return "(" + digits.Substring(0, 3) + ") " + digits.Substring(3, 3) + "-" + digits.Substring(6);
            }
        }

        /// <summary>Format a phone number with an optional extension suffix —
        /// produces e.g. <c>(941) 555-1234 x100</c>. Pass an empty extension
        /// and you just get the masked phone number.</summary>
        public static string PhoneWithExtension(string number, string extension)
        {
            var formatted = Phone(number);
            if (string.IsNullOrEmpty(formatted)) return formatted;
            if (string.IsNullOrEmpty(extension)) return formatted;
            return formatted + " x" + extension;
        }

        /// <summary>Format an SSN as <c>999-99-9999</c>. Use sparingly —
        /// most lists should use the privacy-masked <c>***-**-NNNN</c> form
        /// instead (see DetailsList.MaskSsn).</summary>
        public static string Ssn(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var digits = _nonDigit.Replace(value, string.Empty);
            if (digits.Length > 9) digits = digits.Substring(0, 9);
            switch (digits.Length)
            {
                case 0: return string.Empty;
                case 1:
                case 2:
                case 3:
                    return digits;
                case 4:
                case 5:
                    return digits.Substring(0, 3) + "-" + digits.Substring(3);
                default:
                    return digits.Substring(0, 3) + "-" + digits.Substring(3, 2) + "-" + digits.Substring(5);
            }
        }
    }
}
