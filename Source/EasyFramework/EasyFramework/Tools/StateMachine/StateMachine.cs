using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework
{
    public class StateMachine<T>
    {
        public T CurrentStateId { get; protected set; }
        public T PreviousStateId { get; protected set; }

        public IState CurrentState { get; protected set; }
        public IState PreviousState { get; protected set; }

        public bool Active { get; protected set; }
        public bool IsInitialized { get; protected set; }

        protected readonly Dictionary<T, IState> States = new Dictionary<T, IState>();

        /// <summary>
        /// 当有状态更改时的回调
        /// </summary>
        public event OnStateChangeDelegate<T> OnStateChange;

        public StateMachine()
        {
        }

        public void Initialize()
        {
            if (IsInitialized)
                return;

            foreach (var state in States.Values)
            {
                state.Initialize();
            }
        }

        public IState[] GetStates()
        {
            return States.Values.ToArray();
        }

        public virtual void Update()
        {
            if (!Active)
                return;

            if (CurrentState is { Enable: true })
            {
                CurrentState.Update();
            }
        }

        public virtual void SetActive(bool active)
        {
            if (Active == active)
                return;
            Active = active;

            foreach (var state in States.Values)
            {
                state.Enable = active;
            }
        }

        public virtual IState GetState(T stateId)
        {
            return States.GetValueOrDefault(stateId);
        }

        public virtual void AddState(T stateId, IState state)
        {
            States[stateId] = state;
            if (IsInitialized && state.IsInitialized)
            {
                state.Initialize();
            }
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

            PreviousState = CurrentState;
            if (States.TryGetValue(stateId, out var state))
            {
                CurrentState = state;
                CurrentState.Enter();
            }
            else
            {
                CurrentState = null;
            }

            OnStateChange?.Invoke(stateId);
            return true;
        }
    }
}
