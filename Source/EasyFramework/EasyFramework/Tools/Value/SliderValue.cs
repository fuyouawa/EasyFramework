using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace EasyFramework
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class SliderValue
    {
        public bool IsSlider;
        public float Value;
        public Vector2 Slider;
        public Vector2 Limit;

        public float Min
        {
            get
            {
                if (IsSlider)
                    return Slider.x;

                return Value;
            }
        }

        public float Max
        {
            get
            {
                if (IsSlider)
                    return Slider.y;

                return Value;
            }
        }

        public SliderValue(float value)
            : this(value, new Vector2(0f, value))
        {

        }

        public SliderValue(float value, Vector2 limit)
        {
            IsSlider = false;
            Value = value;
            Slider = new Vector2(0f, value);
            Limit = limit;
        }

        public SliderValue(Vector2 slider)
            : this(slider, slider)
        {
        }

        public SliderValue(Vector2 slider, Vector2 limit)
        {
            IsSlider = true;
            Slider = slider;
            Value = slider.y;
            Limit = limit;
        }

        public SliderValue(bool isSlider, float value, Vector2 slider, Vector2 limit)
        {
            IsSlider = isSlider;
            Slider = slider;
            Value = value;
            Slider = slider;
        }

        public float Evaluate()
        {
            if (IsSlider)
                return UnityEngine.Random.Range(Slider.x, Slider.y);
            return Value;
        }
    }
}
