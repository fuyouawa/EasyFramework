using System;
using System.Linq;
using UnityEngine;

namespace EasyFramework
{
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanInitialize
    {
        bool IsInitialized { get; }
        void Initialize();
    }

    public interface ICanDispose : IDisposable
    {
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface IArchitecture
    {
        void RegisterSystem<T>(T system) where T : ISystem;

        void RegisterModel<T>(T model) where T : IModel;

        T GetSystem<T>() where T : class, ISystem;
        T GetModel<T>() where T : class, IModel;

        void SendCommand<T>(T command) where T : ICommand;

        TResult SendCommand<TResult>(ICommand<TResult> command);

        void SendEvent<T>(T e);

        IFromRegisterEvent RegisterEvent<T>(EventHandlerDelegate<T> onEvent);
        void UnRegisterEvent<T>(EventHandlerDelegate<T> onEvent);
        
        IFromRegisterEvent RegisterEventSubscriber();
        void UnRegisterEventSubscriber();
    }

    public abstract class Architecture<T> : Singleton<T>, IArchitecture
        where T : Architecture<T>, new()
    {
        private readonly DiContainer _container = new DiContainer();

        protected override void OnSingletonInit()
        {
            Initialize();

            foreach (var model in _container.ResolveAll<IModel>()
                         .Where(m => !m.IsInitialized))
            {
                model.Initialize();
            }

            foreach (var system in _container.ResolveAll<ISystem>()
                         .Where(m => !m.IsInitialized))
            {
                system.Initialize();
            }
        }

        protected override void OnSingletonDispose()
        {
            Dispose();

            foreach (var system in _container.ResolveAll<ISystem>().Where(s => s.IsInitialized))
                system.Dispose();
            foreach (var model in _container.ResolveAll<IModel>().Where(m => m.IsInitialized))
                model.Dispose();

            _container.Clear();
        }

        protected abstract void Initialize();

        protected virtual void Dispose()
        {
        }

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            _container.Bind(system);

            if (IsSingletonInitialized)
            {
                system.Initialize();
            }
        }

        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            _container.Bind(model);

            if (IsSingletonInitialized)
            {
                model.Initialize();
            }
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return _container.Resolve<TSystem>();
        }

        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return _container.Resolve<TModel>();
        }

        public TResult SendCommand<TResult>(ICommand<TResult> command)
        {
            return ExecuteCommand(command);
        }

        public void SendCommand<TCommand>(TCommand command) where TCommand : ICommand
        {
            ExecuteCommand(command);
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
            EventManager.Instance.SendEvent(this, e);
        }

        public IFromRegisterEvent RegisterEvent<TEvent>(EventHandlerDelegate<TEvent> onEvent)
        {
            return EventManager.Instance.Register(this, onEvent);
        }

        public void UnRegisterEvent<TEvent>(EventHandlerDelegate<TEvent> onEvent)
        {
            EventManager.Instance.UnRegister(this, onEvent);
        }

        public IFromRegisterEvent RegisterEventSubscriber()
        {
            return EventManager.Instance.RegisterSubscriber(this);
        }

        public void UnRegisterEventSubscriber()
        {
            EventManager.Instance.UnRegisterSubscriber(this);
        }
    }
}
