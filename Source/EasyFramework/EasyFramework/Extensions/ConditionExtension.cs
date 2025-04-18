using System;

namespace EasyFramework
{
    public static class ConditionExtension
    {
        public static bool Is<T>(this T val, Func<T, bool> cond)
        {
            return cond(val);
        }
    }
}
