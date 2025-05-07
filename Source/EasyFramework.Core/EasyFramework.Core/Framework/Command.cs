namespace EasyFramework.Core
{
    public interface ICanSendCommand : IBelongToArchitecture
    {
    }

    public static class CanSendCommandExtension
    {
        public static void SendCommand<T>(this ICanSendCommand self, T command) where T : ICommand
        {
            self.GetArchitecture().SendCommand(command);
        }

        public static void SendCommand<T>(this ICanSendCommand self) where T : class, ICommand, new()
        {
            using var cmd = EasyPool<T>.AllocScope();
            self.SendCommand(cmd.Value);
        }

        public static TResult SendCommand<TResult>(this ICanSendCommand self, ICommand<TResult> command)
        {
            return self.GetArchitecture().SendCommand(command);
        }
    }

    public interface ICommand : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel,
        ICanSendEvent, ICanSendCommand, ICanGetUtility, ICanSendQuery
    {
        void Execute();
    }

    public interface ICommand<TResult> : IBelongToArchitecture, ICanSetArchitecture, ICanGetSystem, ICanGetModel,
        ICanSendEvent, ICanSendCommand, ICanGetUtility, ICanSendQuery
    {
        TResult Execute();
    }

    public abstract class AbstractCommand : ICommand
    {
        private IArchitecture _arch = null!;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;

        void ICommand.Execute() => OnExecute();

        protected abstract void OnExecute();
    }

    public abstract class AbstractCommand<TResult> : ICommand<TResult>
    {
        private IArchitecture _arch;

        IArchitecture IBelongToArchitecture.GetArchitecture() => _arch;

        void ICanSetArchitecture.SetArchitecture(IArchitecture architecture) => _arch = architecture;

        TResult ICommand<TResult>.Execute() => OnExecute();

        protected abstract TResult OnExecute();
    }
}
