using Sirenix.OdinInspector;
using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class GradientColor
    {
        public bool IsGradient;
        public Color Color;
        public Gradient Gradient;

        public GradientColor(Color color)
        {
            IsGradient = false;
            Color = color;
            Gradient = new Gradient();

            var colorKeys = new GradientColorKey[2];
            colorKeys[0].color = Color;
            colorKeys[0].time = 0.0f;
            colorKeys[1].color = Color;
            colorKeys[1].time = 1.0f;

            Gradient.colorKeys = colorKeys;
        }

        public GradientColor(Gradient gradient)
        {
            IsGradient = true;
            Gradient = gradient;
        }

        public GradientColor(
            bool isGradient,
            Color color,
            Gradient gradient)
        {
            IsGradient = isGradient;
            Color = color;
            Gradient = gradient;
        }

        public Color Evaluate(float time, float maxTime)
        {
            if (IsGradient)
                return Gradient.EvaluateWithRemap(time, maxTime);

            return Color;
        }
    }
}
