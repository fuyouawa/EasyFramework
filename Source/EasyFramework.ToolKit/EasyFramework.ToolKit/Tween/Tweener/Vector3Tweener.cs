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

            var dist = Vector3.Distance(startValue, endValue);
            var dir = (endValue - startValue).normalized;
            var curDist = TweenUtility.EaseValue(EaseMode, t, 0f, dist);

            return startValue + curDist * dir;
        }
    }
}
