using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyFramework
{
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanInitializeAsync
    {
        bool IsInitialized { get; }
        UniTask InitializeAsync();
        UniTask DeinitializeAsync();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface IArchitecture
    {
        internal static readonly List<IArchitecture> Architectures = new List<IArchitecture>();
        static IArchitecture[] GetArchitectures() => Architectures.ToArray();

        int Index { get; }

        UniTask InitializeAsync();
        UniTask DeinitializeAsync();

        void RegisterSystem<T>(T system) where T : ISystem;
        void RegisterModel<T>(T model) where T : IModel;
        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetSystem<T>() where T : class, ISystem;
        T GetModel<T>() where T : class, IModel;
        T GetUtility<T>() where T : class, IUtility;

        ISystem[] GetAllSystems();
        IModel[] GetAllModels();
        IUtility[] GetAllUtilities();

        void SendCommand(ICommand command);
        TResult SendCommand<TResult>(ICommand<TResult> command);
        TResult SendQuery<TResult>(IQuery<TResult> query);

        void SendEvent<T>(T e);

        IFromRegisterEvent RegisterEvent<T>(EasyEventHandler<T> onEvent);
        void UnregisterEvent<T>(EasyEventHandler<T> onEvent);
    }

    public enum ArchitectureState
    {
        UnInitialize,
        Initializing,
        Initialized,
        Deinitializing,
        Deinitialized
    }

    public abstract class Architecture<T> : Singleton<T>, IArchitecture
        where T : Architecture<T>, new()
    {
        private readonly DiContainer _container = new DiContainer();

        public ArchitectureState State { get; private set; }

        public int Index { get; private set; }

        protected override void OnSingletonInit()
        {
            Index = IArchitecture.Architectures.Count;
            IArchitecture.Architectures.Add(this);
        }

        protected override void OnSingletonDispose()
        {
            IArchitecture.Architectures[Index] = null;
            Index = -1;
        }

        public async UniTask InitializeAsync()
        {
            if (State != ArchitectureState.UnInitialize && State != ArchitectureState.Deinitialized)
                return;

            State = ArchitectureState.Initializing;
            await OnInitAsync();

            var tasks = new List<UniTask>();
            foreach (var model in _container.ResolveAll<IModel>()
                         .Where(m => !m.IsInitialized))
            {
                tasks.Add(model.InitializeAsync());
            }

            foreach (var system in _container.ResolveAll<ISystem>()
                         .Where(m => !m.IsInitialized))
            {
                tasks.Add(system.InitializeAsync());
            }

            await UniTask.WhenAll(tasks);
            State = ArchitectureState.Initialized;
        }

        public async UniTask DeinitializeAsync()
        {
            if (State != ArchitectureState.Initialized)
                return;

            State = ArchitectureState.Deinitializing;
            await OnDeinitAsync();

            var tasks = new List<UniTask>();
            foreach (var system in _container.ResolveAll<ISystem>().Where(s => s.IsInitialized))
            {
                tasks.Add(system.DeinitializeAsync());
            }

            foreach (var model in _container.ResolveAll<IModel>().Where(m => m.IsInitialized))
            {
                tasks.Add(model.DeinitializeAsync());
            }

            await UniTask.WhenAll(tasks);
            _container.Clear();
            State = ArchitectureState.Deinitialized;
        }


        protected abstract UniTask OnInitAsync();

        protected virtual UniTask OnDeinitAsync() => UniTask.CompletedTask;

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            CheckRegistrable();
            system.SetArchitecture(this);
            _container.Bind(system);
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            CheckRegistrable();
            model.SetArchitecture(this);
            _container.Bind(model);
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility
        {
            _container.Bind(utility);
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return CheckNull(_container.Resolve<TSystem>());
        }

        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return CheckNull(_container.Resolve<TModel>());
        }

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return CheckNull(_container.Resolve<TUtility>());
        }

        public ISystem[] GetAllSystems()
        {
            return _container.ResolveAll<ISystem>().ToArray();
        }

        public IModel[] GetAllModels()
        {
            return _container.ResolveAll<IModel>().ToArray();
        }

        public IUtility[] GetAllUtilities()
        {
            return _container.ResolveAll<IUtility>().ToArray();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
            return ExecuteCommand(command);
        }

        public void SendCommand(ICommand command)
        {
            ExecuteCommand(command);
        }

        public TResult SendQuery<TResult>(IQuery<TResult> query)
        {
            return DoQuery<TResult>(query);
        }

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }

        protected virtual TResult ExecuteCommand<TResult>(ICommand<TResult> command)
        {
            command.SetArchitecture(this);
            return command.Execute();
        }

        protected virtual void ExecuteCommand(ICommand command)
        {
            command.SetArchitecture(this);
            command.Execute();
        }


        public void SendEvent<TEvent>(TEvent e)
        {
            EasyEventManager.Instance.SendEvent(this, e);
        }

        public IFromRegisterEvent RegisterEvent<TEvent>(EasyEventHandler<TEvent> onEvent)
        {
            return EasyEventManager.Instance.Register(this, onEvent);
        }

        public void UnregisterEvent<TEvent>(EasyEventHandler<TEvent> onEvent)
        {
            EasyEventManager.Instance.Unregister(this, onEvent);
        }

        private T CheckNull<T>(T val) where T : class
        {
            if (val == null)
            {
                throw new InvalidOperationException(
                    $"The type '{typeof(T)}' " +
                    $"is not registered to architecture '{GetType()}'");
            }

            return val;
        }

        private void CheckRegistrable()
        {
            if (State == ArchitectureState.Initialized)
            {
                throw new InvalidOperationException(
                    $"The architecture '{GetType()}' has been initialized, you can no longer register anything.");
            }
        }
    }
}
