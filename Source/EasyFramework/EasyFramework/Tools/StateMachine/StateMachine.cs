using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework
{
    public class StateMachine<T> : IEnumerable<KeyValuePair<T, IState>>
    {
        public T CurrentStateId { get; protected set; }
        public T PreviousStateId { get; protected set; }

        public IState CurrentState { get; protected set; }
        public IState PreviousState { get; protected set; }

        public bool Active { get; protected set; }
        public bool IsInitialized { get; protected set; }

        protected readonly Dictionary<T, IState> States = new Dictionary<T, IState>();

        /// <summary>
        /// 当状态更改完的的回调
        /// </summary>
        public event OnStateChangeDelegate<T> OnStateChanged;

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
            CheckCurrentState();

            CurrentState.Update();
        }

        public virtual void FixedUpdate()
        {
            if (!Active)
                return;
            CheckCurrentState();

            CurrentState.FixedUpdate();
        }

        public virtual void SetActive(bool active)
        {
            if (Active == active)
                return;
            Active = active;
        }

        public virtual IState GetState(T stateId)
        {
            return States.GetValueOrDefault(stateId);
        }

        public virtual void SetState(T stateId, IState state)
        {
            // if (!States.TryAdd(stateId, state))
            // {
            //     throw new InvalidOperationException(
            //         $"The state id '{stateId}' already has a corresponding state '{state.GetType().Name}'");
            // }
            States[stateId] = state;
            if (IsInitialized && state.IsInitialized)
            {
                state.Initialize();
            }
        }


        public virtual IFluentState FluentState(T stateId)
        {
            if (States.TryGetValue(stateId, out var state))
            {
                if (state.GetType().HasInterface(typeof(IFluentState)))
                {
                    return (IFluentState)state;
                }

                throw new InvalidOperationException(
                    $"The state id '{stateId}' already has a corresponding state '{state.GetType().Name}', " +
                    $"and it's not a fluent state.");
            }

            var fluentState = new FluentState();
            SetState(stateId, fluentState);
            return fluentState;
        }

        public virtual bool StartState(T stateId)
        {
            return ChangeState(stateId, false);
        }

        public virtual bool ChangeState(T stateId, bool withEvent = true)
        {
            if (CurrentState != null && EqualityComparer<T>.Default.Equals(stateId, CurrentStateId))
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

                if (withEvent)
                    OnStateChanged?.Invoke(stateId);

                CurrentState.Enter();
            }
            else
            {
                if (withEvent)
                    OnStateChanged?.Invoke(stateId);

                CurrentState = null;
            }

            return true;
        }

        private void CheckCurrentState()
        {
            if (CurrentState == null)
            {
                throw new InvalidOperationException(
                    $"The current state in the state machine '{GetType().Name}' is null, " +
                    $"you must first call the 'StartState' method to set a starting state");
            }
        }

        public IEnumerator<KeyValuePair<T, IState>> GetEnumerator()
        {
            return States.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
