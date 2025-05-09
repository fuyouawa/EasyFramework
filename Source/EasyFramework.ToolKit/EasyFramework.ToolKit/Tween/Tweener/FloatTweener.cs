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

        protected override float GetLinearValue(float startValue, float endValue, float t)
        {
            return Mathf.Lerp(startValue, endValue, t);
        }

        protected override float GetRelativeEndValue(float startValue, float relativeValue)
        {
            return startValue + relativeValue;
        }
    }
}
