using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using EasyFramework;
using JetBrains.Annotations;

namespace EasyGameFramework
{
    public class EasyFeedbacks : SerializedMonoBehaviour
    {
        public enum InitializationModes
        {
            Awake,
            Start
        }

        [FoldoutGroup("设置")]
        [TitleCN("初始化")]
        [LabelText("初始化模式")]
        public InitializationModes InitializationMode = InitializationModes.Awake;

        [FoldoutGroup("设置")]
        [Tooltip("确保播放前所有Feedbacks都初始化")]
        [LabelText("自动初始化")]
        public bool AutoInitialization = true;

        [FoldoutGroup("设置")]
        [Tooltip("在Start时自动播放一次")]
        [LabelText("Start时自动播放")]
        public bool AutoPlayOnStart;

        [FoldoutGroup("设置")]
        [Tooltip("在OnEnable时自动播放一次")]
        [LabelText("Enable时自动播放")]
        public bool AutoPlayOnEnable;

        [FoldoutGroup("设置")]
        [TitleCN("播放")]
        [Tooltip("是否可以播放")]
        [LabelText("是否可以播放")]
        public bool CanPlay = true;

        [FoldoutGroup("设置")]
        [Tooltip("在当前Play还没结束时是否可以开始新的播放")]
        [LabelText("播放时是否可以继续播放")]
        public bool CanPlayWhileAlreadyPlaying = false;

        [FoldoutGroup("设置")]
        [Tooltip("是否可以同时存在多个播放\n注意：Feedback的OnFeedbackStop只会在最后一个播放结束时调用")]
        [LabelText("是否可以多重播放")]
        [ShowIf("CanPlayWhileAlreadyPlaying")]
        public bool CanMultiPlay = false;

        [LabelText("反馈列表")]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(CustomAddFunction = "OnAddFeedback")]
        public List<AbstractEasyFeedback> FeedbackList = new List<AbstractEasyFeedback>();

        public bool IsInitialized { get; private set; }

        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }

        public float TimeSinceLastPlay { get; private set; }

        private List<Coroutine> _coroutines;

        public bool HasFeedbackPlaying()
        {
            foreach (var feedback in FeedbackList)
            {
                if (feedback.Enable && feedback.IsPlaying)
                {
                    return true;
                }
            }

            return false;
        }

        private void Awake()
        {
            if (!TryGetComponent(out FeedbacksCoroutineHelper coroutineHelper))
            {
                coroutineHelper = gameObject.AddComponent<FeedbacksCoroutineHelper>();
            }

            _coroutines = new List<Coroutine>();
            CoroutineHelper = coroutineHelper;
            if (InitializationMode == InitializationModes.Awake)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (InitializationMode == InitializationModes.Start)
            {
                Initialize();
            }

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        private void Update()
        {
        }

        private void OnEnable()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnEnable();
            }

            if (AutoPlayOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            foreach (var item in FeedbackList)
            {
                item.OnDisable();
            }
        }

        public bool IsPlayable()
        {
            if (!CanPlay) return false;
            if (HasFeedbackPlaying())
            {
                if (!CanPlayWhileAlreadyPlaying)
                    return false;
            }

            return true;
        }

        public void Play()
        {
            if (!IsInitialized && AutoInitialization)
            {
                Initialize();
            }

            if (!IsPlayable()) return;
            if (!CanMultiPlay)
                Stop();
            ResetFeedbacks();

            _coroutines.Add(StartCoroutine(PlayFeedbacksCo()));
        }

        private void ResetFeedbacks()
        {
            foreach (var feedback in FeedbackList)
            {
                if (feedback is { Enable: true })
                {
                    feedback.Reset();
                }
            }
        }

        private IEnumerator PlayFeedbacksCo()
        {
            TimeSinceLastPlay = Time.time;

            foreach (var feedback in FeedbackList)
            {
                if (feedback.Enable)
                {
                    yield return feedback.PlayCo();
                }
            }
        }

        public void Stop()
        {
            if (!IsInitialized)
            {
                return;
            }

            foreach (var co in _coroutines)
            {
                if (co != null)
                {
                    StopCoroutine(co);
                }
            }

            _coroutines.Clear();
            foreach (var feedback in FeedbackList)
            {
                feedback.Stop();
            }
        }

        private void OnDestroy()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDestroy();
            }
        }

        private void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            foreach (var feedback in FeedbackList)
            {
                feedback.Setup(this);
                feedback.Initialize();
            }
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_coroutines != null)
            {
                Stop();
            }
            else
            {
                _coroutines = new List<Coroutine>();
            }

            IsInitialized = false;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDrawGizmosSelected();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnDrawGizmos();
            }
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.Setup(this);
                feedback.OnInspectorInit();
            }
        }

        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (var feedback in FeedbackList)
            {
                feedback.OnInspectorGUI();
            }
        }

        private bool DisableTestInit => IsInitialized || !UnityEditor.EditorApplication.isPlaying;

        [ButtonGroup]
        [DisableIf("DisableTestInit")]
        private void TestInit()
        {
            Initialize();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestPlay()
        {
            Play();
        }

        [ButtonGroup]
        [DisableInEditorMode]
        private void TestStop()
        {
            Stop();
        }

        public void OnSceneGUI()
        {
            foreach (var item in FeedbackList)
            {
                item.OnSceneGUI();
            }
        }

        private static Type[] s_allFeedbackTypes;

        static EasyFeedbacks()
        {
            s_allFeedbackTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(AbstractEasyFeedback))
                            && !t.IsAbstract
                            && t.HasCustomAttribute<AddEasyFeedbackMenuAttribute>())
                .ToArray();
        }

        [UsedImplicitly]
        private void OnAddFeedback()
        {
            void OnConfirm(Type t)
            {
                var inst = t.CreateInstance<AbstractEasyFeedback>();
                if (IsInitialized)
                {
                    inst.Initialize();
                }

                FeedbackList.Add(inst);
            }

            var config = new EasyEditorGUI.PopupSelectorConfig<Type>(s_allFeedbackTypes, OnConfirm)
            {
                MenuItemNameGetter = t => t.GetCustomAttribute<AddEasyFeedbackMenuAttribute>().Path
            };
            EasyEditorGUI.ShowSelectorInPopup(config);
        }
#endif
    }

    public class FeedbacksCoroutineHelper : MonoBehaviour
    {
    }
}
