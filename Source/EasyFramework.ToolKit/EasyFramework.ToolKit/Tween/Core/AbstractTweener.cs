using System;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public delegate object TweenGetter();
    public delegate void TweenSetter(object val);

    public delegate T TweenGetter<T>();
    public delegate void TweenSetter<T>(T val);

    public enum DurationMode
    {
        /// <summary>
        /// 正常模式，在“持续时间”内从“起始值”缓动到“结束值”。
        /// </summary>
        Normal,
        /// <summary>
        /// 速度模式，将“持续时间”的值变成“速度”，从“起始值”开始每秒增加“速度”直到“结束值”。
        /// </summary>
        Speed
    }

    public abstract class AbstractTweener : AbstractTween
    {
        private object _startValue;
        private object _endValue;
        private Type _valueType;
        private TweenGetter _getter;
        private TweenSetter _setter;

        private float _duration;
        protected internal EaseMode EaseMode { get; set; }
        protected internal DurationMode DurationMode { get; set; }

        protected override float GetDuration()
        {
            switch (DurationMode)
            {
                case DurationMode.Normal:
                    return _duration;
                case DurationMode.Speed:
                    if (CurrentState == TweenState.Idle)
                        throw new InvalidOperationException("The duration in the duration mode 'Speed' can only be obtained after started.");
                    return GetDistance(_startValue, _endValue) * _duration;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract float GetDistance(object startValue, object endValue);

        internal void SetDuration(float duration)
        {
            _duration = duration;
            if (OwnerSequence != null)
            {
                OwnerSequence.RefreshDuration();
            }
        }

        protected override void OnReset()
        {
            _startValue = null;
            _endValue = null;
            _valueType = null;
            _getter = null;
            _setter = null;
            _duration = 0f;
        }

        internal void Apply(Type valueType, TweenGetter getter, TweenSetter setter, object endValue)
        {
            _valueType = valueType;
            _getter = getter;
            _setter = setter;
            _endValue = endValue;
        }

        protected override void OnStart()
        {
            _startValue = _getter();
        }

        protected override void OnPlaying(float time)
        {
            var curValue = GetCurrentValue(time, _duration, _startValue, _endValue);
            _setter(curValue);
        }

        protected abstract object GetCurrentValue(float time, float duration, object startValue, object endValue);
    }

    public abstract class AbstractTweener<T> : AbstractTweener
    {
        internal void Apply(TweenGetter<T> getter, TweenSetter<T> setter, T endValue)
        {
            Apply(typeof(T), () => getter(), val => setter((T)val), endValue);
        }

        protected override float GetDistance(object startValue, object endValue)
        {
            return GetDistance((T)startValue, (T)endValue);
        }

        protected override object GetCurrentValue(float time, float duration, object startValue, object endValue)
        {
            return GetCurrentValue(time, duration, (T)startValue, (T)endValue);
        }

        protected abstract float GetDistance(T startValue, T endValue);
        protected abstract T GetCurrentValue(float time, float duration, T startValue, T endValue);
    }
}
