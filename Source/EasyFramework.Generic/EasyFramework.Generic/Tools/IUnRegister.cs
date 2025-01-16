using System;

namespace EasyFramework
{
    public interface IUnRegister
    {
        void UnRegister();
    }

    public struct CustomUnRegister : IUnRegister
    {
        private Action OnUnRegister { get; }
        public CustomUnRegister(Action onUnRegister) => OnUnRegister = onUnRegister;

        public void UnRegister()
        {
            OnUnRegister?.Invoke();
        }
    }
}