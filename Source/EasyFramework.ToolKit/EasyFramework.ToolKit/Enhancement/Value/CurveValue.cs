using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class CurveValue
    {
        public bool IsCurve;
        public float Value;
        public AnimationCurve Curve;
        public Vector2 CurveValueRemap;

        public CurveValue(float value)
        {
            IsCurve = false;
            Value = value;
            Curve = AnimationCurve.EaseInOut(0f, 0f, 1f, value);
            CurveValueRemap = new Vector2(0f, value);
        }

        public CurveValue(AnimationCurve curve)
            : this(true, 0f, curve, new Vector2(0f, 1f))
        {
        }

        public CurveValue(AnimationCurve curve, Vector2 curveValueRemap)
            : this(true, 0f, curve, curveValueRemap)
        {
        }

        public CurveValue(bool isCurve, float value, AnimationCurve animationCurve)
        {
            IsCurve = isCurve;
            Value = value;
            Curve = animationCurve;
            CurveValueRemap = new Vector2(0f, value);
        }

        public CurveValue(bool isCurve, float value,
            AnimationCurve curve, Vector2 curveValueRemap)
        {
            IsCurve = isCurve;
            Value = value;
            Curve = curve;
            CurveValueRemap = curveValueRemap;
        }

        public float Evaluate(float time, float maxTime)
        {
            if (IsCurve)
                return Curve.EvaluateWithRemap(time, maxTime, CurveValueRemap.x, CurveValueRemap.y);

            return Value;
        }
    }
}
