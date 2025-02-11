using System;

namespace EasyFramework
{
    public interface IFluentState
    {
        IFluentState OnInitialize(Action on);
        IFluentState OnCondition(Func<bool> on);
        IFluentState OnEnter(Action on);
        IFluentState OnUpdate(Action on);
        IFluentState OnExit(Action on);
        IFluentState OnEnable(Action on);
        IFluentState OnDisable(Action on);
    }

    public class FluentState : AbstractState, IFluentState
    {
        public Action OnInit;
        public Func<bool> OnCondition;

        public Action OnEnter;
        public Action OnExit;
        public Action OnUpdate;
        public Action OnEnable;
        public Action OnDisable;

        protected override void OnStateInit()
        {
            OnInit?.Invoke();
        }

        protected override bool OnStateCondition()
        {
            if (OnCondition != null)
            {
                return OnCondition();
            }
            return true;
        }

        protected override void OnStateEnter()
        {
            OnEnter?.Invoke();
        }

        protected override void OnStateExit()
        {
            OnExit?.Invoke();
        }

        protected override void OnStateUpdate()
        {
            OnUpdate?.Invoke();
        }

        protected override void OnStateEnable()
        {
            OnEnable?.Invoke();
        }

        protected override void OnStateDisable()
        {
            OnDisable?.Invoke();
        }

        IFluentState IFluentState.OnInitialize(Action on)
        {
            OnInit += on;
            return this;
        }

        IFluentState IFluentState.OnCondition(Func<bool> on)
        {
            OnCondition += on;
            return this;
        }

        IFluentState IFluentState.OnEnter(Action on)
        {
            OnEnter += on;
            return this;
        }

        IFluentState IFluentState.OnUpdate(Action on)
        {
            OnUpdate += on;
            return this;
        }

        IFluentState IFluentState.OnExit(Action on)
        {
            OnExit += on;
            return this;
        }

        IFluentState IFluentState.OnEnable(Action on)
        {
            OnEnable += on;
            return this;
        }

        IFluentState IFluentState.OnDisable(Action on)
        {
            OnDisable += on;
            return this;
        }
    }
}
