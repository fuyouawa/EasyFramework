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

        protected override int GetLinearValue(int startValue, int endValue, float t)
        {
            var res = Mathf.Lerp(startValue, endValue, t);
            return Mathf.RoundToInt(res);
        }

        protected override int GetRelativeEndValue(int startValue, int relativeValue)
        {
            return startValue + relativeValue;
        }
    }
}
