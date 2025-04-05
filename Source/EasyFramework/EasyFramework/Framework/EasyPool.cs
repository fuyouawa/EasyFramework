using System;
using System.Collections.Generic;

namespace EasyFramework
{
    internal class EasyPoolImpl<T>
        where T : class, new()
    {
        public static EasyPoolImpl<T> Instance { get; } = new EasyPoolImpl<T>();

        private T _fastCache;
        private readonly Stack<T> _stack = new Stack<T>();

        public T Alloc()
        {
            if (_fastCache != null)
            {
                var tmp = _fastCache;
                _fastCache = null;
                return tmp;
            }

            if (_stack.TryPop(out var result))
            {
                return result;
            }

            return new T();
        }


        public void Recycle(T value)
        {
            if (_fastCache == null)
            {
                _fastCache = value;
                return;
            }

            _stack.Push(value);
        }
    }

    public class EasyPool<T>
        where T : class, new()
    {
        public struct Scope : IDisposable
        {
            public T Value { get; }

            public Scope(T value)
            {
                Value = value;
            }

            public void Dispose()
            {
                Recycle(Value);
            }
        }

        public static Scope AllocScope()
        {
            return new Scope(Alloc());
        }

        public static T Alloc()
        {
            return EasyPoolImpl<T>.Instance.Alloc();
        }

        public static void Recycle(T value)
        {
            EasyPoolImpl<T>.Instance.Recycle(value);
        }
    }
}
