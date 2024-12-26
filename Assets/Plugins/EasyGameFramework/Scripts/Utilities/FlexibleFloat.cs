using System;
using EasyGameFramework;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    public struct FlexibleFloat
    {
        public bool IsCurve;
        public float Value;
        public AnimationCurve Curve;
        public float CurveMinValueRemap;
        public float CurveMaxValueRemap;

        public FlexibleFloat(float value)
        {
            IsCurve = false;
            Value = value;
            Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, value);
            CurveMinValueRemap = 0f;
            CurveMaxValueRemap = value;
        }

        public FlexibleFloat(AnimationCurve curve, float minValueRemap = 0f, float maxValueRemap = 1f)
        {
            IsCurve = true;
            Value = 0;
            Curve = curve;
            CurveMinValueRemap = minValueRemap;
            CurveMaxValueRemap = maxValueRemap;
        }

        public FlexibleFloat(bool isCurve, float value, AnimationCurve curve, float minValueRemap = 0f, float maxValueRemap = 1f)
        {
            IsCurve = isCurve;
            Value = value;
            Curve = curve;
            CurveMinValueRemap = minValueRemap;
            CurveMaxValueRemap = maxValueRemap;
        }

        public float Evaluate(float time, float maxTime)
        {
            if (IsCurve)
            {
                return Curve.EvaluateWithRemap(time, maxTime, CurveMinValueRemap, CurveMaxValueRemap);
            }

            return Value;
        }
    }
}
