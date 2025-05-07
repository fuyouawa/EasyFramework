using UnityEngine;

namespace EasyFramework.Core
{
    public abstract class AbstractStateBehaviour : MonoBehaviour, IState
    {
        public bool IsCurrentState { get; protected set; }

        public bool IsInitialized { get; protected set; }

        void IState.Initialize()
        {
            if (IsInitialized)
                return;
            OnStateInit();
            IsInitialized = true;
        }

        bool IState.Condition()
        {
            return OnStateCondition();
        }

        void IState.Enter()
        {
            IsCurrentState = true;
            OnStateEnter();
        }

        void IState.Update()
        {
            if (gameObject.activeSelf)
            {
                OnStateUpdate();
            }
        }

        void IState.Exit()
        {
            IsCurrentState = false;
            OnStateExit();
        }

        void IState.FixedUpdate()
        {
            OnStateFixedUpdate();
        }

        protected virtual void OnStateInit()
        {
        }

        protected virtual bool OnStateCondition()
        {
            return true;
        }

        protected virtual void OnStateEnter()
        {
        }

        protected virtual void OnStateExit()
        {
        }

        protected virtual void OnStateUpdate()
        {
        }

        protected virtual void OnStateFixedUpdate()
        {
        }
    }
}
