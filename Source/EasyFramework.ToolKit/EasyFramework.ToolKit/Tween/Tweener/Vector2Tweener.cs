using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class Vector2Tweener : AbstractTweener<Vector2>
    {
        protected override float GetDistance(Vector2 startValue, Vector2 endValue)
        {
            return Mathf.Max(
                (endValue.x - startValue.x).Abs(),
                (endValue.y - startValue.y).Abs()
            );
        }

        protected override Vector2 GetCurrentValue(float time, float duration, Vector2 startValue, Vector2 endValue)
        {
            var t = MathUtility.Remap(time, 0f, duration, 0f, 1f);

            float x;
            float y;
            switch (DurationMode)
            {
                case DurationMode.Normal:
                    x = TweenUtility.EaseValue(EaseMode, t, startValue.x, endValue.x);
                    y = TweenUtility.EaseValue(EaseMode, t, startValue.y, endValue.y);
                    break;
                case DurationMode.Speed:
                {
                    var dist = GetDistance(startValue, endValue);
                    x = TweenUtility.EaseValue(EaseMode, t, startValue.x, startValue.x + dist * endValue.x.Sign());
                    y = TweenUtility.EaseValue(EaseMode, t, startValue.y, startValue.y + dist * endValue.y.Sign());
                    x = x.Clamp(startValue.x, endValue.x);
                    y = y.Clamp(startValue.y, endValue.y);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Vector2(x, y);
        }
    }
}
