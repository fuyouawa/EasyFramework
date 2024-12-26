using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Diagnostics;

namespace EasyFramework
{
    public class EasyEventHandlerAttribute : Attribute
    {

    }

    public class EasyEventManager
    {
        private static readonly Dictionary<Type, Delegate> _handlers;
    
        static EasyEventManager()
        {
            _handlers = new Dictionary<Type, Delegate>();
        }

        private static Action<TEvent> GetHandler<TEvent>()
        {
            var eventType = typeof(TEvent);
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = null;
            }

            return _handlers[eventType] as Action<TEvent>;
        }

        public static IUnRegister RegisterSubscriber<T>(T obj)
        {
            var type = typeof(T);
            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var handlers = type.GetMethods(bf);
            // 排除基类
            if (type.BaseType != null)
            {
                handlers = handlers.Except(type.BaseType.GetMethods(bf)).ToArray();
            }

            handlers = handlers.Where(h => h.GetCustomAttribute<EasyEventHandlerAttribute>() != null).ToArray();

            Action unregister = null;
            foreach (var h in handlers)
            {
                var p = h.GetParameters();
                if (p.Length != 1)
                {
                    throw new ArgumentException($"事件处理器({h.GetSignature()})的参数数量必须是1!");
                }

                var et = p[0].ParameterType;
                var func = h.CreateDelegate(obj);

                AddTypeListener(et, func);
                unregister += () => RemoveTypeListener(et, func);
            }

            return new CustomUnRegister(unregister);
        }

        private static MethodInfo _addListener;
        private static void AddTypeListener(Type eventType, Delegate handler)
        {
            if (_addListener == null)
            {
                _addListener = typeof(EasyEventManager).GetMethod(nameof(AddListener), BindingFlags.NonPublic | BindingFlags.Static);
            }
            Debug.Assert(_addListener != null);

            var m = _addListener.MakeGenericMethod(eventType);
            m.Invoke(null, new object[] { handler });
        }

        private static void AddListener<TEvent>(Action<TEvent> handler)
        {
            _handlers[typeof(TEvent)] = GetHandler<TEvent>() + handler;
        }


        private static MethodInfo _removeListener;
        private static void RemoveTypeListener(Type eventType, Delegate handler)
        {
            if (_removeListener == null)
            {
                _removeListener = typeof(EasyEventManager).GetMethod(nameof(RemoveListener), BindingFlags.NonPublic | BindingFlags.Static);
            }
            Debug.Assert(_removeListener != null);

            var m = _removeListener.MakeGenericMethod(eventType);
            m.Invoke(null, new object[] { handler });
        }

        private static void RemoveListener<TEvent>(Action<TEvent> handler)
        {
            _handlers[typeof(TEvent)] = GetHandler<TEvent>() - handler;
        }
    
        public static void TriggerEvent<TEvent>(TEvent arg)
        {
            GetHandler<TEvent>()?.Invoke(arg);
        }
    }
}