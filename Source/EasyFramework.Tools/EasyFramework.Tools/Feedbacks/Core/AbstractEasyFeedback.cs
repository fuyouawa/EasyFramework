using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyGameFramework
{
    public class AddEasyFeedbackMenuAttribute : Attribute
    {
        public string Path { get; }

        public AddEasyFeedbackMenuAttribute(string path)
        {
            Path = path;
        }
    }

    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class AbstractEasyFeedback
    {
        public string Label;
        public bool Enable = true;
        public float DelayBeforePlay;
        public bool Blocking;
        public bool RepeatForever = false;
        public int AmountOfRepeat = 1;
        public float IntervalBetweenRepeats = 0f;

        public virtual string Tip => string.Empty;

        public EasyFeedbacks Owner { get; private set; }
        public bool IsPlaying { get; protected set; }
        public float TimeSincePlay { get; protected set; }

        protected virtual IEnumerator Pause => null;

        protected bool IsInitialized = false;
        private Coroutine _lastPlayCoroutine;
        private int _playCount = 0;

        public virtual void Setup(EasyFeedbacks owner)
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

            if (!Blocking)
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
            if (IsInitialized) return;
            IsInitialized = true;

            OnFeedbackInit();
        }

        protected virtual IEnumerator FeedbackPlayCo()
        {
            _playCount++;
            if (DelayBeforePlay > 0f)
            {
                yield return new WaitForSeconds(DelayBeforePlay);
            }

            var loop = Mathf.Max(AmountOfRepeat, 1);
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

                if (!RepeatForever)
                {
                    loop--;
                }

                if (loop > 0 && IntervalBetweenRepeats > 0)
                {
                    yield return new WaitForSeconds(IntervalBetweenRepeats);
                }

            }

            _playCount--;
            if (_playCount == 0)
            {
                Stop();
            }
        }

        protected virtual void OnFeedbackInit() { }

        protected virtual void OnFeedbackReset() { }

        protected abstract void OnFeedbackPlay();

        protected abstract void OnFeedbackStop();

        public virtual void OnDestroy() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

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

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
        public virtual void OnValidate() { }
    }
}
