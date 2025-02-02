namespace EasyFramework
{
    public interface IState
    {
        bool Enable { get; set; }

        bool Condition();
        void Enter();
        void Update();
        void Exit();
    }

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
