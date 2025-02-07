using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace EasyFramework
{
    #region Define

    /// <summary>
    /// <para>事件处理器的触发行为扩展</para>
    /// <para>可以使用该委托实现例如：确保在UI线程调用事件处理器</para>
    /// </summary>
    /// <param name="triggerInvoker">事件处理器的触发调用对象</param>
    public delegate void EventTriggerExtensionDelegate(Action triggerInvoker);

    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    public delegate void EventHandlerDelegate<in TEvent>(object sender, TEvent e);

    /// <summary>
    /// <para>标记为事件处理器</para>
    /// <para>事件处理器必须为2个参数，第一个参数是发送者（推荐直接object），第二个参数是事件类型</para>
    /// <para>当RegisterEasyEventSubscriber时会使用该特性</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EasyEventHandlerAttribute : Attribute
    {
    }

    #endregion

    public class EventManager : Singleton<EventManager>
    {
        #region Internal

        private class HandlerItem
        {
            public Delegate Handler;
            public EventTriggerExtensionDelegate TriggerExtension;

            public void Invoke(object sender, object eventArg)
            {
                void Call() => Handler.DynamicInvoke(sender, eventArg);
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

        private class HandleExtension
        {
            public EventTriggerExtensionDelegate TriggerExtension;
        }

        private class Handlers
        {
            private readonly Dictionary<object, HashSet<Delegate>> _handlesMap =
                new Dictionary<object, HashSet<Delegate>>();

            private readonly Dictionary<Delegate, HandleExtension> _handlerExtensionMap =
                new Dictionary<Delegate, HandleExtension>();

            public void AddHandler(object target, Delegate handler)
            {
                if (!_handlesMap.TryGetValue(target, out var handlers))
                {
                    handlers = new HashSet<Delegate>();
                    _handlesMap[target] = handlers;
                }

                var suc = handlers.Add(handler);
                if (!suc)
                {
                    throw new ArgumentException($"You can't register an event handler({handler}) repeatedly");
                }
            }

            public HandleExtension GetHandleExtension(Delegate handler)
            {
                if (!_handlerExtensionMap.TryGetValue(handler, out var extension))
                {
                    extension = new HandleExtension();
                    _handlerExtensionMap[handler] = extension;
                }

                return extension;
            }

            public bool RemoveHandler(object target, Delegate handler)
            {
                if (_handlesMap.TryGetValue(target, out var handlers))
                {
                    return handlers.Remove(handler);
                }

                return false;
            }

            public bool RemoveSubscriber(object target)
            {
                return _handlesMap.Remove(target);
            }

            public void Invoke(object sender, object eventArg)
            {
                foreach (var handlers in _handlesMap.Values)
                {
                    foreach (var handler in handlers)
                    {
                        InternalInvoke(sender, eventArg, handler);
                    }
                }
            }

            private void InternalInvoke(object sender, object eventArg, Delegate handler)
            {
                void Call() => handler.DynamicInvoke(sender, eventArg);

                if (_handlerExtensionMap.TryGetValue(handler, out var extension))
                {
                    if (extension.TriggerExtension != null)
                    {
                        extension.TriggerExtension(Call);
                        return;
                    }
                }

                Call();
            }
        }

        private readonly Dictionary<Type, Handlers> _handlesDict;

        EventManager()
        {
            _handlesDict = new Dictionary<Type, Handlers>();
        }

        #endregion

        #region Register

        /// <summary>
        /// 注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IFromRegisterEvent RegisterSubscriber(object target)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var targetType = target.GetType();
            var handlers = targetType.GetMethods(flags)
                .Where(h => h.HasCustomAttribute<EasyEventHandlerAttribute>())
                .ToArray();

            Action inUnityThreadSetter = null;
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

                var ret = Register(target, eventType, func);
                inUnityThreadSetter += () => ret.InUnityThread();
            }

            return new FromRegisterEventGeneric(() => UnRegisterSubscriber(target), inUnityThreadSetter);
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        public IFromRegisterEvent Register<TEvent>(object target, EventHandlerDelegate<TEvent> handler)
        {
            return Register(target, typeof(TEvent), handler);
        }

        public IFromRegisterEvent Register(object target, Type eventType, Delegate handler)
        {
            Handlers handlers;
            lock (_handlesDict)
            {
                if (!_handlesDict.TryGetValue(eventType, out handlers))
                {
                    handlers = new Handlers();
                    _handlesDict[eventType] = handlers;
                }
            }

            lock (handlers)
            {
                handlers.AddHandler(target, handler);
            }

            return new FromRegisterEventGeneric(
                () => UnRegister(target, eventType, handler),
                () =>
                {
                    lock (handlers)
                    {
                        handlers.GetHandleExtension(handler).TriggerExtension += UnityInvoke.Invoke;
                    }
                });
        }

        private HandleExtension GetHandleExtension(Type eventType, Delegate handler)
        {
            Handlers handlers;
            lock (_handlesDict)
            {
                handlers = _handlesDict[eventType];
            }

            if (handlers == null)
                return null;

            lock (handlers)
            {
                return handlers.GetHandleExtension(handler);
            }
        }

        #endregion

        #region UnRegister

        /// <summary>
        /// 取消注册target中所有事件处理器
        /// </summary>
        /// <param name="target"></param>
        /// <param name="includeBaseClass">包含基类的事件处理器</param>
        /// <returns></returns>
        public bool UnRegisterSubscriber(object target, bool includeBaseClass = false)
        {
            bool has = false;

            lock (_handlesDict)
            {
                foreach (var handlers in _handlesDict.Values)
                {
                    var suc = handlers.RemoveSubscriber(target);
                    if (!has) has = suc;
                }
            }

            return has;
        }

        /// <summary>
        /// 取消注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool UnRegister<TEvent>(object target, EventHandlerDelegate<TEvent> handler)
        {
            return UnRegister(target, typeof(TEvent), handler);
        }

        public bool UnRegister(object target, Type eventType, Delegate handler)
        {
            Handlers handlers;
            lock (_handlesDict)
            {
                if (!_handlesDict.TryGetValue(eventType, out handlers))
                {
                    return false;
                }
            }

            lock (handlers)
            {
                return handlers.RemoveHandler(target, handler);
            }
        }

        #endregion

        #region Trigger

        /// <summary>
        /// <para>触发事件</para>
        /// <para>注意：在事件处理器中触发事件要谨慎，处理不当可能会死锁</para>
        /// <para>如果出现死锁了，检查以下情况</para>
        /// <para>1、是否事件处理器中再次触发处理的事件（包括整个触发栈）</para>
        /// <para>2、待补充。。。。。。</para>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="eventArg">事件</param>
        /// <returns></returns>
        public bool SendEvent<TEvent>(object target, TEvent eventArg)
        {
            return SendEvent(target, eventArg, typeof(TEvent));
        }

        public bool SendEvent(object target, object eventArg, Type eventType)
        {
            Handlers handlers;
            lock (_handlesDict)
            {
                if (!_handlesDict.TryGetValue(eventType, out handlers))
                {
                    return false;
                }
            }

            lock (handlers)
            {
                handlers.Invoke(target, eventArg);
            }

            return true;
        }

        #endregion
    }
}
