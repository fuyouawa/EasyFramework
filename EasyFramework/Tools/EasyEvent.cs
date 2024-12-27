using System;

namespace EasyFramework
{
    public class EasyEvent
    {
        private event Action _handler;
        
        public IUnRegister Register(Action handler)
        {
            _handler += handler;
            return new CustomUnRegister(() => UnRegister(handler));
        }

        public void UnRegister(Action handler)
        {
            _handler -= handler;
        }

        public void Trigger()
        {
            _handler?.Invoke();
        }
    }

    public class EasyEvent<T>
    {
        private event Action<T> _handler;
        
        public IUnRegister Register(Action<T> handler)
        {
            _handler += handler;
            return new CustomUnRegister(() => UnRegister(handler));
        }

        public void UnRegister(Action<T> handler)
        {
            _handler -= handler;
        }

        public void Trigger(T arg)
        {
            _handler?.Invoke(arg);
        }
    }

    public class EasyEvent<T1, T2>
    {
        private event Action<T1, T2> _handler;
        
        public IUnRegister Register(Action<T1, T2> handler)
        {
            _handler += handler;
            return new CustomUnRegister(() => UnRegister(handler));
        }

        public void UnRegister(Action<T1, T2> handler)
        {
            _handler -= handler;
        }

        public void Trigger(T1 arg1, T2 arg2)
        {
            _handler?.Invoke(arg1, arg2);
        }
    }

    public class EasyEvent<T1, T2, T3>
    {
        private event Action<T1, T2, T3> _handler;
        
        public IUnRegister Register(Action<T1, T2, T3> handler)
        {
            _handler += handler;
            return new CustomUnRegister(() => UnRegister(handler));
        }

        public void UnRegister(Action<T1, T2, T3> handler)
        {
            _handler -= handler;
        }

        public void Trigger(T1 arg1, T2 arg2, T3 arg3)
        {
            _handler?.Invoke(arg1, arg2, arg3);
        }
    }
}