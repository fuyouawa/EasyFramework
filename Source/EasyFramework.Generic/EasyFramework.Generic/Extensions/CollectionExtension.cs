using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework
{
    public static class CollectionExtension
    {
        public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
        {
            return Array.IndexOf(array, value, startIndex, count);
        }
        
        public static int IndexOf<T>(this T[] array, T value)
        {
            return Array.IndexOf(array, value);
        }
        
        public static int IndexOf<T>(this T[] array, T value, int startIndex)
        {
            return Array.IndexOf(array, value, startIndex);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> val)
        {
            return val == null || !val.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> val)
        {
            return !val.IsNullOrEmpty();
        }

        public static void AddRange<T>(this IList<T> val, IEnumerable<T> enumerable)
        {
            foreach (var e in enumerable)
            {
                val.Add(e);
            }
        }
    }
}
