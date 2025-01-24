using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EasyFramework
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class FlexibleColor
    {
        [SerializeField] private FlexibleModes _mode;
        [SerializeField] private Color _color;
        [SerializeField] private Gradient _gradient;

        public FlexibleModes Mode
        {
            get => _mode;
            set => _mode = value;
        }
        
        public bool IsInstant => _mode == FlexibleModes.Instant;
        public bool IsOvertime => _mode == FlexibleModes.Overtime;

        public Color InstantOfColor
        {
            get => _color;
            set => _color = value;
        }

        public Gradient OvertimeOfGradient
        {
            get => _gradient;
            set => _gradient = value;
        }

        public FlexibleColor(Color color)
        {
            _mode = FlexibleModes.Instant;
            _color = color;
            _gradient = new Gradient();
            
            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = _color;
            colorKeys[0].time = 0.0f;
            colorKeys[1].color = _color;
            colorKeys[1].time = 1.0f;

            _gradient.colorKeys = colorKeys;
        }

        public FlexibleColor(Gradient gradient)
        {
            _mode = FlexibleModes.Overtime;
            _gradient = gradient;
        }

        public FlexibleColor(
            FlexibleModes mode,
            Color instantOfColor,
            Gradient overtimeOfGradient)
        {
            _mode = mode;
            _color = instantOfColor;
            _gradient = overtimeOfGradient;
        }

        public Color Evaluate(float time, float maxTime)
        {
            if (IsOvertime)
            {
                return _gradient.EvaluateWithRemap(time, maxTime);
            }

            return _color;
        }
    }
}
