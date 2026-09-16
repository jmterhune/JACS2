using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace tjc.Modules.jacs.Components
{
    /// <summary>
    /// A single user-defined-field value pulled from an event's <c>template</c> column.
    /// </summary>
    internal class UdfTemplateEntry
    {
        /// <summary>Id of the defining row in <c>user_defined_fields</c>. Null for legacy rows.</summary>
        public long? FieldId { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string Value { get; set; }
    }

    /// <summary>
    /// Parses the event <c>template</c> column, which stores user-defined-field
    /// values as JSON. Two on-disk formats are understood so existing events keep
    /// rendering after the format change:
    ///
    ///   Current: {"userDefinedFields":[{"fieldId":8,"fieldName":"Interpreter Requested?","fieldType":"yes_no","value":"no"}, ...]}
    ///   Legacy:  {"Interpreter Requested?_|center_|yes_no":"no", ...}
    ///
    /// Legacy rows carry no field id (<see cref="UdfTemplateEntry.FieldId"/> is null);
    /// the name and type are recovered from the composite "name_|align_|type" key.
    /// Returns an empty list for null/blank/malformed input.
    /// </summary>
    internal static class UserDefinedFieldTemplate
    {
        private static readonly string[] LegacyKeySeparator = { "_|" };

        public static List<UdfTemplateEntry> Parse(string templateJson)
        {
            var entries = new List<UdfTemplateEntry>();
            if (string.IsNullOrWhiteSpace(templateJson))
                return entries;

            JToken root;
            try
            {
                root = JToken.Parse(templateJson);
            }
            catch
            {
                return entries;
            }

            var obj = root as JObject;
            if (obj == null)
                return entries;

            // Current format: a "userDefinedFields" array of objects.
            if (obj["userDefinedFields"] is JArray fields)
            {
                foreach (var f in fields.OfType<JObject>())
                {
                    long? fieldId = null;
                    var idToken = f["fieldId"];
                    if (idToken != null && idToken.Type != JTokenType.Null
                        && long.TryParse(idToken.ToString(), out long parsedId))
                    {
                        fieldId = parsedId;
                    }
                    entries.Add(new UdfTemplateEntry
                    {
                        FieldId = fieldId,
                        FieldName = f["fieldName"]?.ToString() ?? string.Empty,
                        FieldType = f["fieldType"]?.ToString() ?? string.Empty,
                        Value = f["value"]?.ToString() ?? string.Empty
                    });
                }
                return entries;
            }

            // Legacy format: a flat object keyed by "name_|align_|type".
            foreach (var prop in obj.Properties())
            {
                var parts = prop.Name.Split(LegacyKeySeparator, StringSplitOptions.None);
                entries.Add(new UdfTemplateEntry
                {
                    FieldId = null,
                    FieldName = parts.Length > 0 ? parts[0] : prop.Name,
                    FieldType = parts.Length > 2 ? parts[2] : string.Empty,
                    Value = prop.Value?.ToString() ?? string.Empty
                });
            }
            return entries;
        }
    }
}
