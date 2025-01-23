using System;
using System.Collections;
using System.Collections.Generic;
using EasyFramework.Generic;
using EasyFramework.Utilities;

namespace EasyFramework.Tools
{
    public struct ApplicationQuitEvent
    {
    }

    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>, IUnityInvoker, IEasyEventDispatcher
    {
        private static readonly Queue<Action> s_executionQueue = new Queue<Action>();

        public void Update()
        {
            lock (s_executionQueue)
            {
                while (s_executionQueue.Count > 0)
                {
                    s_executionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(IEnumerator action)
        {
            lock (s_executionQueue)
            {
                s_executionQueue.Enqueue(() => { StartCoroutine(action); });
            }
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            this.TriggerEasyEvent(new ApplicationQuitEvent());
        }
    }
}
