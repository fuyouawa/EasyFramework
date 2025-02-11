using System;

namespace EasyFramework
{
    public interface IState
    {
        bool Enable { get; set; }
        bool IsInitialized { get; }

        void Initialize();
        bool Condition();
        void Enter();
        void Update();
        void Exit();
    }

    /// <summary>
    /// 当状态更改完成的回调
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stateId">当前状态</param>
    public delegate void OnStateChangeDelegate<in T>(T stateId);

    public abstract class AbstractState : IState
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
            OnStateUpdate();
        }

        void IState.Exit()
        {
            IsCurrentState = false;
            OnStateExit();
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

        protected virtual void OnStateEnable()
        {
        }

        protected virtual void OnStateDisable()
        {
        }
    }
}
