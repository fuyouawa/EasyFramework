using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class AbstractFeedback : IFeedback
    {
        [SerializeField] private string _label;
        [SerializeField] private bool _enable = true;

        [SerializeField] private float _delayBeforePlay;
        [SerializeField] private bool _blocking;
        [SerializeField] private bool _repeatForever = false;

        [HideIf(nameof(_repeatForever))]
        [SerializeField] private int _amountOfRepeat = 1;

        [SerializeField] private float _intervalBetweenRepeats = 0f;

        public virtual string Tip => string.Empty;

        public Feedbacks Owner { get; private set; }

        public bool IsPlaying { get; protected set; }
        public float TimeSincePlay { get; protected set; }

        bool IFeedback.Enable
        {
            get => _enable;
            set => _enable = value;
        }

        string IFeedback.Label => _label;


        protected virtual IEnumerator Pause => null;

        private bool _isInitialized = false;
        private Coroutine _lastPlayCoroutine;
        private int _playCount = 0;

        public virtual void Setup(Feedbacks owner)
        {
            Owner = owner;
        }

        public virtual void Reset()
        {
            OnFeedbackReset();
        }

        public virtual IEnumerator PlayCo()
        {
            IsPlaying = true;
            TimeSincePlay = Time.time;

            if (!_blocking)
            {
                if (!Owner.CanMultiPlay)
                {
                    if (_lastPlayCoroutine != null)
                        StopCoroutine(_lastPlayCoroutine);
                }

                _lastPlayCoroutine = StartCoroutine(FeedbackPlayCo());
            }
            else
            {
                yield return FeedbackPlayCo();
            }
        }


        public virtual void Stop()
        {
            if (!IsPlaying)
                return;
            IsPlaying = false;

            if (_lastPlayCoroutine != null)
                StopCoroutine(_lastPlayCoroutine);
            _playCount = 0;

            OnFeedbackStop();
        }

        public virtual void Initialize()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            OnFeedbackInit();
        }

        protected virtual IEnumerator FeedbackPlayCo()
        {
            _playCount++;
            if (_delayBeforePlay > 0f)
            {
                yield return new WaitForSeconds(_delayBeforePlay);
            }

            var loop = Mathf.Max(_amountOfRepeat, 1);
            while (loop > 0 && IsPlaying)
            {
                if (!IsPlaying)
                    yield break;

                Reset();
                OnFeedbackPlay();

                var p = Pause;
                if (p != null)
                {
                    yield return p;
                }

                var d = GetDuration();
                if (d > 0f)
                {
                    yield return new WaitForSeconds(d);
                }

                if (!_repeatForever)
                {
                    loop--;
                }

                if (loop > 0 && _intervalBetweenRepeats > 0)
                {
                    yield return new WaitForSeconds(_intervalBetweenRepeats);
                }
            }

            _playCount--;
            if (_playCount == 0)
            {
                Stop();
            }
        }

        protected virtual void OnFeedbackInit()
        {
        }

        protected virtual void OnFeedbackReset()
        {
        }

        protected abstract void OnFeedbackPlay();

        protected abstract void OnFeedbackStop();

        public virtual void OnDestroy()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual float GetDuration()
        {
            return 0;
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return Owner.CoroutineHelper.StartCoroutine(routine);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        public void StopAllCoroutine()
        {
            Owner.CoroutineHelper.StopAllCoroutines();
        }

        public virtual void OnDrawGizmos()
        {
        }

        public virtual void OnDrawGizmosSelected()
        {
        }

        public virtual void OnValidate()
        {
        }
    }
}
