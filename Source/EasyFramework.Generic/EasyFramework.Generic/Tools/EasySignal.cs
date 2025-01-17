using System;

namespace EasyFramework.Generic
{
    //TODO EasySignal
    public class EasySignal
    {
        private Action _handler;
        
        public IUnRegister Register(Action handler)
        {
            _handler += handler;
            return new CustomUnRegister(() => UnRegister(handler));
        }

        public void UnRegister(Action handler)
        {
            _handler -= handler;
        }

        public void Trigger()
        {
            _handler?.Invoke();
        }
    }
}