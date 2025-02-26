using System;

namespace EasyFramework
{
    public interface IUnRegister
    {
        void UnRegister();
    }
    
    public class UnRegisterGeneric : IUnRegister
    {
        private readonly Action _onUnRegister;

        public UnRegisterGeneric(Action onUnRegister)
        {
            _onUnRegister = onUnRegister;
        }

        public void UnRegister()
        {
            _onUnRegister?.Invoke();
        }
    }

}
