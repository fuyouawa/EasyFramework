using UnityEngine;

namespace EasyFramework
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
