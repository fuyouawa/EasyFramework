using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum FlexibleModes
    {
        Instant,
        Overtime
    }
    
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class FlexibleFloat
    {
        [SerializeField] private FlexibleModes _mode;
        [SerializeField] private float _value;
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] private float _curveMinValueRemap;
        [SerializeField] private float _curveMaxValueRemap;

        public FlexibleModes Mode
        {
            get => _mode;
            set => _mode = value;
        }
        
        public bool IsInstant => _mode == FlexibleModes.Instant;
        public bool IsOvertime => _mode == FlexibleModes.Overtime;

        public float InstantOfValue
        {
            get => _value;
            set => _value = value;
        }

        public AnimationCurve OvertimeOfCurve
        {
            get => _curve;
            set => _curve = value;
        }

        public float CurveMinValueRemap
        {
            get => _curveMinValueRemap;
            set => _curveMinValueRemap = value;
        }

        public float CurveMaxValueRemap
        {
            get => _curveMaxValueRemap;
            set => _curveMaxValueRemap = value;
        }

        public Vector2 CurveValueRemap
        {
            get => new Vector2(_curveMinValueRemap, _curveMaxValueRemap);
            set
            {
                _curveMinValueRemap = value.x;
                _curveMaxValueRemap = value.y;
            }
        }


        public FlexibleFloat(float value)
        {
            _mode = FlexibleModes.Instant;
            _value = value;
            _curve = AnimationCurve.EaseInOut(0f, 0f, 1f, value);
            _curveMinValueRemap = 0f;
            _curveMaxValueRemap = value;
        }

        public FlexibleFloat(
            AnimationCurve curve,
            float minValueRemap = 0f,
            float maxValueRemap = 1f)
        {
            _mode = FlexibleModes.Overtime;
            _value = 0;
            _curve = curve;
            _curveMinValueRemap = minValueRemap;
            _curveMaxValueRemap = maxValueRemap;
        }

        public FlexibleFloat(
            FlexibleModes mode,
            float instantOfValue,
            AnimationCurve overtimeOfCurve,
            float minValueRemap = 0f,
            float maxValueRemap = 1f)
        {
            _mode = mode;
            _value = instantOfValue;
            _curve = overtimeOfCurve;
            _curveMinValueRemap = minValueRemap;
            _curveMaxValueRemap = maxValueRemap;
        }

        public float Evaluate(float time, float maxTime)
        {
            if (IsOvertime)
            {
                return _curve.EvaluateWithRemap(time, maxTime, _curveMinValueRemap, _curveMaxValueRemap);
            }

            return _value;
        }
    }
}
