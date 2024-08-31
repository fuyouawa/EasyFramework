﻿using System.Collections.Generic;

namespace UniTool.Utilities
{
    public static class CollectionsExtension
    {
        public static bool IsNullOrEmpty<T>(this IList<T> val)
        {
            return val == null || val.Count == 0;
        }

        public static bool IsNotNullOrEmpty<T>(this IList<T> val)
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