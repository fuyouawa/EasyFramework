using System;
using UnityEngine;
using EasyToolKit.Core;

namespace EasyToolKit.Tweening
{
    public delegate object TweenGetter();
    public delegate void TweenSetter(object val);

    public delegate T TweenGetter<T>();
    public delegate void TweenSetter<T>(T val);

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

    public class Tweener : AbstractTween
    {
        private object _startValue;
        private object _endValue;
        private Type _valueType;
        private TweenGetter _getter;
        private TweenSetter _setter;

        private float _duration;
        private bool _hasUnityObject;

        private UnityEngine.Object _unityObject;
        internal UnityEngine.Object UnityObject
        {
            get => _unityObject;
            set
            {
                if (ReferenceEquals(value, null))
                {
                    _hasUnityObject = false;
                    _unityObject = null;
                }
                else
                {
                    _hasUnityObject = true;
                    _unityObject = value;
                }
            }
        }
        protected internal LoopType LoopType { get; set; }
        protected internal ITweenerEase Ease { get; set; }

        protected internal bool IsSpeedBased { get; set; }
        protected internal bool IsRelative { get; set; }

        private ITweenerEffect _effect;
        private ITweenerProcessor _processor;
        private float? _actualDuration;

        protected override float? ActualDuration
        {
            get
            {
                if (_actualDuration.HasValue)
                {
                    return _actualDuration.Value;
                }

                if (IsSpeedBased)
                {
                    if (CurrentState == TweenState.Idle)
                        return null;
                    _actualDuration = _processor.GetDistance() / _duration;
                }
                else
                {
                    _actualDuration = _duration;
                }

                return _actualDuration;
            }
        }

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
            _unityObject = null;
            _hasUnityObject = false;
            IsRelative = false;
        }

        internal void SetEffectWithUpdateProcessor(ITweenerEffect effect)
        {
            if (effect == _effect)
                return;

            if (_effect == null || effect.GetType() != _effect.GetType())
            {
                _processor = TweenManager.Instance.GetTweenerProcessor(_valueType, effect.GetType());
            }
            _effect = effect;
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
            if (_hasUnityObject && _unityObject == null)
            {
                PendingKillSelf = true;
                return;
            }

            if (Ease == null)
            {
                Ease = Tweening.Ease.Linear();
            }

            if (_effect == null)
            {
                SetEffectWithUpdateProcessor(Effect.Linear());
            }

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
                if (IsRelative)
                {
                    _endValue = _processor.GetRelativeValue(_startValue, _endValue);
                }
            }
            
            _processor.Context.Effect = _effect;
            _processor.Context.StartValue = _startValue;
            _processor.Context.EndValue = _endValue;
            _processor.Initialize();
        }

        protected override void OnPlaying(float time)
        {
            if (_hasUnityObject && _unityObject == null)
            {
                PendingKillSelf = true;
                return;
            }

            var duration = ActualDuration;
            Assert.True(duration.HasValue);
            
            var t = MathUtility.Remap(time, 0f, duration.Value, 0f, 1f);
            var easedT = Ease.EaseTime(t);
            var curValue = _processor.Process(easedT);
            _setter(curValue);
        }
    }
}
