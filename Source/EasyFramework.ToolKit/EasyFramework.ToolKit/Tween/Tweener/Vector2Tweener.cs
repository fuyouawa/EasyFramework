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

        protected override Vector2 GetLinearValue(Vector2 startValue, Vector2 endValue, float t)
        {
            var dist = Vector2.Distance(startValue, endValue);
            var dir = (endValue - startValue).normalized;
            var curDist = Mathf.Lerp(0, dist, t);
            return startValue + curDist * dir;
        }

        protected override Vector2 GetRelativeEndValue(Vector2 startValue, Vector2 relativeValue)
        {
            return startValue + relativeValue;
        }
    }
}
