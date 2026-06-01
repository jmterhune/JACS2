using System;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Marks a string property whose stored value must be digits only.
    /// <see cref="ModelNormalizer.StripDigitsOnly"/> strips every non-digit
    /// character before the row is INSERTed / UPDATEd.
    /// </summary>
    /// <remarks>
    /// Use case: SSN and phone-number columns. The UI applies visual masks
    /// like <c>(999) 999-9999</c> or <c>999-99-9999</c> for user comfort,
    /// but the database should only ever contain the raw digits so callers
    /// (search, joins, exports, SWN) don't have to deal with mask variation.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DigitsOnlyAttribute : Attribute { }
}
