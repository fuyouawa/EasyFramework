using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

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
        ICanRegisterEvent, ICanSendEvent, ICanGetSystem, ICanInitializeAsync, ICanGetUtility
    {
    }

    public abstract class AbstractSystem : ISystem
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
