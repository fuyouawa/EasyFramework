using UnityEngine;

namespace EasyToolKit.Core
{
    public static class MathExtensions
    {
        public static float Round(this float value, int decimals)
        {
            return (float)System.Math.Round(value, decimals);
        }

        public static float Round(this float value)
        {
            return (float)System.Math.Round(value);
        }

        public static float Abs(this float value)
        {
            return Mathf.Abs(value);
        }

        public static int Abs(this int value)
        {
            return Mathf.Abs(value);
        }

        public static int Sign(this int value)
        {
            return value >= 0 ? 1 : -1;
        }

        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        public static float Sign(this float value)
        {
            return Mathf.Sign(value);
        }

        public static int FloorToInt(this float value)
        {
            return Mathf.FloorToInt(value);
        }

        public static bool Approximately(this float a, float b)
        {
            return Mathf.Approximately(a, b);
        }

        public static bool Approximately(this Quaternion a, Quaternion b, float similarityThreshold = 0.99f)
        {
            var dot = Quaternion.Dot(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return dot >= threshold;
        }

        public static bool Approximately(this Vector3 a, Vector3 b, float similarityThreshold = 0.99f)
        {
            var distance = Vector3.Distance(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return distance <= 1 - threshold;
        }
    }
}
