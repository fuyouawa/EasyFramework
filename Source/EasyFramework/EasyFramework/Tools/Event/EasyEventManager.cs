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
    [AttributeUsage(AttributeTargets.Method)]
    public class EasyEventHandlerAttribute : Attribute
    {
    }

    #endregion

    public class EasyEventManager : Singleton<EasyEventManager>
    {
        #region Internal

        private class HandlerItem
        {
            public Delegate Handler;
            public EasyEventTriggerExtensionDelegate TriggerExtension;

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

        private class Handlers
        {
            private Dictionary<object, List<HandlerItem>> _handlesDict;

            public void AddHandler(object target, Delegate handler,
                EasyEventTriggerExtensionDelegate triggerExtension)
            {
                if (!_handlesDict.TryGetValue(target, out var handlers))
                {
                    handlers = new List<HandlerItem>();
                    _handlesDict[target] = handlers;
                }

                if (handlers.FirstOrDefault(h => h.Handler == handler) != null)
                {
                    throw new ArgumentException($"You can't register an event handler({handler}) repeatedly");
                }

                handlers.Add(new HandlerItem() { Handler = handler, TriggerExtension = triggerExtension });
            }

            public bool RemoveHandler(object target, Delegate handler)
            {
                if (_handlesDict.TryGetValue(target, out var handlers))
                {
                    int i = 0;
                    for (; i < _handlesDict.Count; i++)
                    {
                        if (handlers[i].Handler == handler)
                        {
                            break;
                        }
                    }

                    if (i < _handlesDict.Count)
                    {
                        handlers.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            public bool RemoveSubscriber(object target)
            {
                return _handlesDict.Remove(target);
            }

            public void Invoke(object sender, object eventArg)
            {
                foreach (var handlers in _handlesDict.Values)
                {
                    foreach (var handler in handlers)
                    {
                        handler.Invoke(sender, eventArg);
                    }
                }
            }
        }

        private readonly Dictionary<Type, Handlers> _handlesDict;

        EasyEventManager()
        {
            _handlesDict = new Dictionary<Type, Handlers>();
        }

        #endregion

        #region Register

        /// <summary>
        /// <para>注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）</para>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        public IUnRegisterConfiguration RegisterSubscriberInUnityThread(object target,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterSubscriber(target, invoker =>
            {
                void Call() => UnityInvoke.Invoke(invoker);
                if (triggerExtension != null)
                {
                    triggerExtension(Call);
                }
                else
                {
                    Call();
                }
            });
        }

        /// <summary>
        /// 注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）
        /// </summary>
        /// <param name="target"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IUnRegisterConfiguration RegisterSubscriber(object target,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var targetType = target.GetType();
            var handlers = targetType.GetMethods(flags)
                .Where(h => h.HasCustomAttribute<EasyEventHandlerAttribute>())
                .ToArray();

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

                RegisterHandler(target, eventType, func, triggerExtension);
            }

            return new UnRegisterConfiguration(() => UnRegisterSubscriber(target));
        }

        /// <summary>
        /// <para>注册事件处理器</para>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        public IUnRegisterConfiguration RegisterHandlerInUnityThread<TEvent>(object target,
            EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterHandler(target, handler, invoker =>
            {
                void Call() => UnityInvoke.Invoke(invoker);
                if (triggerExtension != null)
                {
                    triggerExtension(Call);
                }
                else
                {
                    Call();
                }
            });
        }

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        public IUnRegisterConfiguration RegisterHandler<TEvent>(object target,
            EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
        {
            return RegisterHandler(target, typeof(TEvent), handler, triggerExtension);
        }

        public IUnRegisterConfiguration RegisterHandler(object target, Type eventType,
            Delegate handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null)
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
                handlers.AddHandler(target, handler, triggerExtension);
            }

            return new UnRegisterConfiguration(() => UnRegisterHandler(target, eventType, handler));
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
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool UnRegisterHandler<TEvent>(object target, EasyEventHandlerDelegate<TEvent> handler)
        {
            return UnRegisterHandler(target, typeof(TEvent), handler);
        }

        public bool UnRegisterHandler(object target, Type eventType, Delegate handler)
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
        public bool TriggerEvent<TEvent>(object target, TEvent eventArg)
        {
            return TriggerEvent(target, eventArg, typeof(TEvent));
        }

        public bool TriggerEvent(object target, object eventArg, Type eventType)
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
