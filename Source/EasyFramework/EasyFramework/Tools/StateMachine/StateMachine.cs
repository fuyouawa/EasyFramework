using System;
using UnityEngine;

namespace EasyFramework
{
    public class StateMachine<T> : StateMachineBase<T, IState>
    {
    }

    public class StateMachineDriver : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnFixedUpdate;
        public event Action OnLateUpdate;

        void Update()
        {
            OnUpdate?.Invoke();
        }

        void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }

        void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
    }


    public static class StateMachineExtension
    {
        public static void UnRegisterDriver<T>(this StateMachine<T> stateMachine, GameObject target)
        {
            var runner = target.GetComponent<StateMachineDriver>();
            if (runner != null)
            {
                runner.OnUpdate -= stateMachine.Update;
            }
        }

        public static IUnRegisterConfiguration RegisterDriver<T>(this StateMachine<T> stateMachine,
            GameObject target)
        {
            var runner = target.GetOrAddComponent<StateMachineDriver>();
            runner.OnUpdate += stateMachine.Update;
            return new UnRegisterConfiguration(() => stateMachine.UnRegisterDriver(target));
        }
    }
}
