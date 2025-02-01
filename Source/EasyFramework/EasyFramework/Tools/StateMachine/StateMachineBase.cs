using System;
using System.Collections.Generic;

namespace EasyFramework
{
    public interface IState
    {
        bool Condition();
        void Enter();
        void Update();
        void Exit();
    }

    public delegate void OnStateChangeDelegate<in T>(T previous, T current);

    public class StateMachineBase<TStateId, TState>
        where TState : IState
    {
        public TStateId CurrentStateId { get; protected set; }
        public TStateId PreviousStateId { get; protected set; }

        public TState CurrentState { get; protected set; }

        protected readonly Dictionary<TStateId, TState> States = new Dictionary<TStateId, TState>();

        /// <summary>
        /// 当有状态更改时的回调
        /// </summary>
        public event OnStateChangeDelegate<TStateId> OnStateChange;

        public StateMachineBase()
        {
        }

        public virtual void Update()
        {
            if (CurrentState != null)
            {
                CurrentState.Update();
            }
        }

        public TState GetState(TStateId stateId)
        {
            return States.GetValueOrDefault(stateId);
        }

        public void AddState(TStateId stateId, TState state)
        {
            States[stateId] = state;
        }

        public virtual bool ChangeState(TStateId stateId)
        {
            if (EqualityComparer<TStateId>.Default.Equals(stateId, CurrentStateId))
            {
                return true;
            }

            if (CurrentState?.Condition() == false)
            {
                return false;
            }

            PreviousStateId = CurrentStateId;
            CurrentStateId = stateId;

            CurrentState?.Exit();
            if (States.TryGetValue(stateId, out var state))
            {
                CurrentState = state;
                CurrentState.Enter();
            }

            OnStateChange?.Invoke(PreviousStateId, stateId);
            return true;
        }
    }
}
