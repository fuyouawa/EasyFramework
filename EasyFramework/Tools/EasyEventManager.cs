using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Diagnostics;

namespace EasyFramework
{
    /// <summary>
    /// <para>事件处理器的触发行为扩展</para>
    /// <para>可以使用该委托实现例如：确保在UI线程调用事件处理器</para>
    /// </summary>
    /// <param name="triggerInvoker">事件处理器的触发调用对象</param>
    public delegate void EasyEventTriggerExtensionDelegate(Action triggerInvoker);

    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    public delegate void EasyEventHandlerDelegate<in TEvent>(object sender, TEvent e);

    /// <summary>
    /// RegisterSubscriber的配置
    /// </summary>
    public class RegisterEasyEventSubscriberConfig
    {
        /// <summary>
        /// 注册所有基类中的事件处理器
        /// </summary>
        public bool IncludeBaseClass = false;
    }

    /// <summary>
    /// <para>标记为事件处理器</para>
    /// <para>事件处理器必须为2个参数，第一个参数是发送者（推荐直接object），第二个参数是事件类型</para>
    /// <para>当RegisterEasyEventSubscriber时会使用该特性</para>
    /// </summary>
    public class EasyEventHandlerAttribute : Attribute
    {
    }

    public interface IEasyEventSubscriber
    {
    }

