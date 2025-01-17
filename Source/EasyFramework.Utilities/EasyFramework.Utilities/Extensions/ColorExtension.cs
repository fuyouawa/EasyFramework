using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class ColorExtension
    {
        public static Color SetR(this Color color, float r)
        {
            color.r = r;
            return color;
        }
        public static Color SetG(this Color color, float g)
        {
            color.g = g;
            return color;
        }
        public static Color SetB(this Color color, float b)
        {
            color.b = b;
            return color;
        }
        public static Color SetA(this Color color, float a)
        {
            color.a = a;
            return color;
        }
    }
}
