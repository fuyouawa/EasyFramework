using System;

namespace EasyFramework.Core
{
    public interface IFromRegister : IUnregister
    {
    }

    public class FromRegisterGeneric : UnregisterGeneric, IFromRegister
    {
        public FromRegisterGeneric(Action onUnregister) : base(onUnregister)
        {
        }
    }
}
