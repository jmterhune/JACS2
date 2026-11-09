/*
' Copyright (c) 2026 12th Judicial Circuit Court
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tjc.Modules.CourtRegistry.Components
{
    /// <summary>
    /// An editable email that the registry sends to an attorney. Court administration
    /// edits the subject and body here rather than in code, because the wording is
    /// revised from time to time.
    /// </summary>
    [TableName("tjc_car_email_templates")]
    [PrimaryKey("TemplateId", AutoIncrement = true)]
    public class EmailTemplate
    {
        public int TemplateId { get; set; }

        /// <summary>Stable identifier the code looks the template up by. Never edited in the UI.</summary>
        public string TemplateKey { get; set; }

        public string TemplateName { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsEnabled { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedByUserId { get; set; }
    }

    /// <summary>The keys the code sends by. Adding one here also needs a row in the table.</summary>
    public static class EmailTemplateKeys
    {
        public const string WithdrawalSubmitted = "withdrawal.submitted";
        public const string WithdrawalApproved = "withdrawal.approved";
        public const string WithdrawalDenied = "withdrawal.denied";

        public static readonly string[] All =
        {
            WithdrawalSubmitted, WithdrawalApproved, WithdrawalDenied
        };
    }

    /// <summary>
    /// Replaces [Token] placeholders in a template's subject and body.
    ///
    /// Tokens are matched without regard to case. A token with no value supplied is
    /// replaced with an empty string, so a template that names a token the sending
    /// code does not provide degrades to a blank rather than printing "[Token]" to
    /// an attorney.
    /// </summary>
    public static class EmailTokens
    {
        private static readonly Regex TokenPattern = new Regex(@"\[([A-Za-z0-9_]+)\]", RegexOptions.Compiled);

        /// <summary>
        /// [if:Token] ... [/if] keeps what is between the markers only when that token
        /// has a value, and drops the lot when it does not. It lets a template carry an
        /// optional section - a reason or a note and its heading - without the wording
        /// having to live in code. Blocks are not nested.
        /// </summary>
        private static readonly Regex ConditionPattern =
            new Regex(@"\[if:([A-Za-z0-9_]+)\](.*?)\[/if\]", RegexOptions.Compiled | RegexOptions.Singleline);

        public static string Render(string text, IDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            if (values == null)
            {
                values = new Dictionary<string, string>();
            }

            // Conditional sections first, so a dropped block never leaves its tokens behind.
            text = ConditionPattern.Replace(text, m =>
            {
                string key = m.Groups[1].Value;
                string value;
                bool hasValue = values.TryGetValue(key, out value) && !string.IsNullOrEmpty(value) && value.Trim().Length > 0;
                return hasValue ? m.Groups[2].Value : string.Empty;
            });

            return TokenPattern.Replace(text, m =>
            {
                string key = m.Groups[1].Value;
                string value;
                return values.TryGetValue(key, out value) ? (value ?? string.Empty) : string.Empty;
            });
        }

        /// <summary>A case-insensitive bag, so [firstname] and [FirstName] both resolve.</summary>
        public static IDictionary<string, string> NewBag()
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
