using System;

namespace EasyFramework
{
    public interface IFromEventRegister : IFromRegister
    {
        /// <summary>
        /// <para>注册事件处理器</para>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        IFromRegister InUnityThread();
    }

    public class FromEventRegister : FromRegister, IFromEventRegister
    {
        private readonly Action _inUnityThreadSetter;

        public FromEventRegister(Action onUnRegister, Action inUnityThreadSetter) : base(onUnRegister)
        {
            _inUnityThreadSetter = inUnityThreadSetter;
        }

        public IFromRegister InUnityThread()
        {
            _inUnityThreadSetter();
            return this;
        }
    }
}
