using System;

namespace EasyToolKit.Core
{
    public interface IUnregister
    {
        void Unregister();
    }
    
    public class UnregisterGeneric : IUnregister
    {
        private readonly Action _onUnregister;

        public UnregisterGeneric(Action onUnregister)
        {
            _onUnregister = onUnregister;
        }

        public void Unregister()
        {
            _onUnregister?.Invoke();
        }
    }

}
