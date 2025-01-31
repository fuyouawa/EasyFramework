using UnityEngine;

namespace EasyFramework
{
    public interface IAbility : IState
    {
        void Enable();
        void Disable();
        void FixedUpdate();
        void LateUpdate();
    }

    public abstract class AbstractAbility : MonoBehaviour, IAbility
    {
        bool IState.Condition()
        {
            OnUpdateAuthorized();
            return Authorized;
        }

        void IState.Enter()
        {
            OnEnterAbility();
        }

        void IState.Update()
        {
            OnEarlyProcessAbility();
            OnProcessAbility();
        }

        void IAbility.FixedUpdate()
        {
            OnFixedProcessAbility();
        }

        void IAbility.LateUpdate()
        {
            OnLateProcessAbility();
        }

        void IState.Exit()
        {
            OnExitAbility();
        }

        void IAbility.Enable()
        {
            OnEnableAbility();
        }

        void IAbility.Disable()
        {
            OnDisableAbility();
        }

        /// <summary>
        /// 当前能力是否有权限运行(非强制性)
        /// </summary>
        public bool Authorized { get; protected set; }

        protected virtual void OnUpdateAuthorized()
        {
            Authorized = true;
        }

        protected virtual void OnEnterAbility()
        {
        }

        protected virtual void OnEarlyProcessAbility()
        {
        }


        protected virtual void OnProcessAbility()
        {
        }

        protected virtual void OnFixedProcessAbility()
        {
        }

        protected virtual void OnLateProcessAbility()
        {
        }

        protected virtual void OnExitAbility()
        {
        }

        protected virtual void OnEnableAbility()
        {
        }

        protected virtual void OnDisableAbility()
        {
        }
    }

    public class AbilityMachine<T>
    {
        public T CurrentAbilityId => _machine.CurrentStateId;
        public T PreviousAbilityId => _machine.PreviousStateId;

        public IAbility CurrentAbility => (IAbility)_machine.CurrentState;

        private readonly StateMachine<T> _machine = new StateMachine<T>();

        public AbilityMachine()
        {
        }

        public virtual void Update()
        {
            CurrentAbility.Update();
        }

        public void FixedUpdate()
        {
            CurrentAbility.FixedUpdate();
        }

        public void LateUpdate()
        {
            CurrentAbility.LateUpdate();
        }

        public void Enable()
        {
            foreach (var state in _machine.States)
            {
                var ability = (IAbility)state.Value;
                ability.Enable();
            }
        }

        public void Disable()
        {
            foreach (var state in _machine.States)
            {
                var ability = (IAbility)state.Value;
                ability.Disable();
            }
        }

        public IAbility GetAbility(T abilityId)
        {
            return (IAbility)_machine.GetState(abilityId);
        }

        public void AddAbility(T abilityId, IAbility ability)
        {
            _machine.AddState(abilityId, ability);
        }

        public bool ChangeAbility(T abilityId)
        {
            return _machine.ChangeState(abilityId);
        }
    }
}
