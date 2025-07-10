using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyToolKit.Framework
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
