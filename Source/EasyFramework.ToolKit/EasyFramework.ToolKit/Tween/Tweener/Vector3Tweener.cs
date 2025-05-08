using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class Vector3Tweener : AbstractTweener<Vector3>
    {
        protected override float GetDistance(Vector3 startValue, Vector3 endValue)
        {
            return Mathf.Max(
                (endValue.x - startValue.x).Abs(),
                (endValue.y - startValue.y).Abs(),
                (endValue.z - startValue.z).Abs()
            );
        }

        protected override Vector3 GetCurrentValue(float time, float duration, Vector3 startValue, Vector3 endValue)
        {
            var t = MathUtility.Remap(time, 0f, duration, 0f, 1f);

            float x;
            float y;
            float z;
            switch (DurationMode)
            {
                case DurationMode.Normal:
                    x = TweenUtility.EaseValue(EaseMode, t, startValue.x, endValue.x);
                    y = TweenUtility.EaseValue(EaseMode, t, startValue.y, endValue.y);
                    z = TweenUtility.EaseValue(EaseMode, t, startValue.z, endValue.z);
                    break;
                case DurationMode.Speed:
                {
                    var dist = GetDistance(startValue, endValue);
                    x = TweenUtility.EaseValue(EaseMode, t, startValue.x, startValue.x + dist * endValue.x.Sign());
                    y = TweenUtility.EaseValue(EaseMode, t, startValue.y, startValue.y + dist * endValue.y.Sign());
                    z = TweenUtility.EaseValue(EaseMode, t, startValue.z, startValue.z + dist * endValue.z.Sign());
                    x = x.Clamp(startValue.x, endValue.x);
                    y = y.Clamp(startValue.y, endValue.y);
                    z = z.Clamp(startValue.y, endValue.y);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Vector3(x, y, z);
        }
    }
}
