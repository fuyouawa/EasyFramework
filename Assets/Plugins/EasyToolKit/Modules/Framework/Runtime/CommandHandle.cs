using System;
using Cysharp.Threading.Tasks;

namespace EasyToolKit.Framework
{
    public class CommandHandle
    {
        private readonly ICommand _command;
        
        private Action<object> _onCompleted;
        private bool _hookedComplete;

        public CommandHandle(ICommand command)
        {
            _command = command;
        }

        internal void AddCompleteCallback(Action<object> callback)
        {
            if (!_hookedComplete)
            {
                HookComplete();
                _hookedComplete = true;
            }

            _onCompleted += callback;
        }

        internal void AddExceptionHandler(Action<Exception> handler)
        {
            _command.ExceptionHandler += handler;
        }

        private void HookComplete()
        {
            if (_command.Executer != null)
            {
                var executer = _command.Executer;
                _command.Executer = () =>
                {
                    var result = executer();
                    _onCompleted?.Invoke(result);
                    return result;
                };
            }
            else
            {
                var executer = _command.TaskExecuter;
                _command.TaskExecuter = ExecuteAsync;

                async UniTask<object> ExecuteAsync()
                {
                    var result = await executer();
                    _onCompleted?.Invoke(result);
                    return result;
                }
            }
        }
    }

    public static class CommandHandleExtension
    {
        public static CommandHandle OnComplete(this CommandHandle handle, Action<object> callback)
        {
            handle.AddCompleteCallback(callback);
            return handle;
        }

        public static CommandHandle OnComplete<TResult>(this CommandHandle handle, Action<TResult> callback)
        {
            handle.AddCompleteCallback(o => callback((TResult)o));
            return handle;
        }

        public static CommandHandle HandleException(this CommandHandle handle, Action<Exception> exceptionHandler)
        {
            handle.AddExceptionHandler(exceptionHandler);
            return handle;
        }

        public static UniTask<object> WaitForComplete(this CommandHandle handle)
        {
            var tcs = new UniTaskCompletionSource<object>();
            handle.AddCompleteCallback(o => tcs.TrySetResult(o));
            return tcs.Task;
        }

        public static UniTask<TResult> WaitForComplete<TResult>(this CommandHandle handle)
        {
            var tcs = new UniTaskCompletionSource<TResult>();
            handle.AddCompleteCallback(o => tcs.TrySetResult((TResult)o));
            return tcs.Task;
        }
    }
}
