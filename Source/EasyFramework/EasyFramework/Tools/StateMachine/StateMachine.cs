using System.Collections.Generic;
using UnityEngine;

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

    public class StateMachine<T>
    {
        public T CurrentStateId { get; protected set; }
        public T PreviousStateId { get; protected set; }

        public IState CurrentState { get; protected set; }

        public Dictionary<T, IState> States { get; } = new Dictionary<T, IState>();

        /// <summary>
        /// 当有状态更改时的回调
        /// </summary>
        public event OnStateChangeDelegate<T> OnStateChange;

        public StateMachine()
        {
        }

        public virtual void Update()
        {
            CurrentState?.Update();
        }

        public IState GetState(T stateId)
        {
            return States.GetValueOrDefault(stateId);
        }

        public void AddState(T stateId, IState state)
        {
            States[stateId] = state;
        }


        public virtual bool ChangeState(T stateId)
        {
            if (EqualityComparer<T>.Default.Equals(stateId, CurrentStateId))
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
