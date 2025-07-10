using Cysharp.Threading.Tasks;
using System;

namespace EasyToolKit.Framework
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

        public bool Initialized { get; protected set; }

        private bool _isDoing = false;

        async UniTask ICanInitializeAsync.InitializeAsync()
        {
            if (Initialized || _isDoing)
                return;

            _isDoing = true;
            await OnInitializeAsync();
            _isDoing = false;

            Initialized = true;
        }

        async UniTask ICanInitializeAsync.DeinitializeAsync()
        {
            if (!Initialized || _isDoing)
                return;

            _isDoing = true;
            await OnDeinitializeAsync();
            _isDoing = false;
            
            _arch = null;
            Initialized = false;
        }

        protected virtual UniTask OnInitializeAsync() => UniTask.CompletedTask;
        protected virtual UniTask OnDeinitializeAsync() => UniTask.CompletedTask;
    }
}
