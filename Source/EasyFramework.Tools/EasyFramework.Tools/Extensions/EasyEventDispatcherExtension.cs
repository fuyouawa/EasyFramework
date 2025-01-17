using EasyFramework.Generic;

namespace EasyFramework.Tools
{
    public static class EasyEventDispatcherExtension
    {
        /// <summary>
        /// <para>注册target中所有事件处理器（标记了EasyEventHandler特性的成员函数）</para>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="includeBaseClass">包含基类的事件处理器</param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        /// <returns></returns>
        public static IUnRegister RegisterEasyEventSubscriberInUnityThread<T>(this T target,
            bool includeBaseClass = false,
            EasyEventTriggerExtensionDelegate triggerExtension = null) where T : IEasyEventDispatcher
        {
            return target.RegisterEasyEventSubscriber(includeBaseClass, invoker =>
            {
                void Call() => UnityInvoker.Invoke(invoker);
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
        /// <para>注册事件处理器</para>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="handler"></param>
        /// <param name="triggerExtension">事件处理器的触发行为扩展</param>
        public static IUnRegister RegisterEasyEventHandlerInUnityThread<T, TEvent>(this T target,
            EasyEventHandlerDelegate<TEvent> handler,
            EasyEventTriggerExtensionDelegate triggerExtension = null) where T : IEasyEventDispatcher
        {
            return target.RegisterEasyEventHandler(handler, invoker =>
            {
                void Call() => UnityInvoker.Invoke(invoker);
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
    }
}
