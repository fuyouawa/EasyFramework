using System;

namespace EasyFramework
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return string.IsNullOrEmpty(val);
        }
        public static bool IsNotNullOrEmpty(this string val)
        {
            return !string.IsNullOrEmpty(val);
        }

        public static string DefaultIfNullOrEmpty(this string val, string defaultValue)
        {
            return IsNullOrEmpty(val) ? defaultValue : val;
        }

        public static bool TrimEquals(this string a, string b)
        {
            return a.Trim().Equals(b.Trim());
        }

        public static bool Contains(this string source, string toCheck, StringComparison comparisonType)
        {
            return source.IndexOf(toCheck, comparisonType) >= 0;
        }
    }
}
