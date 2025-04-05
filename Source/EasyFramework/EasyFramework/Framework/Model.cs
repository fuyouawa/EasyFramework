using Cysharp.Threading.Tasks;
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

    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, ICanSendEvent, ICanInitializeAsync,
        ICanGetUtility
    {
    }

    public abstract class AbstractModel : IModel
    {
        private IArchitecture _arch;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;

        public bool IsInitialized { get; protected set; }

        private bool _isDoing = false;

        async UniTask ICanInitializeAsync.InitializeAsync()
        {
            if (IsInitialized || _isDoing)
                return;

            _isDoing = true;
            await OnInitAsync();
            _isDoing = false;

            IsInitialized = true;
        }

        async UniTask ICanInitializeAsync.DeinitializeAsync()
        {
            if (!IsInitialized || _isDoing)
                return;

            _isDoing = true;
            await OnDeinitAsync();
            _isDoing = false;

            IsInitialized = false;
        }

        protected virtual UniTask OnInitAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnDeinitAsync() => UniTask.CompletedTask;
    }
}
