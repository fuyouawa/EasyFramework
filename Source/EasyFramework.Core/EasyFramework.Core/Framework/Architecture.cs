using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace EasyFramework.Core
{
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

        private readonly List<ISystem> _pendingInitSystems = new List<ISystem>();
        private readonly List<IModel> _pendingInitModels = new List<IModel>();
        private readonly List<ISystem> _pendingDeinitSystems = new List<ISystem>();
        private readonly List<IModel> _pendingDeinitModels = new List<IModel>();

        public ArchitectureState State { get; private set; }

        public int Index { get; private set; }
        
        /// <inheritdoc />
        protected override void OnSingletonInit()
        {
            Index = IArchitecture.Architectures.Count;
            IArchitecture.Architectures.Add(this);
        }
        
        /// <inheritdoc />
        protected override void OnSingletonDispose()
        {
            IArchitecture.Architectures[Index] = null;
            Index = -1;
        }
        
        /// <inheritdoc />
        public async UniTask InitializeAsync()
        {
            if (State != ArchitectureState.UnInitialize && State != ArchitectureState.Deinitialized)
                return;

            State = ArchitectureState.Initializing;
            await OnInitAsync();

            var tasks = new List<UniTask>();
            foreach (var model in _container.ResolveAll<IModel>())
            {
                Assert.False(model.IsInitialized);
                tasks.Add(model.InitializeAsync());
            }

            foreach (var system in _container.ResolveAll<ISystem>())
            {
                Assert.False(system.IsInitialized);
                tasks.Add(system.InitializeAsync());
            }

            await UniTask.WhenAll(tasks);
            State = ArchitectureState.Initialized;
        }
        
        /// <inheritdoc />
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
        
        /// <inheritdoc />
        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            CheckRegistrable();
            system.SetArchitecture(this);
            _container.Bind(system);
        }
        
        /// <inheritdoc />
        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            CheckRegistrable();
            model.SetArchitecture(this);
            _container.Bind(model);
        }
        
        /// <inheritdoc />
        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility
        {
            _container.Bind(utility);
        }
        
        /// <inheritdoc />
        public UniTask IncrementalInitializeAsync()
        {
            if (State != ArchitectureState.Initialized)
            {
                throw new InvalidOperationException($"The architecture '{GetType()}' must initialize first.");
            }

            var tasks = new List<UniTask>();
            foreach (var system in _pendingInitSystems)
            {
                tasks.Add(system.InitializeAsync());
            }
            foreach (var model in _pendingInitModels)
            {
                tasks.Add(model.InitializeAsync());
            }

            _pendingInitSystems.Clear();
            _pendingInitModels.Clear();
            return UniTask.WhenAll(tasks);
        }
        
        /// <inheritdoc />
        public UniTask IncrementalDeinitializeAsync()
        {
            if (State != ArchitectureState.Initialized)
            {
                throw new InvalidOperationException($"The architecture '{GetType()}' must initialize first.");
            }
            

            var tasks = new List<UniTask>();
            foreach (var system in _pendingDeinitSystems)
            {
                tasks.Add(system.DeinitializeAsync());
            }
            foreach (var model in _pendingDeinitModels)
            {
                tasks.Add(model.DeinitializeAsync());
            }

            _pendingDeinitSystems.Clear();
            _pendingDeinitModels.Clear();
            return UniTask.WhenAll(tasks);
        }

        /// <inheritdoc />
        public void DynamicRegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            _container.Bind(system);
            _pendingInitSystems.Add(system);
        }
        
        /// <inheritdoc />
        public void DynamicUnregisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            _container.Unbind<TSystem>();
            _pendingDeinitSystems.Add(system);
        }
        
        /// <inheritdoc />
        public void DynamicRegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            _container.Bind(model);
            _pendingInitModels.Add(model);
        }
        
        /// <inheritdoc />
        public void DynamicUnregisterModel<TModel>(TModel model) where TModel : IModel
        {
            _container.Unbind<TModel>();
            _pendingDeinitModels.Add(model);
        }
        
        /// <inheritdoc />
        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return CheckNull(_container.Resolve<TSystem>());
        }
        
        /// <inheritdoc />
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return CheckNull(_container.Resolve<TModel>());
        }
        
        /// <inheritdoc />
        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return CheckNull(_container.Resolve<TUtility>());
        }
        
        /// <inheritdoc />
        public ISystem[] GetAllSystems()
        {
            return _container.ResolveAll<ISystem>().ToArray();
        }
        
        /// <inheritdoc />
        public IModel[] GetAllModels()
        {
            return _container.ResolveAll<IModel>().ToArray();
        }
        
        /// <inheritdoc />
        public IUtility[] GetAllUtilities()
        {
            return _container.ResolveAll<IUtility>().ToArray();
        }
        
        /// <inheritdoc />
        public void SendCommand(ICommand command)
        {
            UnityMainThreadDispatcher.Instance.Enquence(() =>
            {
                if (command.Executer != null)
                {
                    try
                    {
                        command.Executer();
                    }
                    catch (Exception e)
                    {
                        if (command.ExceptionHandler != null)
                        {
                            command.ExceptionHandler(e);
                            return;
                        }

                        throw;
                    }
                }
                else
                {
                    command.TaskExecuter().Forget(command.ExceptionHandler);
                }
            });
        }
        
        /// <inheritdoc />
        public TResult SendQuery<TResult>(IQuery<TResult> query)
        {
            return DoQuery<TResult>(query);
        }

        protected virtual TResult DoQuery<TResult>(IQuery<TResult> query)
        {
            query.SetArchitecture(this);
            return query.Do();
        }
        
        /// <inheritdoc />
        public void SendEvent<TEvent>(TEvent e)
        {
            EasyEventManager.Instance.SendEvent(this, e);
        }
        
        /// <inheritdoc />
        public IFromRegisterEvent RegisterEvent<TEvent>(EasyEventHandler<TEvent> onEvent)
        {
            return EasyEventManager.Instance.Register(this, onEvent);
        }
        
        /// <inheritdoc />
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
