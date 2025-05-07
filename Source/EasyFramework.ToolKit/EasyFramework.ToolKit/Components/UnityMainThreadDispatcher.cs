using System;
using System.Collections;
using System.Collections.Generic;
using EasyFramework.Core;

namespace EasyFramework.ToolKit
{
    [MonoSingletonConfig(MonoSingletonFlags.DontDestroyOnLoad)]
    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>, IUnityInvoker
    {
        private static readonly Queue<Action> ExecutionQueue = new Queue<Action>();

        protected override void Awake()
        {
            base.Awake();
            
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
