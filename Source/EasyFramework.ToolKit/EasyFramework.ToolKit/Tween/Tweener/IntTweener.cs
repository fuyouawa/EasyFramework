using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class IntTweener : AbstractTweener<int>
    {
        protected override float GetDistance(int startValue, int endValue)
        {
            return (endValue - startValue).Abs();
        }

        protected override int GetCurrentValue(float time, float? duration, int startValue, int endValue)
        {
            Assert.True(duration.HasValue);

            var t = MathUtility.Remap(time, 0f, duration.Value, 0f, 1f);
            var res = TweenUtility.EaseValue(EaseMode, t, startValue, endValue);
            return Mathf.RoundToInt(res);
        }
    }
}
