using UnityEngine;

namespace EasyToolKit.Core
{
    public static class ColorExtensions
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

        public static Color MulR(this Color color, float radio)
        {
            color.r *= radio;
            return color;
        }

        public static Color MulG(this Color color, float radio)
        {
            color.g *= radio;
            return color;
        }

        public static Color MulB(this Color color, float radio)
        {
            color.b *= radio;
            return color;
        }

        public static Color MulA(this Color color, float radio)
        {
            color.a *= radio;
            return color;
        }

        public static Color MulRGB(this Color color, float radio)
        {
            color.r *= radio;
            color.g *= radio;
            color.b *= radio;
            return color;
        }
    }
}
