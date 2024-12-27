using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Diagnostics;

namespace EasyFramework
{
    /// <summary>
    /// <para>标记为事件处理器</para>
    /// <para>当使用EasyEventManager.Instance.RegisterSubscriber时会使用该特性</para>
    /// </summary>
    public class EasyEventHandlerAttribute : Attribute
    {

    }

    public class EasyEventManager : Singleton<EasyEventManager>
    {
        /// <summary>
        /// <para>事件处理器的触发行为扩展</para>
        /// <para>可以使用该委托实现例如：确保在UI线程调用事件处理器</para>
        /// </summary>
        /// <param name="triggerInvoker">事件处理器的触发调用对象</param>
        public delegate void TriggerExtensionDelegate(Action triggerInvoker);
        
        /// <summary>
        /// RegisterSubscriber的配置
        /// </summary>
        public class RegisterSubscriberConfig
        {
            /// <summary>
            /// 忽略基类中的事件处理器（只注册自己类中标记了EasyEventHandler特性的处理器）
            /// </summary>
            public bool IgnoreBaseClass = true;
        }

        private class HandlerItem
        {
            public Delegate Handler;
            public TriggerExtensionDelegate TriggerExtension;

            public void Invoke(object arg)
            {
                if (TriggerExtension != null)
                {
                    TriggerExtension(() => Handler.DynamicInvoke(arg));
                }
                else
                {
                    Handler.DynamicInvoke(arg);
                }
            }
        }

        private class HandlerItemList : List<HandlerItem>
        {
            public void AddHandler(Delegate handler, TriggerExtensionDelegate triggerExtension)
            {
                if (this.FirstOrDefault(h => h.Handler == handler) != null)
                {
                    throw new ArgumentException($"You can't register an event handler({handler}) repeatedly");
                }
                Add(new HandlerItem(){Handler = handler, TriggerExtension = triggerExtension});
            }

            public bool RemoveHandler(Delegate handler)
            {
                int i = 0;
                for (; i < Count; i++)
                {
                    if (this[i].Handler == handler)
                    {
                        break;
                    }
                }

                if (i < Count)
                {
                    RemoveAt(i);
                    return true;
                }

                return false;
            }

            public void Invoke(object arg)
            {
                foreach (var handler in this)
                {
                    handler.Invoke(arg);
                }
            }
        }

        private readonly Dictionary<Type, HandlerItemList> _eventTypeToHandlers;
    
        EasyEventManager()
        {
            _eventTypeToHandlers = new Dictionary<Type, HandlerItemList>();
        }

        /// <summary>
        /// 注册target中所有标记了EasyEventHandler特性的成员函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IUnRegister RegisterSubscriber<T>(T target, TriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterSubscriber(target, new RegisterSubscriberConfig(), triggerExtension);
        }

        /// <summary>
        /// 注册target中所有标记了EasyEventHandler特性的成员函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="config">配置</param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IUnRegister RegisterSubscriber<T>(T target, RegisterSubscriberConfig config, TriggerExtensionDelegate triggerExtension = null)
        {
            var type = typeof(T);
            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var handlers = type.GetMethods(bf);
            // 排除基类
            if (type.BaseType != null && config.IgnoreBaseClass)
            {
                handlers = handlers.Except(type.BaseType.GetMethods(bf)).ToArray();
            }

            handlers = handlers.Where(h => h.HasCustomAttribute<EasyEventHandlerAttribute>()).ToArray();

            Action unregister = null;
            foreach (var h in handlers)
            {
                var p = h.GetParameters();
                if (p.Length != 1)
                {
                    throw new ArgumentException($"The number of arguments to the event handler({h.GetSignature()}) must be 1!");
                }

                var et = p[0].ParameterType;
                var func = h.CreateDelegate(target);

                RegisterTypeEvent(et, func, triggerExtension);
                unregister += () => UnRegisterTypeEvent(et, func);
            }

            return new CustomUnRegister(unregister);
        }

        // private MethodInfo _register;
        private IUnRegister RegisterTypeEvent(Type eventType, Delegate handler, TriggerExtensionDelegate triggerExtension)
        {
            // if (_register == null)
            // {
            //     _register = typeof(EasyEventManager).GetMethod(nameof(Register), BindingFlags.Public | BindingFlags.Instance);
            // }
            // Debug.Assert(_register != null);
            //
            // var m = _register.MakeGenericMethod(eventType);
            // m.Invoke(this, new object[] { handler, triggerExtension });
            if (!_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new HandlerItemList();
                _eventTypeToHandlers[eventType] = handlers;
            }
            handlers.AddHandler(handler, triggerExtension);
            return new CustomUnRegister(() => UnRegisterTypeEvent(eventType, handler));
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        public IUnRegister Register<TEvent>(Action<TEvent> handler, TriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterTypeEvent(typeof(TEvent), handler, triggerExtension);
        }


        // private MethodInfo _unregister;
        private bool UnRegisterTypeEvent(Type eventType, Delegate handler)
        {
            // if (_unregister == null)
            // {
            //     _unregister = typeof(EasyEventManager).GetMethod(nameof(UnRegister), BindingFlags.Public | BindingFlags.Instance);
            // }
            // Debug.Assert(_unregister != null);
            //
            // var m = _unregister.MakeGenericMethod(eventType);
            // m.Invoke(this, new object[] { handler });

            if (_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                return handlers.RemoveHandler(handler);
            }
            return false;
        }

        /// <summary>
        /// 取消注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool UnRegister<TEvent>(Action<TEvent> handler)
        {
            return UnRegisterTypeEvent(typeof(TEvent), handler);
        }

        private bool TriggerTypeEvent(Type eventType, object arg)
        {
            if (_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Invoke(arg);
                return true;
            }
            return false;
        }
    
        public bool Trigger<TEvent>(TEvent arg)
        {
            return TriggerTypeEvent(typeof(TEvent), arg);
        }
    }
}