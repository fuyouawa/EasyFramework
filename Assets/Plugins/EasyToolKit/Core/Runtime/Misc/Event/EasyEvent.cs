using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EasyToolKit.Core
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

    public interface IEasyEvent
    {
        IFromRegister Register(Action handler);
        void Unregister(Action handler);
    }

    [Serializable]
    public class EasyEvent : EasyEventBase, IEasyEvent
    {
        [SerializeField] private UnityEvent _unityEvent;

        public IFromRegister Register(Action handler) => RegisterImpl(handler);
        public void Unregister(Action handler) => UnregisterImpl(handler);

        public void Invoke()
        {
            _unityEvent?.Invoke();
            InvokeImpl();
        }
    }

    public interface IEasyEvent<T>
    {
        IFromRegister Register(Action<T> handler);
        void Unregister(Action<T> handler);
    }

    [Serializable]
    public class EasyEvent<T> : EasyEventBase, IEasyEvent<T>
    {
        [SerializeField] private UnityEvent<T> _unityEvent;

        public IFromRegister Register(Action<T> handler) => RegisterImpl(handler);
        public void Unregister(Action<T> handler) => UnregisterImpl(handler);

        public void Invoke(T arg)
        {
            _unityEvent?.Invoke(arg);
            InvokeImpl(arg);
        }
    }

    public interface IEasyEvent<T1, T2>
    {
        IFromRegister Register(Action<T1, T2> handler);
        void Unregister(Action<T1, T2> handler);
    }

    [Serializable]
    public class EasyEvent<T1, T2> : EasyEventBase, IEasyEvent<T1, T2>
    {
        [SerializeField] private UnityEvent<T1, T2> _unityEvent;

        public IFromRegister Register(Action<T1, T2> handler) => RegisterImpl(handler);
        public void Unregister(Action<T1, T2> handler) => UnregisterImpl(handler);

        public void Invoke(T1 arg1, T2 arg2)
        {
            _unityEvent?.Invoke(arg1, arg2);
            InvokeImpl(arg1, arg2);
        }
    }

    public interface IEasyEvent<T1, T2, T3>
    {
        IFromRegister Register(Action<T1, T2, T3> handler);
        void Unregister(Action<T1, T2, T3> handler);
    }

    [Serializable]
    public class EasyEvent<T1, T2, T3> : EasyEventBase, IEasyEvent<T1, T2, T3>
    {
        [SerializeField] private UnityEvent<T1, T2, T3> _unityEvent;

        public IFromRegister Register(Action<T1, T2, T3> handler) => RegisterImpl(handler);
        public void Unregister(Action<T1, T2, T3> handler) => UnregisterImpl(handler);

        public void Invoke(T1 arg1, T2 arg2, T3 arg3)
        {
            _unityEvent?.Invoke(arg1, arg2, arg3);
            InvokeImpl(arg1, arg2, arg3);
        }
    }

    public interface IEasyEvent<T1, T2, T3, T4>
    {
        IFromRegister Register(Action<T1, T2, T3, T4> handler);
        void Unregister(Action<T1, T2, T3, T4> handler);
    }

    [Serializable]
    public class EasyEvent<T1, T2, T3, T4> : EasyEventBase, IEasyEvent<T1, T2, T3, T4>
    {
        [SerializeField] private UnityEvent<T1, T2, T3, T4> _unityEvent;

        public IFromRegister Register(Action<T1, T2, T3, T4> handler) => RegisterImpl(handler);
        public void Unregister(Action<T1, T2, T3, T4> handler) => UnregisterImpl(handler);

        public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            _unityEvent?.Invoke(arg1, arg2, arg3, arg4);
            InvokeImpl(arg1, arg2, arg3, arg4);
        }
    }
}
