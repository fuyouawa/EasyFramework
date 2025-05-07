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
        Paused
    }

    public delegate void TweenEventHandler();

    public abstract class AbstractTween : ITweenClip
    {
        internal TweenType Type { get; set; }
        TweenType ITweenClip.Type => Type;

        internal string Id { get; set; }

        internal float Duration => GetDuration();

        internal float Delay { get; set; }

        internal TweenState CurrentState => _state.CurrentStateId;

        internal event TweenEventHandler OnPlay;

        internal event TweenEventHandler OnPause;

        internal event TweenEventHandler OnComplete;

        internal event TweenEventHandler OnKill;

        internal Sequence OwnerSequence { get; set; }

        protected abstract float GetDuration();
        
        private readonly StateMachine<TweenState> _state = new StateMachine<TweenState>();
        private bool _pause;
        private float _playElapsedTime;

        internal void Reset()
        {
            Id = string.Empty;
            Delay = 0f;
            OwnerSequence = null;

            OnPlay = null;
            OnPause = null;
            OnComplete = null;
            OnKill = null;

            _pause = false;
            _playElapsedTime = 0f;
            _state.StartState(TweenState.Idle);

            OnReset();
        }


        internal void Initialize()
        {
            _playElapsedTime = 0f;
            OnInit();
            _state.ChangeState(TweenState.DelayAfterPlay);
        }

        internal void Update()
        {
            var stateId = _state.CurrentStateId;
            if (stateId == TweenState.Idle)
            {
                throw new InvalidOperationException("Tween must be initialize first.");
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
                        OnPlay?.Invoke();
                    }
                }
                else
                {
                    var time = _playElapsedTime - Delay;
                    if (time > Duration)
                    {
                        _state.ChangeState(TweenState.Idle);
                        OnComplete?.Invoke();
                        OnKill?.Invoke();
                    }
                    else
                    {
                        OnPlaying(time);
                    }
                }

                _playElapsedTime += Time.deltaTime;
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

        protected virtual void OnReset() {}
        protected virtual void OnInit() {}
        protected abstract void OnPlaying(float time);
    }
}
