using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

namespace EasyToolKit.Core
{
    #region Define

    /// <summary>
    /// 事件处理器
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件参数</param>
    public delegate void EasyEventHandler<in TEvent>(object sender, TEvent e);

    /// <summary>
    /// <para>事件处理器的触发行为扩展</para>
    /// <para>可以使用该委托实现例如：确保在UI线程调用事件处理器</para>
    /// </summary>
    /// <param name="triggerInvoker">事件处理器的触发调用对象</param>
    public delegate void EventTriggerWrapper(Action triggerInvoker);

    internal class EasyEventHandlers
    {
        private readonly Dictionary<object, HashSet<Delegate>> _handlesByTarget =
            new Dictionary<object, HashSet<Delegate>>();

        private readonly Dictionary<Delegate, EventTriggerWrapper> _eventTriggerWrapperByHandler =
            new Dictionary<Delegate, EventTriggerWrapper>();

        public void AddHandler(object target, Delegate handler)
        {
            if (!_handlesByTarget.TryGetValue(target, out var handlers))
            {
                handlers = new HashSet<Delegate>();
                _handlesByTarget[target] = handlers;
            }

            handlers.Add(handler);
        }

        public void AddTriggerWrapper(Delegate handler, EventTriggerWrapper wrapper)
        {
            if (_eventTriggerWrapperByHandler.TryGetValue(handler, out var w))
            {
                w += wrapper;
            }
            else
            {
                w = wrapper;
            }

            _eventTriggerWrapperByHandler[handler] = w;
        }

        public bool RemoveHandler(object target, Delegate handler)
        {
            if (_handlesByTarget.TryGetValue(target, out var handlers))
            {
                return handlers.Remove(handler);
            }

            return false;
        }

        public bool RemoveSubscriber(object target)
        {
            return _handlesByTarget.Remove(target);
        }

        public void Invoke(object sender, object eventArg)
        {
            foreach (var handlers in _handlesByTarget.Values)
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

            if (_eventTriggerWrapperByHandler.TryGetValue(handler, out var wrapper))
            {
                wrapper?.Invoke(Call);
            }

            Call();
        }
    }

    #endregion

    public class EasyEventManager : Singleton<EasyEventManager>
    {
        private readonly Dictionary<Type, EasyEventHandlers> _handlesDict;

        EasyEventManager()
        {
            _handlesDict = new Dictionary<Type, EasyEventHandlers>();
        }

        #region Register

        /// <summary>
        /// 注册事件处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        public IFromRegisterEvent Register<TEvent>(object target, EasyEventHandler<TEvent> handler)
        {
            return Register(target, typeof(TEvent), handler);
        }

        public IFromRegisterEvent Register(object target, Type eventType, Delegate handler)
        {
            EasyEventHandlers handlers;
            lock (_handlesDict)
            {
                if (!_handlesDict.TryGetValue(eventType, out handlers))
                {
                    handlers = new EasyEventHandlers();
                    _handlesDict[eventType] = handlers;
                }
            }

            lock (handlers)
            {
                handlers.AddHandler(target, handler);
            }

            return new FromRegisterEventGeneric(
                () => Unregister(target, eventType, handler),
                () =>
                {
                    lock (handlers)
                    {
                        handlers.AddTriggerWrapper(handler, UnityMainThreadDispatcher.Instance.Enquence);
                    }
                });
        }

        #endregion

        #region Unregister

        /// <summary>
        /// 取消注册target中所有事件处理器
        /// </summary>
        /// <param name="target"></param>
        /// <param name="includeBaseClass">包含基类的事件处理器</param>
        /// <returns></returns>
        public bool UnregisterSubscriber(object target, bool includeBaseClass = false)
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
        public bool Unregister<TEvent>(object target, EasyEventHandler<TEvent> handler)
        {
            return Unregister(target, typeof(TEvent), handler);
        }

        public bool Unregister(object target, Type eventType, Delegate handler)
        {
            EasyEventHandlers handlers;
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
            EasyEventHandlers handlers;
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
