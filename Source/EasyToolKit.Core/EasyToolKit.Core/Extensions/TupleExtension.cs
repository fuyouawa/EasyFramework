using System;

namespace EasyToolKit.Core
{
    public static class TupleExtension
    {
        public static bool HasOneOf<T>(this ValueTuple<T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true;
        }

        public static bool HasOneOf<T>(this ValueTuple<T, T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true ||
                   tuple.Item2?.Equals(val) == true;
        }

        public static bool HasOneOf<T>(this ValueTuple<T, T, T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true ||
                   tuple.Item2?.Equals(val) == true ||
                   tuple.Item3?.Equals(val) == true;
        }

        public static bool HasOneOf<T>(this ValueTuple<T, T, T, T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true ||
                   tuple.Item2?.Equals(val) == true ||
                   tuple.Item3?.Equals(val) == true ||
                   tuple.Item4?.Equals(val) == true;
        }

        public static bool HasOneOf<T>(this ValueTuple<T, T, T, T, T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true ||
                   tuple.Item2?.Equals(val) == true ||
                   tuple.Item3?.Equals(val) == true ||
                   tuple.Item4?.Equals(val) == true ||
                   tuple.Item5?.Equals(val) == true;
        }

        public static bool HasOneOf<T>(this ValueTuple<T, T, T, T, T, T> tuple, T val)
        {
            return tuple.Item1?.Equals(val) == true ||
                   tuple.Item2?.Equals(val) == true ||
                   tuple.Item3?.Equals(val) == true ||
                   tuple.Item4?.Equals(val) == true ||
                   tuple.Item5?.Equals(val) == true ||
                   tuple.Item6?.Equals(val) == true;
        }
    }
}
