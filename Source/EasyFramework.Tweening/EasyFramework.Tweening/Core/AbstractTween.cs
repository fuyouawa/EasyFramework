using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.Tweening
{
    public enum TweenState
    {
        Idle,
        DelayAfterPlay,
        Playing,
        Paused,
        Completed,
        Killed,
    }

    public delegate void TweenEventHandler(AbstractTween tween);

    public abstract class AbstractTween
    {
        private string _id;
        internal string Id 
        { 
            get => _id;
            set
            {
                if (_id == value)
                    return;

                if (!string.IsNullOrEmpty(_id))
                {
                    TweenController.Instance.UnregisterTweenById(_id);
                }

                _id = value;

                if (!string.IsNullOrEmpty(_id))
                {
                    TweenController.Instance.RegisterTweenById(_id, this);
                }
            }
        }

        internal float? GetActualDuration() => ActualDuration;

        /// <summary>
        /// 获取实际的持续时间，返回null代表无法判断具体持续时间。
        /// </summary>
        /// <returns></returns>
        protected virtual float? ActualDuration => null;

        internal float? LastPlayTime { get; private set; }

        internal float Delay { get; set; }

        internal bool InfiniteLoop { get; set; }
        internal int LoopCount { get; set; }

        internal TweenState CurrentState => _state.CurrentStateId;
        
        internal event TweenEventHandler OnPlay;

        internal event TweenEventHandler OnPause;

        internal event TweenEventHandler OnCompleted;

        internal event TweenEventHandler OnKilled;

        internal TweenSequence OwnerSequence { get; set; }

        internal bool PendingKillSelf { get; set; }
        protected internal bool IsInLoop { get; set; }

        internal bool IsPendingKill()
        {
            if (PendingKillSelf)
            {
                return true;
            }

            if (OwnerSequence != null)
            {
                return OwnerSequence.IsPendingKill();
            }
            return false;
        }

        private readonly StateMachine<TweenState> _state = new StateMachine<TweenState>();
        private bool _pause;
        private float _playElapsedTime;

        protected AbstractTween()
        {
            _state.OnStateChanged += OnStateChanged;
            Reset();
            TweenController.Instance.Attach(this);
        }

        internal void Reset()
        {
            Id = string.Empty;
            Delay = 0f;
            InfiniteLoop = false;
            LoopCount = 1;
            OwnerSequence = null;
            LastPlayTime = null;

            OnPlay = null;
            OnPause = null;
            OnCompleted = null;
            OnKilled = null;

            _pause = false;
            _playElapsedTime = 0f;
            _state.StartState(TweenState.Idle);

            OnReset();
        }

        internal void Start()
        {
            _playElapsedTime = 0f;
            OnStart();

            if (Delay > 0)
            {
                _state.ChangeState(TweenState.DelayAfterPlay);
            }
            else
            {
                _state.ChangeState(TweenState.Playing);
            }

            if (IsInLoop)
            {
                OnLoop();
            }
        }

        protected virtual void OnLoop() {}

        protected virtual void OnStateChanged(TweenState state)
        {
            switch (state)
            {
                case TweenState.Idle:
                    break;
                case TweenState.DelayAfterPlay:
                    break;
                case TweenState.Playing:
                    OnPlay?.Invoke(this);
                    break;
                case TweenState.Paused:
                    OnPause?.Invoke(this);
                    break;
                case TweenState.Completed:
                    OnCompleted?.Invoke(this);
                    break;
                case TweenState.Killed:
                    OnKilled?.Invoke(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        internal void Update()
        {
            var stateId = _state.CurrentStateId;
            Assert.True(stateId != TweenState.Idle);

            if (stateId == TweenState.Completed)
            {
                return;
            }

            if (stateId == TweenState.Paused)
            {
                if (!_pause)
                {
                    if (_playElapsedTime < Delay)
                    {
                        _state.ChangeState(TweenState.DelayAfterPlay);
                    }
                    else
                    {
                        _state.ChangeState(TweenState.Playing);
                    }
                }
            }

            if (stateId == TweenState.DelayAfterPlay || stateId == TweenState.Playing)
            {
                if (_pause)
                {
                    _state.ChangeState(TweenState.Paused);
                }

                if (stateId == TweenState.DelayAfterPlay)
                {
                    if (_playElapsedTime > Delay)
                    {
                        _state.ChangeState(TweenState.Playing);
                    }
                }
                else
                {
                    var time = _playElapsedTime - Delay;
                    if (ActualDuration.HasValue && time >= ActualDuration.Value)
                    {
                        // 减小运动误差
                        if (!time.Approximately(ActualDuration.Value))
                        {
                            OnPlaying(ActualDuration.Value);
                        }

                        Complete();
                    }
                    else
                    {
                        OnPlaying(time);
                    }
                }

                _playElapsedTime += Time.deltaTime;
            }
        }

        internal void Kill()
        {
            OnKill();

            if (Id.IsNotNullOrEmpty())
            {
                TweenController.Instance.UnregisterTweenById(Id);
            }
            _state.ChangeState(TweenState.Killed);
        }

        internal void Complete()
        {
            LastPlayTime = Mathf.Max(_playElapsedTime - Delay, 0f);
            _state.ChangeState(TweenState.Completed);

            if (InfiniteLoop || LoopCount >= 2)
            {
                if (!InfiniteLoop)
                    LoopCount--;

                IsInLoop = true;
                _state.ChangeState(TweenState.Idle);
            }
            else
            {
                PendingKillSelf = true;
            }
        }

        internal void Pause()
        {
            _pause = true;
        }

        internal void Resume()
        {
            _pause = false;
        }

        protected virtual void OnReset()
        {
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnKill()
        {
        }

        protected abstract void OnPlaying(float time);

        public override string ToString()
        {
            if (Id.IsNotNullOrEmpty())
            {
                return Id;
            }

            return "<No ID>";
        }
    }
}
