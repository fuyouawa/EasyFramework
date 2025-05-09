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
        /// <para>速度模式，将“持续时间”的值变成“速度”，从“起始值”开始每秒增加“速度”直到“结束值”。</para>
        /// <para>注意：使用此模式后，Tween.Duration()将返回null，除非该Tweener的第一帧被调用了。</para>
        /// </summary>
        Speed
    }

    public enum LoopType
    {
        /// <summary>
        /// 每次循环都从起点重新开始（从A到B，再从A到B，...）
        /// </summary>
        Restart,
        /// <summary>
        /// 每次循环都会反转方向（A到B，再B到A，再A到B...）
        /// </summary>
        Yoyo
    }

    public abstract class AbstractTweener : AbstractTween
    {
        private object _startValue;
        private object _endValue;
        private Type _valueType;
        private TweenGetter _getter;
        private TweenSetter _setter;

        private float _duration;

        protected internal LoopType LoopType { get; set; }
        protected internal EaseType EaseType { get; set; }
        protected internal IEaseConfig EaseConfig { get; set; }
        protected internal SecondaryEaseType SecondaryEaseType { get; set; }
        protected internal ISecondaryEaseConfig SecondaryEaseConfig { get; set; }

        protected internal DurationMode DurationMode { get; set; }

        private float? _actualDuration;

        protected override float? ActualDuration
        {
            get
            {
                if (_actualDuration.HasValue)
                {
                    return _actualDuration.Value;
                }

                switch (DurationMode)
                {
                    case DurationMode.Normal:
                        _actualDuration = _duration;
                        break;
                    case DurationMode.Speed:
                        if (CurrentState == TweenState.Idle)
                            return null;
                        _actualDuration = GetDistance(_startValue, _endValue) / _duration;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return _actualDuration;
            }
        }

        protected abstract float GetDistance(object startValue, object endValue);

        internal void SetDuration(float duration)
        {
            _duration = duration;
            _actualDuration = null;
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
            if (IsInLoop)
            {
                switch (LoopType)
                {
                    case LoopType.Restart:
                        break;
                    case LoopType.Yoyo:
                        (_startValue, _endValue) = (_endValue, _startValue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                _startValue = _getter();
            }
        }

        protected override void OnPlaying(float time)
        {
            var curValue = GetCurrentValue(time, GetActualDuration(), _startValue, _endValue);
            _setter(curValue);
        }

        protected abstract object GetCurrentValue(float time, float? duration, object startValue, object endValue);
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

        protected override object GetCurrentValue(float time, float? duration, object startValue, object endValue)
        {
            return GetCurrentValue(time, duration, (T)startValue, (T)endValue);
        }

        protected abstract float GetDistance(T startValue, T endValue);
        protected abstract T GetCurrentValue(float time, float? duration, T startValue, T endValue);
    }
}
