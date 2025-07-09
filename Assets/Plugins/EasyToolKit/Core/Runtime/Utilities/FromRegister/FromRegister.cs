using System;

namespace EasyToolKit.Core
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
