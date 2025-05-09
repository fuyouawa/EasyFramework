using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class Vector2Tweener : AbstractTweener<Vector2>
    {
        protected override float GetDistance(Vector2 startValue, Vector2 endValue)
        {
            return Vector2.Distance(startValue, endValue);
        }

        protected override Vector2 GetCurrentValue(float time, float? duration, Vector2 startValue, Vector2 endValue)
        {
            Assert.True(duration.HasValue);

            var t = MathUtility.Remap(time, 0f, duration.Value, 0f, 1f);
            var easedT = Ease.EaseTime(t);
            
            if (SecondaryEase != null)
                return SecondaryEase.Ease(startValue, endValue, easedT);
            
            var dist = Vector2.Distance(startValue, endValue);
            var dir = (endValue - startValue).normalized;
            var curDist = Mathf.Lerp(0, dist, easedT);
            return startValue + curDist * dir;
        }
    }
}
