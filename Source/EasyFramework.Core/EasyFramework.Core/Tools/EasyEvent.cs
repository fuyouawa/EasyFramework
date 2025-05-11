using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EasyFramework.Core
{
    [Serializable]
    public abstract class EasyEventBase
    {
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
            _handlers?.DynamicInvoke(args);
        }

        public void Clear()
        {
            _handlers = null;
        }
    }
    
    [Serializable]
    public class EasyEvent : EasyEventBase
    {
        public delegate void Handler();

        [SerializeField] private UnityEvent _unityEvent;

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);

        public void Invoke()
        {
            _unityEvent?.Invoke();
            InvokeImpl();
        }
    }
    
    [Serializable]
    public class EasyEvent<T> : EasyEventBase
    {
        public delegate void Handler(T arg);

        [SerializeField] private UnityEvent<T> _unityEvent;

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);

        public void Invoke(T arg)
        {
            _unityEvent?.Invoke(arg);
            InvokeImpl(arg);
        }
    }
    
    [Serializable]
    public class EasyEvent<T1, T2> : EasyEventBase
    {
        public delegate void Handler(T1 arg1, T2 arg2);

        [SerializeField] private UnityEvent<T1, T2> _unityEvent;

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);

        public void Invoke(T1 arg1, T2 arg2)
        {
            _unityEvent?.Invoke(arg1, arg2);
            InvokeImpl(arg1, arg2);
        }
    }
    
    [Serializable]
    public class EasyEvent<T1, T2, T3> : EasyEventBase
    {
        public delegate void Handler(T1 arg1, T2 arg2, T3 arg3);

        [SerializeField] private UnityEvent<T1, T2, T3> _unityEvent;

        public IFromRegister Register(Handler handler) => RegisterImpl(handler);
        public void Unregister(Handler handler) => UnregisterImpl(handler);

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            _unityEvent?.Invoke(arg1, arg2, arg3);
            InvokeImpl(arg1, arg2, arg3);
        }
    }
}
