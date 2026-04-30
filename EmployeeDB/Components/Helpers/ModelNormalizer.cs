using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace tjc.Modules.EmployeeDB.Components.Helpers
{
    /// <summary>
    /// Cross-cutting model fix-ups applied right before a row is INSERTed / UPDATEd.
    /// </summary>
    /// <remarks>
    /// Two passes are run by <see cref="Normalize"/>:
    ///   1. Any nullable numeric property with the value <c>0</c> is rewritten to
    ///      <c>null</c> — empty form fields and unselected dropdowns shouldn't
    ///      be persisted as a hard zero. Examples: PhoneCascade, CallOrder.
    ///   2. Any string property tagged with <see cref="DigitsOnlyAttribute"/>
    ///      has every non-digit character stripped — UI masks like
    ///      <c>(999) 999-9999</c> stay client-side; the DB sees raw digits.
    ///      Examples: SocialSecurityNumber, PhoneNumber, PhoneHome / Work / Mobile.
    /// </remarks>
    internal static class ModelNormalizer
    {
        // Reflection lookup is cached per type so the per-row cost is just a
        // dictionary lookup + N PropertyInfo.GetValue calls.
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _nullableCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _digitsCache =
            new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly Regex _nonDigit = new Regex(@"\D", RegexOptions.Compiled);

        /// <summary>Run all normalization passes on the given entity.</summary>
        public static void Normalize(object item)
        {
            ZeroToNull(item);
            StripDigitsOnly(item);
        }

        /// <summary>Convert nullable numeric properties whose value is 0 to null.</summary>
        public static void ZeroToNull(object item)
        {
            if (item == null) return;

            var props = _nullableCache.GetOrAdd(item.GetType(), GetNullableNumericProperties);
            foreach (var prop in props)
            {
                var value = prop.GetValue(item, null);
                if (value == null) continue;
                if (Convert.ToDecimal(value) == 0m)
                    prop.SetValue(item, null, null);
            }
        }

        /// <summary>Strip non-digit characters from string properties tagged
        /// with <see cref="DigitsOnlyAttribute"/>.</summary>
        public static void StripDigitsOnly(object item)
        {
            if (item == null) return;

            var props = _digitsCache.GetOrAdd(item.GetType(), GetDigitsOnlyProperties);
            foreach (var prop in props)
            {
                var value = prop.GetValue(item, null) as string;
                if (string.IsNullOrEmpty(value)) continue;
                var stripped = _nonDigit.Replace(value, string.Empty);
                if (!ReferenceEquals(stripped, value))
                    prop.SetValue(item, stripped, null);
            }
        }

        private static PropertyInfo[] GetNullableNumericProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p =>
                {
                    var pt = p.PropertyType;
                    if (!pt.IsGenericType) return false;
                    if (pt.GetGenericTypeDefinition() != typeof(Nullable<>)) return false;
                    var u = Nullable.GetUnderlyingType(pt);
                    return u == typeof(int)
                        || u == typeof(long)
                        || u == typeof(short)
                        || u == typeof(byte)
                        || u == typeof(decimal)
                        || u == typeof(double)
                        || u == typeof(float);
                })
                .ToArray();
        }

        private static PropertyInfo[] GetDigitsOnlyProperties(Type type)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite)
                .Where(p => p.PropertyType == typeof(string))
                .Where(p => p.GetCustomAttributes(typeof(DigitsOnlyAttribute), true).Any())
                .ToArray();
        }
    }
}
