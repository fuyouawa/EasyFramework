using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using JetBrains.Annotations;

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
    /// <para>标记为事件处理器</para>
    /// <para>事件处理器必须为2个参数，第一个参数是发送者（推荐直接object），第二个参数是事件类型</para>
    /// <para>当RegisterEasyEventSubscriber时会使用该特性</para>
    /// </summary>
    [MeansImplicitUse(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Itself)]
    [AttributeUsage(AttributeTargets.Method)]
    public class EasyEventHandlerAttribute : Attribute
    {
    }

    /// <summary>
    /// <para>事件发派者</para>
    /// <para>继承此接口后便可以使用this.xxx进行注册/取消注册/触发事件等</para>
    /// </summary>
    public interface IEasyEventDispatcher
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static class EasyEventSubscriberExtension
    {
        /// <summary>
        /// 注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="includeBaseClass">包含基类的事件处理器</param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IUnRegisterConfiguration RegisterEasyEventSubscriber<T>(this T target,
            bool includeBaseClass = false,
            EasyEventTriggerExtensionDelegate triggerExtension = null) where T : IEasyEventDispatcher
        {
            return EasyEventManager.Instance.RegisterSubscriber(target, typeof(T), includeBaseClass, triggerExtension);
        }

        /// <summary>
        /// 取消注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="includeBaseClass">包含基类的事件处理器</param>
        /// <returns></returns>
        public static bool UnRegisterEasyEventSubscriber<T>(this T target,
            bool includeBaseClass = false)
        {
            return EasyEventManager.Instance.UnRegisterSubscriber(target, typeof(T), includeBaseClass);
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        public static IUnRegisterConfiguration RegisterEasyEventHandler<T, TEvent>(this T target,
            EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
            where T : IEasyEventDispatcher
        {
            return EasyEventManager.Instance.RegisterHandler(target, typeof(T), typeof(TEvent), handler, triggerExtension);
        }

        /// <summary>
        /// 取消注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static bool UnRegisterEasyEventHandler<T, TEvent>(this T target,
            EasyEventHandlerDelegate<TEvent> handler)
            where T : IEasyEventDispatcher
        {
            return EasyEventManager.Instance.UnRegisterHandler(target, typeof(T), typeof(TEvent), handler);
        }

        /// <summary>
        /// <para>触发事件</para>
        /// <para>注意：在事件处理器中触发事件要谨慎，处理不当可能会死锁</para>
        /// <para>如果出现死锁了，检查以下情况</para>
        /// <para>1、是否事件处理器中再次触发处理的事件（包括整个触发栈）</para>
        /// <para>2、待补充。。。。。。</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="e">事件</param>
        /// <returns></returns>
        public static bool TriggerEasyEvent<T, TEvent>(this T target, TEvent e)
            where T : IEasyEventDispatcher
        {
            return EasyEventManager.Instance.TriggerEvent(target, typeof(T), e, typeof(TEvent));
        }
    }

    internal class EasyEventManager : Singleton<EasyEventManager>
    {
        private readonly Dictionary<Type, TargetToHandlers> _eventTypeToHandlers;

        EasyEventManager()
        {
            _eventTypeToHandlers = new Dictionary<Type, TargetToHandlers>();
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
                void Call() => Handler.DynamicInvoke(sender.Target, eventArg);
                if (TriggerExtension != null)
                {
                    TriggerExtension(Call);
                }
                else
                {
                    Call();
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

        public IUnRegisterConfiguration RegisterSubscriber(object target, Type targetType, bool includeBaseClass = false, EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var handlers = targetType.GetMethods(flags);
            // 排除基类
            if (targetType.BaseType != null)
            {
                handlers = handlers.Except(targetType.BaseType.GetMethods(flags)).ToArray();
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

                var eventType = p[1].ParameterType;
                var func = h.CreateDelegate(target);

                RegisterHandler(target, targetType, eventType, func, triggerExtension);
            }

            Action unregister = () => UnRegisterSubscriber(target, targetType, includeBaseClass);

            if (includeBaseClass && targetType.BaseType != null)
            {
                var ret = RegisterSubscriber(target, targetType.BaseType, true, triggerExtension);
                unregister += () => ret.UnRegister();
            }

            return new UnRegisterConfiguration(unregister);
        }

        public bool UnRegisterSubscriber(object target, Type targetType, bool includeBaseClass = false)
        {
            var t = new TypeTarget(target, targetType);
            bool has = false;

            lock (_eventTypeToHandlers)
            {
                foreach (var handlers in _eventTypeToHandlers.Values)
                {
                    var suc = handlers.Remove(t);
                    if (!has) has = suc;
                }
            }

            if (includeBaseClass && targetType.BaseType != null)
            {
                var suc = UnRegisterSubscriber(target, targetType, true);
                if (!has) has = suc;
            }

            return has;
        }

        public IUnRegisterConfiguration RegisterHandler(object target, Type targetType, Type eventType, Delegate handler,
            EasyEventTriggerExtensionDelegate triggerExtension)
        {
            TargetToHandlers handlers;
            lock (_eventTypeToHandlers)
            {
                if (!_eventTypeToHandlers.TryGetValue(eventType, out handlers))
                {
                    handlers = new TargetToHandlers();
                    _eventTypeToHandlers[eventType] = handlers;
                }
            }

            lock (handlers)
            {
                handlers.AddHandler(new TypeTarget(target, targetType), handler, triggerExtension);
            }

            return new UnRegisterConfiguration(() => UnRegisterHandler(target, targetType, eventType, handler));
        }


        public bool UnRegisterHandler(object target, Type targetType, Type eventType, Delegate handler)
        {
            TargetToHandlers handlers;
            lock (_eventTypeToHandlers)
            {
                if (!_eventTypeToHandlers.TryGetValue(eventType, out handlers))
                {
                    return false;
                }
            }

            lock (handlers)
            {
                return handlers.RemoveHandler(new TypeTarget(target, targetType), handler);
            }
        }

        public bool TriggerEvent(object target, Type targetType, object eventArg, Type eventType)
        {
            TargetToHandlers handlers;
            lock (_eventTypeToHandlers)
            {
                if (!_eventTypeToHandlers.TryGetValue(eventType, out handlers))
                {
                    return false;
                }
            }

            lock (handlers)
            {
                handlers.Invoke(new TypeTarget(target, targetType), eventArg);
            }
            return true;
        }
    }
}
