using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework
{
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class EasyEvent
    {
        public delegate void Handler();

        [SerializeField, MethodPickerSettings(LimitParameterCount = 0)]
        private List<MethodPicker> _methodPickers;

        private Handler _handler;

        public IFromRegister Register(Handler handler)
        {
            _handler += handler;
            return new FromRegisterGeneric(() => Unregister(handler));
        }

        public void Unregister(Handler handler)
        {
            _handler -= handler;
        }

        public void Invoke()
        {
            if (_methodPickers.IsNotNullOrEmpty())
            {
                foreach (var picker in _methodPickers)
                    picker.Invoke();
            }

            _handler?.Invoke();
        }
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class EasyEvent<T>
    {
        public delegate void Handler(T arg);

        [SerializeField, MethodPickerSettings(LimitParameterTypesGetter = nameof(GetLimitParameterTypes))]
        private List<MethodPicker> _methodPickers;

        private Handler _handler;

        public IFromRegister Register(Handler handler)
        {
            _handler += handler;
            return new FromRegisterGeneric(() => Unregister(handler));
        }

        public void Unregister(Handler handler)
        {
            _handler -= handler;
        }

        public void Invoke(T arg)
        {
            if (_methodPickers.IsNotNullOrEmpty())
            {
                foreach (var picker in _methodPickers)
                    picker.Invoke();
            }

            _handler?.Invoke(arg);
        }

        private Type[] GetLimitParameterTypes()
        {
            return new[] { typeof(T) };
        }
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class EasyEvent<T1, T2>
    {
        public delegate void Handler(T1 arg1, T2 arg2);

        [SerializeField, MethodPickerSettings(LimitParameterTypesGetter = nameof(GetLimitParameterTypes))]
        private List<MethodPicker> _methodPickers;

        private Handler _handler;

        public IFromRegister Register(Handler handler)
        {
            _handler += handler;
            return new FromRegisterGeneric(() => Unregister(handler));
        }

        public void Unregister(Handler handler)
        {
            _handler -= handler;
        }

        public void Invoke(T1 arg1, T2 arg2)
        {
            if (_methodPickers.IsNotNullOrEmpty())
            {
                foreach (var picker in _methodPickers)
                    picker.Invoke();
            }

            _handler?.Invoke(arg1, arg2);
        }

        private Type[] GetLimitParameterTypes()
        {
            return new[] { typeof(T1), typeof(T2) };
        }
    }
    
    [Serializable, HideReferenceObjectPicker, InlineProperty]
    public class EasyEvent<T1, T2, T3>
    {
        public delegate void Handler(T1 arg1, T2 arg2, T3 arg3);

        [SerializeField, MethodPickerSettings(LimitParameterTypesGetter = nameof(GetLimitParameterTypes))]
        private List<MethodPicker> _methodPickers;

        private Handler _handler;

        public IFromRegister Register(Handler handler)
        {
            _handler += handler;
            return new FromRegisterGeneric(() => Unregister(handler));
        }

        public void Unregister(Handler handler)
        {
            _handler -= handler;
        }

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            if (_methodPickers.IsNotNullOrEmpty())
            {
                foreach (var picker in _methodPickers)
                    picker.Invoke();
            }

            _handler?.Invoke(arg1, arg2, arg3);
        }

        private Type[] GetLimitParameterTypes()
        {
            return new[] { typeof(T1), typeof(T2), typeof(T3) };
        }
    }
}
