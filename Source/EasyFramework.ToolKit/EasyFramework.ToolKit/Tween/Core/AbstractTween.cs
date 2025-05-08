using System;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
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

    public delegate void TweenEventHandler();

    public abstract class AbstractTween : ITweenClip
    {
        internal TweenType Type { get; set; }
        TweenType ITweenClip.Type => Type;

        internal string Id { get; set; }

        internal float Duration => GetDuration();

        internal float Delay { get; set; }

        internal int Loop { get; set; }

        internal TweenState CurrentState => _state.CurrentStateId;

        internal event TweenEventHandler OnPlay;

        internal event TweenEventHandler OnPause;

        internal event TweenEventHandler OnCompleted;

        internal event TweenEventHandler OnKill;

        internal Sequence OwnerSequence { get; set; }

        internal bool PendingKill { get; set; }

        protected abstract float GetDuration();
        
        private readonly StateMachine<TweenState> _state = new StateMachine<TweenState>();
        private bool _pause;
        private float _playElapsedTime;

        internal void Reset()
        {
            Id = string.Empty;
            Delay = 0f;
            Loop = 1;
            OwnerSequence = null;

            OnPlay = null;
            OnPause = null;
            OnCompleted = null;
            OnKill = null;

            _pause = false;
            _playElapsedTime = 0f;
            _state.StartState(TweenState.Idle);
            _state.OnStateChanged -= OnStateChanged;

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
            _state.OnStateChanged += OnStateChanged;
        }

        protected virtual void OnStateChanged(TweenState state)
        {
            switch (state)
            {
                case TweenState.Idle:
                    break;
                case TweenState.DelayAfterPlay:
                    break;
                case TweenState.Playing:
                    OnPlay?.Invoke();
                    break;
                case TweenState.Paused:
                    OnPause?.Invoke();
                    break;
                case TweenState.Completed:
                    OnCompleted?.Invoke();
                    break;
                case TweenState.Killed:
                    OnKill?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        internal void Update()
        {
            var stateId = _state.CurrentStateId;
            if (stateId == TweenState.Idle)
            {
                throw new InvalidOperationException("Tween must be initialize first.");
            }

            if (stateId == TweenState.Completed)
            {
                throw new InvalidOperationException("The tween has been finished.");
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
                    if (time > Duration)
                    {
                        _state.ChangeState(TweenState.Completed);
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
            _state.ChangeState(TweenState.Killed);
        }

        internal void Pause()
        {
            _pause = true;
        }

        internal void Resume()
        {
            _pause = false;
        }

        protected virtual void OnReset() {}
        protected virtual void OnStart() {}
        protected abstract void OnPlaying(float time);
    }
}
