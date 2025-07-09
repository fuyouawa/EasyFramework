using System;
using Cysharp.Threading.Tasks;

namespace EasyToolKit.Framework
{
    public interface ICanSendCommand : IBelongToArchitecture
    {
    }

    public static class CanSendCommandExtension
    {
        public static CommandHandle SendCommand(this ICanSendCommand self, ICommand command)
        {
            self.GetArchitecture().SendCommand(command);
            return new CommandHandle(command);
        }
    }

    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel,
        ICanSendEvent, ICanSendCommand, ICanGetUtility, ICanSendQuery
    {
        Action<Exception> ExceptionHandler { get; set; }
        Func<object> Executer { get; set; }
        Func<UniTask<object>> TaskExecuter { get; set; }
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture _arch = null!;
        private Func<object> _executer;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;
        Action<Exception> ICommand.ExceptionHandler { get; set; }

        Func<object> ICommand.Executer
        {
            get => _executer ??= Execute;
            set => _executer = value;
        }

        Func<UniTask<object>> ICommand.TaskExecuter
        {
            get => null;
            set { }
        }

        private object Execute()
        {
            OnExecute();
            return null;
        }

        protected abstract void OnExecute();
    }

    public abstract class AbstractCommand<TResult> : ICommand
    {
        private IArchitecture _arch = null!;
        private Func<object> _executer;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;
        Action<Exception> ICommand.ExceptionHandler { get; set; }

        Func<object> ICommand.Executer
        {
            get => _executer ??= Execute;
            set => _executer = value;
        }

        Func<UniTask<object>> ICommand.TaskExecuter
        {
            get => null;
            set { }
        }

        private object Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }


    public abstract class AbstractCommandAsync : ICommand
    {
        private IArchitecture _arch = null!;
        private Func<UniTask<object>> _taskExecuter;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;
        Action<Exception> ICommand.ExceptionHandler { get; set; }

        Func<object> ICommand.Executer
        {
            get => null;
            set { }
        }

        Func<UniTask<object>> ICommand.TaskExecuter
        {
            get => _taskExecuter ?? ExecuteAsync;
            set => _taskExecuter = value;
        }
        
        private async UniTask<object> ExecuteAsync()
        {
            await OnExecuteAsync();
            return null;
        }

        protected abstract UniTask OnExecuteAsync();
    }

    public abstract class AbstractCommandAsync<TResult> : ICommand
    {
        private IArchitecture _arch = null!;
        private Func<UniTask<object>> _taskExecuter;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;
        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;
        Action<Exception> ICommand.ExceptionHandler { get; set; }

        Func<object> ICommand.Executer
        {
            get => null;
            set { }
        }

        Func<UniTask<object>> ICommand.TaskExecuter
        {
            get => _taskExecuter ?? ExecuteAsync;
            set => _taskExecuter = value;
        }

        private async UniTask<object> ExecuteAsync() => await OnExecuteAsync();

        protected abstract UniTask<TResult> OnExecuteAsync();
    }
}
