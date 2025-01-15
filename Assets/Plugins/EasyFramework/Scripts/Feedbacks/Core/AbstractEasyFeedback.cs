using System;
using System.Collections;
using EasyFramework;
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

    public class EasyFeedbackHelperAttribute : Attribute
    {
        public string Message { get; }

        public EasyFeedbackHelperAttribute(string message)
        {
            Message = message;
        }
    }


    [Serializable]
    public abstract class AbstractEasyFeedback
    {
        [InfoBoxCN("@FeedbackHelpMessage", VisibleIf = "VisibleFeedbackHelper")]
        [LabelText("标签")]
        public string Label;
        // public Color BarColor = Color.black;
        [LabelText("激活")]
        public bool Enable = true;

        [FoldoutGroup("Feedback设置")]
        [TitleCN("时间")]
        [Tooltip("在正式Play前经过多少时间的延迟(s)")]
        [LabelText("播放前延迟")]
        public float DelayBeforePlay;
        [FoldoutGroup("Feedback设置")]
        [Tooltip("是否会阻塞Feedbacks运行")]
        [LabelText("阻塞")]
        public bool Blocking;

        [FoldoutGroup("Feedback设置")]
        [TitleCN("重复")]
        [Tooltip("无限重复播放")]
        [LabelText("无限重复")]
        public bool RepeatForever = false;
        [FoldoutGroup("Feedback设置")]
        [HideIf("RepeatForever")]
        [Tooltip("重复播放的次数")]
        [LabelText("重复次数")]
        public int AmountOfRepeat = 1;
        [FoldoutGroup("Feedback设置")]
        [Tooltip("每次循环播放的间隔")]
        [LabelText("重复间隔")]
        public float IntervalBetweenRepeats = 0f;

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

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return Owner.CoroutineHelper.StartCoroutine(routine);
        }

        protected void StopCoroutine(IEnumerator routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopCoroutine(Coroutine routine)
        {
            Owner.CoroutineHelper.StopCoroutine(routine);
        }

        protected void StopAllCoroutine()
        {
            Owner.CoroutineHelper.StopAllCoroutines();
        }

#if UNITY_EDITOR
        private string _message;
        private string FeedbackHelpMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_message))
                {
                    var attr = GetType().GetCustomAttribute<EasyFeedbackHelperAttribute>();
                    if (attr != null)
                    {
                        _message = attr.Message;
                    }
                }
                return _message;
            }
        }

        private bool VisibleFeedbackHelper => FeedbackHelpMessage.IsNotNullOrEmpty();

        public virtual void OnDrawGizmos() { }
        public virtual void OnDrawGizmosSelected() { }
        public virtual void OnValidate() { }
        public virtual void OnInspectorInit() { }
        public virtual void OnInspectorGUI() { }
        public virtual void OnSceneGUI() { }


        [ButtonGroup]
        [DisableInEditorMode]
        private void TestPlay()
        {
            StartCoroutine(PlayCo());
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }
#endif
    }
}
