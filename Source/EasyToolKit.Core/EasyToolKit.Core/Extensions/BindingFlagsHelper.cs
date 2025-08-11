using System.Reflection;

namespace EasyToolKit.Core
{
    public static class BindingFlagsHelper
    {
        public static readonly BindingFlags Public = BindingFlags.Public;

        public static readonly BindingFlags NonPublic = BindingFlags.NonPublic;

        public static readonly BindingFlags Instance = BindingFlags.Instance;

        public static readonly BindingFlags Static = BindingFlags.Static;

        public static readonly BindingFlags PublicInstance = Public | Instance;

        public static readonly BindingFlags PublicStatic = Public | Static;

        public static readonly BindingFlags NonPublicInstance = NonPublic | Instance;

        public static readonly BindingFlags NonPublicStatic = NonPublic | Static;

        public static readonly BindingFlags AllInstance = Public | NonPublicInstance;

        public static readonly BindingFlags AllStatic = Public | NonPublicStatic;

        public static readonly BindingFlags All = AllInstance | AllStatic;

        public static readonly BindingFlags AllPublic = PublicInstance | PublicStatic;

        public static readonly BindingFlags AllNonPublic = NonPublicInstance | NonPublicStatic;
    }
}
