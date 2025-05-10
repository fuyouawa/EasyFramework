using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class IntLinearTweenerProcessor : AbstractTweenerProcessor<int, LinearTweenerEffect>
    {
        protected override int GetRelativeValue(int value, int relative)
        {
            return value + relative;
        }

        protected override float GetDistance()
        {
            return (Context.EndValue - Context.StartValue).Abs();
        }

        protected override int OnProcess(float time)
        {
            return Mathf.RoundToInt(Mathf.Lerp(Context.StartValue, Context.EndValue, time));
        }
    }
}
