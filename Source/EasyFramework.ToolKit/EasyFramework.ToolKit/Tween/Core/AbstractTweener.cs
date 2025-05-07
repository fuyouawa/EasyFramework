using System;

namespace EasyFramework.ToolKit
{
    public delegate object TweenGetter();
    public delegate void TweenSetter(object val);

    public delegate T TweenGetter<T>();
    public delegate void TweenSetter<T>(T val);

    public abstract class AbstractTweener : AbstractTween
    {
        private object _startValue;
        private object _endValue;
        private Type _valueType;
        private TweenGetter _getter;
        private TweenSetter _setter;

        private float _duration;
        internal Ease Ease { get; set; }

        protected override float GetDuration()
        {
            return _duration;
        }

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

        protected override void OnInit()
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

        protected override object GetCurrentValue(float time, float duration, object startValue, object endValue)
        {
            return GetCurrentValue(time, duration, (T)startValue, (T)endValue);
        }

        protected abstract T GetCurrentValue(float time, float duration, T startValue, T endValue);
    }
}
