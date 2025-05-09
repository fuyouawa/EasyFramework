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

        protected override Vector3 GetLinearValue(Vector3 startValue, Vector3 endValue, float t)
        {
            var dist = Vector3.Distance(startValue, endValue);
            var dir = (endValue - startValue).normalized;
            var curDist = Mathf.Lerp(0, dist, t);
            return startValue + curDist * dir;
        }

        protected override Vector3 GetRelativeEndValue(Vector3 startValue, Vector3 relativeValue)
        {
            return startValue + relativeValue;
        }
    }
}
