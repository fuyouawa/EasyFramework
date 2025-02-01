using System;

namespace EasyFramework
{
    public interface IUnRegisterConfiguration
    {
        void UnRegister();
    }

    public class UnRegisterConfiguration : IUnRegisterConfiguration
    {
        private Action OnUnRegister { get; }
        public UnRegisterConfiguration(Action onUnRegister) => OnUnRegister = onUnRegister;

        public void UnRegister()
        {
            OnUnRegister?.Invoke();
        }
    }
}
