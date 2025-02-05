using System;

namespace EasyFramework
{
    public interface IFromRegisterEvent : IFromRegister
    {
        /// <summary>
        /// <para>确保会在Unity线程触发</para>
        /// </summary>
        IFromRegister InUnityThread();
    }

    public class FromRegisterEventGeneric : FromRegisterGeneric, IFromRegisterEvent
    {
        private readonly Action _inUnityThreadSetter;

        public FromRegisterEventGeneric(Action onUnRegister, Action inUnityThreadSetter) : base(onUnRegister)
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
