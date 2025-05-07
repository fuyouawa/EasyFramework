using System;

namespace EasyFramework.Core
{
    public interface IState
    {
        bool IsInitialized { get; }

        void Initialize();
        bool Condition();
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }

    /// <summary>
    /// 当状态更改完成的回调
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stateId">当前状态</param>
    public delegate void StateChangedHandler<in T>(T stateId);

    public abstract class AbstractState : IState
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
            OnStateUpdate();
        }

        void IState.FixedUpdate()
        {
            OnStateFixedUpdate();
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

        protected virtual void OnStateFixedUpdate()
        {
        }
    }
}
