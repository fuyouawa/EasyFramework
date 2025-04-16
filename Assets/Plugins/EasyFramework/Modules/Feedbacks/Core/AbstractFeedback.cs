using Sirenix.OdinInspector;
using System;
using System.Collections;
using EasyFramework.ToolKit;
using UnityEngine;

namespace EasyFramework.Modules
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class AbstractFeedback : IFeedback
    {
        [LabelText("标签")]
        public string Label;
        
        [LabelText("启用")]
        public bool Enable = true;

        [FoldoutGroup("反馈设置")]
        [LabelText("播放前延迟")]
        [Tooltip("在正式Play前经过多少时间的延迟(s)")]
        public float DelayBeforePlay;

        [FoldoutGroup("反馈设置")]
        [LabelText("阻塞")]
        [Tooltip("是否会阻塞反馈运行")]
        public bool Blocking;

        [FoldoutGroup("反馈设置")]
        [LabelText("无限重复")]
        [Tooltip("无限重复播放")]
        public bool RepeatForever = false;

        [FoldoutGroup("反馈设置")]
        [HideIf(nameof(RepeatForever))]
        [LabelText("重复次数")]
        [Tooltip("重复播放的次数")]
        public int AmountOfRepeat = 1;

        [FoldoutGroup("反馈设置")]
        [LabelText("重复间隔")]
        [Tooltip("每次循环播放的间隔")]
        public float IntervalBetweenRepeats = 0f;

        public virtual string Tip => string.Empty;

        public Feedbacks Owner { get; private set; }

        public bool IsPlaying { get; protected set; }
        public float TimeSincePlay { get; protected set; }

        bool IFeedback.Enable
        {
            get => Enable;
            set => Enable = value;
        }

        string IFeedback.Label => Label;


        protected virtual IEnumerator Pause => null;

        protected bool IsInitialized = false;
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
