using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyFramework
{
    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>, IUnityInvoker
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        void Awake()
        {
            UnityInvoke.Invoker = this;
        }

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
    }
}
