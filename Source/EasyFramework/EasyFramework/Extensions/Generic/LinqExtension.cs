using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework
{
    public static class LinqExtension
    {
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> callback)
        {
            foreach (var elem in enumerator)
            {
                callback(elem);
            }
        }

        public static bool HasDuplicate<T, E>(this IEnumerable<T> enumerator, Func<T, E> selector)
        {
            var set = new HashSet<E>();

            foreach (var e in enumerator)
            {
                if (!set.Add(selector(e)))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasDuplicate<T>(this IEnumerable<T> enumerator)
        {
            return enumerator.HasDuplicate(e => e);
        }
    }
}
