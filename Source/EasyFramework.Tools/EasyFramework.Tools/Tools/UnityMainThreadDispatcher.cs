using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyFramework.Generic;
using EasyFramework.Utilities;

namespace EasyFramework.Tools
{
    public struct ApplicationQuitEvent {}

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

    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>, IEasyEventDispatcher
    {
        private static readonly Queue<Action> s_executionQueue = new Queue<Action>();

        public void Update() {
            lock(s_executionQueue) {
                while (s_executionQueue.Count > 0) {
                    s_executionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(IEnumerator action) {
            lock (s_executionQueue) {
                s_executionQueue.Enqueue (() => {
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

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            this.TriggerEasyEvent(new ApplicationQuitEvent());
        }
    }
}
