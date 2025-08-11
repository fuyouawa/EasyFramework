using System;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class MathExtensions
    {
        public static float SafeFloor(this float value, float? epsilon = null)
        {
            var rounded = Mathf.Round(value);
            if (value.IsApproximatelyOf(rounded, epsilon))
            {
                return rounded;
            }
            else
            {
                return Mathf.Floor(value);
            }
        }

        public static float SafeFloorMultipleOf(this float a, float multiple, float? epsilon = null)
        {
            if (multiple.IsApproximatelyOf(0f, epsilon))
                throw new ArgumentException("multiple cannot be 0", nameof(multiple));

            var quotient = a / multiple;
            var rounded = Mathf.Round(quotient);

            if (quotient.IsApproximatelyOf(rounded, epsilon))
            {
                return rounded * multiple;
            }
            else
            {
                var floored = Mathf.Floor(quotient);
                return floored * multiple;
            }
        }

        public static float SafeCeil(this float value, float? epsilon = null)
        {
            var rounded = Mathf.Round(value);
            if (value.IsApproximatelyOf(rounded, epsilon))
            {
                return rounded;
            }
            else
            {
                return Mathf.Ceil(value);
            }
        }

        public static int SafeFloorToInt(this float value, float? epsilon = null)
        {
            return (int)value.SafeFloor(epsilon);
        }

        public static int SafeCeilToInt(this float value, float? epsilon = null)
        {
            return (int)value.SafeCeil(epsilon);
        }

        public static bool IsApproximatelyOf(this double a, double b, double? epsilon = null)
        {
            if (epsilon == null)
            {
                if (a > float.MaxValue || a < float.MinValue || b > float.MaxValue || b < float.MinValue)
                {
                    //TODO better double approximately
                    throw new NotImplementedException("a or b is out of float range");
                }
                return Mathf.Approximately((float)a, (float)b);
            }
            else
            {
                return Math.Abs(a - b) < epsilon.Value;
            }
        }

        public static bool IsApproximatelyOf(this float a, float b, float? epsilon = null)
        {
            if (epsilon == null)
            {
                return Mathf.Approximately(a, b);
            }
            else
            {
                return Mathf.Abs(a - b) < epsilon.Value;
            }
        }

        public static bool IsApproximatelyOf(this Quaternion a, Quaternion b, float similarityThreshold = 0.99f)
        {
            var dot = Quaternion.Dot(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return dot >= threshold;
        }

        public static bool IsApproximatelyOf(this Vector3 a, Vector3 b, float similarityThreshold = 0.99f)
        {
            var distance = Vector3.Distance(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return distance <= 1 - threshold;
        }

        public static bool IsApproximatelyMultipleOf(this float a, float multiple, float? epsilon = null)
        {
            if (multiple.IsApproximatelyOf(0f, epsilon))
                throw new ArgumentException("multiple cannot be 0", nameof(multiple));

            var quotient = a / multiple;
            return (quotient - Mathf.Round(quotient)).IsApproximatelyOf(0f, epsilon);
        }
    }
}
