using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public abstract class EasyEventBase
    {
        [SerializeField, MethodPickerSettings(LimitParameterTypesGetter = nameof(GetLimitParameterTypes))]
        private List<MethodPicker> _methodPickers;

        private Delegate _handlers;

        protected IFromRegister RegisterImpl(Delegate handler)
        {
            _handlers = Delegate.Combine(_handlers, handler);
            return new FromRegisterGeneric(() => UnregisterImpl(handler));
        }

        protected void UnregisterImpl(Delegate handler)
        {
            _handlers = Delegate.Remove(_handlers, handler);
        }

        protected void InvokeImpl(params object[] args)
        {
            if (_methodPickers.IsNotNullOrEmpty())
            {
                foreach (var picker in _methodPickers)
                    picker.Invoke();
            }

            _handlers?.DynamicInvoke(args);
        }

        public void Clear()
        {
            _methodPickers.Clear();
            _handlers = null;
        }

        protected abstract IEnumerable<Type> GetLimitParameterTypes();
    }

    public class EasyEvent : EasyEventBase
    {
        public delegate void Handler();

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);
        public void Invoke() => InvokeImpl();
        protected override IEnumerable<Type> GetLimitParameterTypes() => null;
    }

    public class EasyEvent<T> : EasyEventBase
    {
        public delegate void Handler(T arg);

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);
        public void Invoke(T arg) => InvokeImpl(arg);
        protected override IEnumerable<Type> GetLimitParameterTypes() => new[] { typeof(T) };
    }

    public class EasyEvent<T1, T2> : EasyEventBase
    {
        public delegate void Handler(T1 arg1, T2 arg2);

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);
        public void Invoke(T1 arg1, T2 arg2) => InvokeImpl(arg1, arg2);
        protected override IEnumerable<Type> GetLimitParameterTypes() => new[] { typeof(T1), typeof(T2) };
    }

    public class EasyEvent<T1, T2, T3> : EasyEventBase
    {
        public delegate void Handler(T1 arg1, T2 arg2, T3 arg3);

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);
        public void Invoke(T1 arg1, T2 arg2, T3 arg3) => InvokeImpl(arg1, arg2, arg3);
        protected override IEnumerable<Type> GetLimitParameterTypes() => new[] { typeof(T1), typeof(T2), typeof(T3) };
    }
}
