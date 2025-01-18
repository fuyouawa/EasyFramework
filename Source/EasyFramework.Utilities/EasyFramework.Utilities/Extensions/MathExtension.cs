using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class MathExtension
    {
        public static float Round(this float value, int decimals)
        {
            return (float)System.Math.Round(value, decimals);
        }
        public static float Round(this float value)
        {
            return (float)System.Math.Round(value);
        }
    }
}