    public static class EasyEventSubscriberExtension
    {
        /// <summary>
        /// 注册target中所有标记了EasyEventHandler特性的成员函数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IUnRegister RegisterEasyEventSubscriber<T>(this T target,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
            where T : IEasyEventSubscriber
        {
            return EasyEventManager.Instance.RegisterSubscriber(target, triggerExtension);
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
        public static IUnRegister RegisterEasyEventSubscriber<T>(this T target,
            RegisterEasyEventSubscriberConfig config, EasyEventTriggerExtensionDelegate triggerExtension = null)
            where T : IEasyEventSubscriber
        {
            return EasyEventManager.Instance.RegisterSubscriber(target, config, triggerExtension);
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        public static IUnRegister RegisterEasyEventHandler<T, TEvent>(this T target, EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
            where T : IEasyEventSubscriber
        {
            return EasyEventManager.Instance.Register(target, handler, triggerExtension);
        }

        /// <summary>
        /// 取消注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static bool UnRegisterEasyEventHandler<T, TEvent>(this T target, EasyEventHandlerDelegate<TEvent> handler)
            where T : IEasyEventSubscriber
        {
            return EasyEventManager.Instance.UnRegister(target, handler);
        }

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool TriggerEasyEvent<T, TEvent>(this T target, TEvent e)
            where T : IEasyEventSubscriber
        {
            return EasyEventManager.Instance.Trigger(target, e);
        }
    }

    internal class EasyEventManager : Singleton<EasyEventManager>
    {
        private readonly Dictionary<Type, TargetToHandlers> _eventTypeToHandlers;

        EasyEventManager()
        {
            _eventTypeToHandlers = new Dictionary<Type, TargetToHandlers>();
        }

        public IUnRegister RegisterSubscriber<T>(T target, EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterSubscriber(target, new RegisterEasyEventSubscriberConfig(), triggerExtension);
        }

        public IUnRegister RegisterSubscriber<T>(T target, RegisterEasyEventSubscriberConfig config,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterTypeSubscriber(new TypeTarget(target, typeof(T)), config, triggerExtension);
        }

        public IUnRegister Register<T, TEvent>(T target, EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterTypeEvent(new TypeTarget(target, typeof(T)), typeof(TEvent), handler, triggerExtension);
        }

        public bool UnRegister<T, TEvent>(T target, EasyEventHandlerDelegate<TEvent> handler)
        {
            return UnRegisterTypeEvent(new TypeTarget(target, typeof(T)), typeof(TEvent), handler);
        }

        public bool Trigger<T, TEvent>(T sender, TEvent eventArg)
        {
            return TriggerTypeEvent(new TypeTarget(sender, typeof(T)), typeof(TEvent), eventArg);
        }
        
        private struct TypeTarget
        {
            public object Target;
            public Type Type;

            public TypeTarget(object target, Type type)
            {
                Target = target;
                Type = type;
            }
        }

        private class HandlerItem
        {
            public Delegate Handler;
            public EasyEventTriggerExtensionDelegate TriggerExtension;

            public void Invoke(TypeTarget sender, object eventArg)
            {
                if (TriggerExtension != null)
                {
                    TriggerExtension(() => Handler.DynamicInvoke(sender.Target, eventArg));
                }
                else
                {
                    Handler.DynamicInvoke(sender.Target, eventArg);
                }
            }
        }

        private class TargetToHandlers : Dictionary<TypeTarget, List<HandlerItem>>
        {
            public void AddHandler(TypeTarget target, Delegate handler,
                EasyEventTriggerExtensionDelegate triggerExtension)
            {
                if (!TryGetValue(target, out var handlers))
                {
                    handlers = new List<HandlerItem>();
                    this[target] = handlers;
                }

                if (handlers.FirstOrDefault(h => h.Handler == handler) != null)
                {
                    throw new ArgumentException($"You can't register an event handler({handler}) repeatedly");
                }

                handlers.Add(new HandlerItem() { Handler = handler, TriggerExtension = triggerExtension });
            }

            public bool RemoveHandler(TypeTarget target, Delegate handler)
            {
                if (TryGetValue(target, out var handlers))
                {
                    int i = 0;
                    for (; i < Count; i++)
                    {
                        if (handlers[i].Handler == handler)
                        {
                            break;
                        }
                    }

                    if (i < Count)
                    {
                        handlers.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            public void Invoke(TypeTarget sender, object eventArg)
            {
                foreach (var handlers in this.Values)
                {
                    foreach (var handler in handlers)
                    {
                        handler.Invoke(sender, eventArg);
                    }
                }
            }
        }

        private IUnRegister RegisterTypeSubscriber(TypeTarget target, RegisterEasyEventSubscriberConfig config,
            EasyEventTriggerExtensionDelegate triggerExtension)
        {
            Action unregister = null;

            if (config.IncludeBaseClass && target.Type.BaseType != null)
            {
                var ret = RegisterTypeSubscriber(new TypeTarget(target.Target, target.Type), config, triggerExtension);
                unregister += () => ret.UnRegister();
            }

            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var handlers = target.Type.GetMethods(bf);
            // 排除基类
            if (target.Type.BaseType != null)
            {
                handlers = handlers.Except(target.Type.BaseType.GetMethods(bf)).ToArray();
            }

            handlers = handlers.Where(h => h.HasCustomAttribute<EasyEventHandlerAttribute>()).ToArray();

            foreach (var h in handlers)
            {
                var p = h.GetParameters();
                if (p.Length != 2)
                {
                    throw new ArgumentException(
                        $"The number of arguments to the event handler({h.GetSignature()}) must be 2!");
                }

                var et = p[0].ParameterType;
                var func = h.CreateDelegate(target);

                RegisterTypeEvent(target, et, func, triggerExtension);
                unregister += () => UnRegisterTypeEvent(target, et, func);
            }

            return new CustomUnRegister(unregister);
        }

        private IUnRegister RegisterTypeEvent(TypeTarget target, Type eventType, Delegate handler,
            EasyEventTriggerExtensionDelegate triggerExtension)
        {
            if (!_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new TargetToHandlers();
                _eventTypeToHandlers[eventType] = handlers;
            }

            handlers.AddHandler(target, handler, triggerExtension);
            return new CustomUnRegister(() => UnRegisterTypeEvent(target, eventType, handler));
        }


        private bool UnRegisterTypeEvent(TypeTarget target, Type eventType, Delegate handler)
        {
            if (_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                return handlers.RemoveHandler(target, handler);
            }

            return false;
        }

        private bool TriggerTypeEvent(TypeTarget target, Type eventType, object eventArg)
        {
            if (_eventTypeToHandlers.TryGetValue(eventType, out var handlers))
            {
                handlers.Invoke(target, eventArg);
                return true;
            }

            return false;
        }
    }
}