using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyGameFramework;

namespace EasyGameFramework
{
    public static class UnityInvoker
    {
        public static Task InvokeAsync(Action action)
        {
            return UnityMainThreadDispatcher.Instance.EnqueueAsync(action);
        }
        public static void Invoke(Action action)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(action);
        }

        public static void Invoke(IEnumerator action)
        {
            UnityMainThreadDispatcher.Instance.Enqueue(action);
        }
    }

    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>
    {
        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        public void Update() {
            lock(_executionQueue) {
                while (_executionQueue.Count > 0) {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(IEnumerator action) {
            lock (_executionQueue) {
                _executionQueue.Enqueue (() => {
                    StartCoroutine (action);
                });
            }
        }

        public void Enqueue(Action action)
        {
            Enqueue(ActionWrapper(action));
        }

        public Task EnqueueAsync(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            void WrappedAction() {
                try 
                {
                    action();
                    tcs.TrySetResult(true);
                } catch (Exception ex) 
                {
                    tcs.TrySetException(ex);
                }
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }


        IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }
    }
}
