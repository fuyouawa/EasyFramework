using System;

namespace EasyFramework.Tweening
{
    internal class TweenerProcessorContext
    {
        public ITweenerEffect Effect { get; set; }
        public object StartValue { get; set; }
        public object EndValue { get; set; }
    }

    public class TweenerProcessorContext<TValue, TEffect>
    {
        public TEffect Effect { get; set; }
        public TValue StartValue { get; set; }
        public TValue EndValue { get; set; }
    }

    internal interface ITweenerProcessor
    {
        bool CanProcess(Type valueType);

        Type ValueType { get; }
        Type EffectType { get; }
        TweenerProcessorContext Context { get; }

        object GetRelativeValue(object value, object relative);
        float GetDistance();
        void Initialize();
        object Process(float normalizedTime);
    }

    public abstract class AbstractTweenerProcessor<TValue, TEffect> : ITweenerProcessor
        where TEffect : ITweenerEffect
    {
        private readonly TweenerProcessorContext _context = new TweenerProcessorContext();

        bool ITweenerProcessor.CanProcess(Type valueType)
        {
            return CanProcess(valueType);
        }

        Type ITweenerProcessor.ValueType => typeof(TValue);
        Type ITweenerProcessor.EffectType => typeof(TEffect);

        TweenerProcessorContext ITweenerProcessor.Context => _context;

        object ITweenerProcessor.GetRelativeValue(object value, object relative)
        {
            return GetRelativeValue((TValue)value, (TValue)relative);
        }

        float ITweenerProcessor.GetDistance()
        {
            return GetDistance();
        }

        void ITweenerProcessor.Initialize()
        {
            Context.Effect = (TEffect)_context.Effect;
            Context.StartValue = (TValue)_context.StartValue;
            Context.EndValue = (TValue)_context.EndValue;
            OnInit();
        }

        object ITweenerProcessor.Process(float normalizedTime)
        {
            return OnProcess(normalizedTime);
        }

        protected virtual bool CanProcess(Type valueType) => true;
        protected abstract TValue GetRelativeValue(TValue value, TValue relative);

        protected TweenerProcessorContext<TValue, TEffect> Context { get; } = new TweenerProcessorContext<TValue, TEffect>();
        protected abstract float GetDistance();
        protected virtual void OnInit() {}
        protected abstract TValue OnProcess(float normalizedTime);
    }
}
