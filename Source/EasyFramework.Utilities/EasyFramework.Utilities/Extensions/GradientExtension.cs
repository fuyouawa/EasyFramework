using UnityEngine;

namespace EasyFramework.Utilities
{
    public static class GradientExtension
    {
        /// <summary>
        /// <para>将给定的time从[0-maxTime]映射到[0-1]</para>
        /// <para>然后传给Gradient.Evaluate</para>
        /// </summary>
        /// <param name="gradient"></param>
        /// <param name="time"></param>
        /// <param name="maxTime"></param>
        /// <returns></returns>
        public static Color EvaluateWithRemap(this Gradient gradient, float time, float maxTime)
        {
            return gradient.Evaluate(MathUtility.Remap(time, 0f, maxTime, 0f, 1f));
        }
    }
}
