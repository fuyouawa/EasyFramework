using System;

namespace EasyFramework
{
    public interface ICanGetSystem : IBelongToArchitecture
    {
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem =>
            self.GetArchitecture().GetSystem<T>();
    }

    public interface ISystem : IBelongToArchitecture, ICanSetArchitecture, ICanGetModel,
        ICanRegisterEvent, ICanSendEvent, ICanGetSystem, ICanInitialize, ICanDispose
    {
    }

    public abstract class AbstractSystem : ISystem
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
