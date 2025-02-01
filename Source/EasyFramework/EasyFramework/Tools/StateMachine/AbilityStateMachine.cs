using System;
using UnityEngine;

namespace EasyFramework
{
    public interface IAbility : IState
    {
        bool IsEnabled { get; }
        void Enable();
        void Disable();

        bool IsInitialized { get; }
        void Initialize();
        void FixedUpdate();
        void LateUpdate();
    }

    public abstract class AbstractAbility : MonoBehaviour, IAbility
    {
        bool IState.Condition()
        {
            return Authorized;
        }

        void IState.Enter()
        {
            OnEnterAbility();
        }

        void IState.Update()
        {
            OnEarlyProcessAbility();
            OnProcessAbility();
        }

        void IAbility.FixedUpdate()
        {
            OnFixedProcessAbility();
        }

        void IAbility.LateUpdate()
        {
            OnLateProcessAbility();
        }

        void IState.Exit()
        {
            OnExitAbility();
        }

        public bool IsInitialized { get; protected set; }

        public bool IsEnabled => gameObject.activeSelf;

        void IAbility.Initialize()
        {
            if (IsInitialized)
                return;
            OnInitializeAbility();
            IsInitialized = true;
        }

        void IAbility.Enable()
        {
            OnEnableAbility();
            gameObject.SetActive(true);
        }

        void IAbility.Disable()
        {
            OnDisableAbility();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 当前能力是否有权限运行(非强制性)
        /// </summary>
        public virtual bool Authorized => true;

        protected virtual void OnInitializeAbility()
        {
        }

        protected virtual void OnEnterAbility()
        {
        }

        protected virtual void OnEarlyProcessAbility()
        {
        }


        protected virtual void OnProcessAbility()
        {
        }

        protected virtual void OnFixedProcessAbility()
        {
        }

        protected virtual void OnLateProcessAbility()
        {
        }

        protected virtual void OnExitAbility()
        {
        }

        protected virtual void OnEnableAbility()
        {
        }

        protected virtual void OnDisableAbility()
        {
        }
    }

    public class AbilityStateMachine<T> : StateMachineBase<T, IAbility>
    {
        public void Initialize()
        {
            foreach (var state in States.Values)
            {
                state.Initialize();
            }
        }

        public void Update()
        {
            if (CurrentState is { IsEnabled: true })
            {
                CurrentState.Update();
            }
        }

        public void FixedUpdate()
        {
            if (CurrentState is { IsEnabled: true })
            {
                CurrentState.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            if (CurrentState is { IsEnabled: true })
            {
                CurrentState.LateUpdate();
            }
        }
    }

    public static class AbilityStateMachineExtension
    {
        public static void UnRegisterDriver<T>(this AbilityStateMachine<T> stateMachine, GameObject target)
        {
            var runner = target.GetComponent<StateMachineDriver>();
            if (runner != null)
            {
                runner.OnUpdate -= stateMachine.Update;
                runner.OnFixedUpdate -= stateMachine.FixedUpdate;
                runner.OnLateUpdate -= stateMachine.LateUpdate;
            }
        }

        public static IUnRegisterConfiguration RegisterDriver<T>(this AbilityStateMachine<T> stateMachine,
            GameObject target)
        {
            var runner = target.GetOrAddComponent<StateMachineDriver>();
            runner.OnUpdate += stateMachine.Update;
            runner.OnFixedUpdate += stateMachine.FixedUpdate;
            runner.OnLateUpdate += stateMachine.LateUpdate;
            return new UnRegisterConfiguration(() => stateMachine.UnRegisterDriver(target));
        }
    }
}
