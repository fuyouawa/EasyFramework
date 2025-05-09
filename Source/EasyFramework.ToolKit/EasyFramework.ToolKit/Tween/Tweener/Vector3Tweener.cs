using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class Vector3Tweener : AbstractTweener<Vector3>
    {
        protected override float GetDistance(Vector3 startValue, Vector3 endValue)
        {
            return Vector3.Distance(startValue, endValue);
        }

        protected override Vector3 GetCurrentValue(float time, float? duration, Vector3 startValue, Vector3 endValue)
        {
            Assert.True(duration.HasValue);

            var t = MathUtility.Remap(time, 0f, duration.Value, 0f, 1f);
            var easedT = Ease.EaseTime(t);
            
            if (SecondaryEase != null)
                return SecondaryEase.Ease(startValue, endValue, easedT);
            
            var dist = Vector3.Distance(startValue, endValue);
            var dir = (endValue - startValue).normalized;
            var curDist = Mathf.Lerp(0, dist, easedT);
            return startValue + curDist * dir;
        }
    }
}
