using UnityEngine;

namespace EasyFramework.Core
{
    public static class AnimationCurveExtension
    {
        /// <summary>
        /// <para>将给定的time从[0-maxTime]映射到[0-1]</para>
        /// <para>然后传给AnimationCurve.Evaluate</para>
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="time"></param>
        /// <param name="maxTime"></param>
        /// <returns></returns>
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime)
        {
            return curve.Evaluate(MathUtility.Remap(time, 0f, maxTime, 0f, 1f));
        }
        
        /// <summary>
        /// <para>将给定的time从[0-maxTime]映射到[0-1]</para>
        /// <para>然后传给AnimationCurve.Evaluate</para>
        /// <para>之后将结果从[0-1]映射到[minValue-maxValue]</para>
        /// <para>注意：AnimationCurve.Evaluate的结果必须在[0-1]区间</para>
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="time"></param>
        /// <param name="maxTime"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns>[minValue-maxValue]区间的数值</returns>
        public static float EvaluateWithRemap(this AnimationCurve curve, float time, float maxTime, float minValue, float maxValue)
        {
            return MathUtility.Remap(curve.EvaluateWithRemap(time, maxTime), 0f, 1f, minValue, maxValue);
        }
    }
}
