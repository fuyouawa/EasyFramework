using UnityEngine;

namespace EasyFramework
{
    public abstract class AbstractStateBehaviour : MonoBehaviour, IState
    {
        public bool IsCurrentState { get; protected set; }

        protected bool IsStateEnabled { get; set; }

        bool IState.Enable
        {
            get { return IsStateEnabled; }
            set
            {
                if (IsStateEnabled == value)
                    return;
                IsStateEnabled = value;
                if (IsStateEnabled)
                {
                    OnStateEnable();
                }
                else
                {
                    OnStateDisable();
                }
            }
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
            OnStateUpdate();
        }

        void IState.Exit()
        {
            IsCurrentState = false;
            OnStateExit();
        }

        protected virtual void Update()
        {
            //TODO
        }

        protected virtual void FixedUpdate()
        {
            if (IsCurrentState)
            {
                OnStateFixedUpdate();
            }
        }

        protected virtual void LateUpdate()
        {
            if (IsCurrentState)
            {
                OnStateLateUpdate();
            }
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

        protected virtual void OnStateLateUpdate()
        {
        }

        protected virtual void OnStateEnable()
        {
        }

        protected virtual void OnStateDisable()
        {
        }
    }
}
