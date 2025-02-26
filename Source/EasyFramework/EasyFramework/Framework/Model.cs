using System;

namespace EasyFramework
{
    public interface ICanGetModel : IBelongToArchitecture
    {
    }

    public static class CanGetModelExtension
    {
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel =>
            self.GetArchitecture().GetModel<T>();
    }

    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanSendEvent, ICanInitialize, ICanDispose, ICanGetUtility
    {
    }

    public abstract class AbstractModel : IModel
    {
        private IArchitecture _arch;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;

        public bool IsInitialized { get; protected set; }

        void ICanInitialize.Initialize()
        {
            Initialize();
            IsInitialized = true;
        }

        void IDisposable.Dispose()
        {
            Dispose();
            IsInitialized = false;
        }

        protected abstract void Initialize();

        protected virtual void Dispose()
        {
        }
    }
}
