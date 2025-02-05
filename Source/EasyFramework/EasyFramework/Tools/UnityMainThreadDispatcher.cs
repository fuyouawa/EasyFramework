using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyFramework
{
    public struct ApplicationQuitEvent
    {
    }

    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>, IUnityInvoker
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        public void Update()
        {
            lock (ExecutionQueue)
            {
                while (ExecutionQueue.Count > 0)
                {
                    ExecutionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(IEnumerator action)
        {
            lock (ExecutionQueue)
            {
                ExecutionQueue.Enqueue(() => { StartCoroutine(action); });
            }
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            EventManager.Instance.SendEvent(this, new ApplicationQuitEvent());
        }
    }
}
