using System;

namespace EasyFramework.Core
{
    [MonoSingletonConfig(MonoSingletonFlags.DontDestroyOnLoad)]
    public class UnityMainThreadDispatcher : MonoSingleton<UnityMainThreadDispatcher>
    {
        private readonly object _lock = new object();
        private Action _pendingActions;

        private UnityMainThreadDispatcher()
        {
        }

        public void Enquence(Action action)
        {
            lock (_lock)
            {
                _pendingActions += action;
            }
        }

        void Update()
        {
            Action temp;
            lock (_lock)
            {
                temp = _pendingActions;
            }

            temp?.Invoke();
        }
    }
}
