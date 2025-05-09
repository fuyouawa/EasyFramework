using EasyFramework.Core;
using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public interface IEase
    {
        float EaseTime(float time);
    }

    public class GenericEase : IEase
    {
        private readonly Func<float, float> _easeTime;

        internal GenericEase(Func<float, float> easeTime)
        {
            _easeTime = easeTime;
        }

        float IEase.EaseTime(float time)
        {
            return _easeTime(time);
        }
    }
}
