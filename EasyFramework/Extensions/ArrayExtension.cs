using System;

namespace EasyFramework
{
    public static class ArrayExtension
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
    }
}