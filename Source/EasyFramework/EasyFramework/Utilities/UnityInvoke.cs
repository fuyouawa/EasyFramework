using System.Threading.Tasks;
using System;
using System.Collections;

namespace EasyFramework
{
    public interface IUnityInvoker
    {
        public void Enqueue(IEnumerator action);
    }

    public static class UnityInvoke
    {
        public static IUnityInvoker Invoker { get; set; }

        public static Task InvokeAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction()
            {
                try
                {
                    action();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }

            Invoker.Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }

        public static void Invoke(Action action)
        {
            Invoker.Enqueue(ActionWrapper(action));
        }

        public static void Invoke(IEnumerator action)
        {
            Invoker.Enqueue(action);
        }

        private static IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }

    }
}
