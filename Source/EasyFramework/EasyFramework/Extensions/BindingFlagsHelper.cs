using System.Reflection;

namespace EasyFramework
{
    public static class BindingFlagsHelper
    {
        public static BindingFlags Public()
        {
            return BindingFlags.Public;
        }

        public static BindingFlags NonPublic()
        {
            return BindingFlags.NonPublic;
        }

        public static BindingFlags Instance()
        {
            return BindingFlags.Instance;
        }

        public static BindingFlags Static()
        {
            return BindingFlags.Instance;
        }

        public static BindingFlags PublicInstance()
        {
            return Public() | Instance();
        }

        public static BindingFlags PublicStatic()
        {
            return Public() | Static();
        }

        public static BindingFlags NonPublicInstance()
        {
            return NonPublic() | Instance();
        }

        public static BindingFlags NonPublicStatic()
        {
            return NonPublic() | Static();
        }

        public static BindingFlags AllInstance()
        {
            return Public() | NonPublicInstance();
        }

        public static BindingFlags AllStatic()
        {
            return Public() | NonPublicStatic();
        }

        public static BindingFlags All()
        {
            return AllInstance() | AllStatic();
        }

        public static BindingFlags AllPublic()
        {
            return PublicInstance() | PublicStatic();
        }

        public static BindingFlags AllNonPublic()
        {
            return NonPublicInstance() | NonPublicStatic();
        }
    }
}
