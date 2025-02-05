using System;

namespace EasyFramework
{
    public interface IUnRegister
    {
        void UnRegister();
    }
    
    public class CustomUnRegister : IUnRegister
    {
        private readonly Action _onUnRegister;

        public CustomUnRegister(Action onUnRegister)
        {
            _onUnRegister = onUnRegister;
        }

        public void UnRegister()
        {
            _onUnRegister?.Invoke();
        }
    }

}
