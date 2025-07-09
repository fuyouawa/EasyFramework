using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Tweening
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
        private Action<AbstractTween> _onPlay;
        private Action<AbstractTween> _onPause;
        private Action<AbstractTween> _onComplete;
        private Action<AbstractTween> _onKill;

        public void AddPlayCallback(Action<AbstractTween> callback) => _onPlay += callback;
        public void AddPauseCallback(Action<AbstractTween> callback) => _onPause += callback;
        public void AddCompleteCallback(Action<AbstractTween> callback) => _onComplete += callback;
        public void AddKillCallback(Action<AbstractTween> callback) => _onKill += callback;


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

            _onPlay = null;
            _onPause = null;
            _onComplete = null;
            _onKill = null;

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
                    _onPlay?.Invoke(this);
                    break;
                case TweenState.Paused:
                    _onPause?.Invoke(this);
                    break;
                case TweenState.Completed:
                    _onComplete?.Invoke(this);
                    break;
                case TweenState.Killed:
                    _onKill?.Invoke(this);
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

            if (stateId != TweenState.Paused)
            {
                _playElapsedTime += Time.deltaTime;
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
