using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework
{
    public delegate void OnStateChangeDelegate<in T>(T previous, T current);

    public class StateMachine<T>
    {
        public GameObject Target;
        public T CurrentState { get; protected set; }
        public T PreviousState { get; protected set; }

        /// <summary>
        /// 当有状态更改时的回调
        /// </summary>
        public event OnStateChangeDelegate<T> OnStateChange;

        public StateMachine(GameObject target)
        {
            Target = target;
        }

        public virtual void ChangeState(T newState)
        {
            if (EqualityComparer<T>.Default.Equals(newState, CurrentState))
            {
                return;
            }

            PreviousState = CurrentState;
            CurrentState = newState;
            OnStateChange?.Invoke(PreviousState, newState);
        }

        public virtual void RestorePreviousState()
        {
            if (PreviousState != null)
            {
                var tmp = CurrentState;
                CurrentState = (T)PreviousState;

                OnStateChange?.Invoke(tmp, (T)PreviousState);
            }
        }
    }
}
