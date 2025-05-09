using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class FloatTweener : AbstractTweener<float>
    {
        protected override float GetDistance(float startValue, float endValue)
        {
            return (endValue - startValue).Abs();
        }

        protected override float GetCurrentValue(float time, float? duration, float startValue, float endValue)
        {
            Assert.True(duration.HasValue);

            var t = MathUtility.Remap(time, 0f, duration.Value, 0f, 1f);
            var easedT = Ease.EaseTime(t);

            if (SecondaryEase != null)
                return SecondaryEase.Ease(startValue, endValue, easedT);

            var res = Mathf.Lerp(startValue, endValue, easedT);
            return res;
        }
    }
}
