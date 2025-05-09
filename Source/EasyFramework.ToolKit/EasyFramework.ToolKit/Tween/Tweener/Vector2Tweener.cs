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
            var easedT = TweenUtility.EaseTime(EaseType, t);

            switch (SecondaryEaseType)
            {
                case SecondaryEaseType.None:
                {

                    var dist = Vector3.Distance(startValue, endValue);
                    var dir = (endValue - startValue).normalized;
                    var curDist = Mathf.Lerp(0, dist, easedT);
                    return startValue + curDist * dir;
                }
                case SecondaryEaseType.QuadraticBezier:
                {
                    var config = SecondaryEaseConfig as QuadraticBezierEaseConfig;
                    if (config == null)
                    {
                        throw new ArgumentException(
                            "Quadratic bezier ease type must have a QuadraticBezierEaseConfig type argument.");
                    }
                    
                    var controlPoint = config.ControlPoint.ToVec2();
                    if (config.RelativeToStartPoint)
                    {
                        controlPoint += startValue;
                    }
                    
                    return MathUtility.QuadraticBezierCurve(startValue, endValue, controlPoint, easedT);
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
