using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class FloatLinearTweenerProcessor : AbstractTweenerProcessor<float, LinearTweenerEffect>
    {
        protected override float GetRelativeValue(float value, float relative)
        {
            return value + relative;
        }

        protected override float GetDistance()
        {
            return (Context.EndValue - Context.StartValue).Abs();
        }

        protected override float OnProcess(float time)
        {
            return Mathf.Lerp(Context.StartValue, Context.EndValue, time);
        }
    }
}
