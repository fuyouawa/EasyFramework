using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum FeedbacksInitializeMode
    {
        OnAwake,
        OnStart
    }

    public class Feedbacks : SerializedMonoBehaviour
    {
        [SerializeField] private FeedbacksInitializeMode _initializeMode = FeedbacksInitializeMode.OnAwake;
        [SerializeField] private bool _autoInitialization = true;
        [SerializeField] private bool _autoPlayOnStart;
        [SerializeField] private bool _autoPlayOnEnable;
        [SerializeField] private bool _canPlay = true;
        [SerializeField] private bool _canPlayWhileAlreadyPlaying = false;

        [ShowIf(nameof(CanPlayWhileAlreadyPlaying))]
        [SerializeField] private bool _canMultiPlay = false;

        [ListDrawerSettings(HideAddButton = true)]
        [SerializeField] private readonly List<IFeedback> _feedbacks = new List<IFeedback>();

        public FeedbacksInitializeMode InitializeMode
        {
            get => _initializeMode;
            set => _initializeMode = value;
        }

        public bool AutoInitialization
        {
            get => _autoInitialization;
            set => _autoInitialization = value;
        }

        public bool AutoPlayOnStart
        {
            get => _autoPlayOnStart;
            set => _autoPlayOnStart = value;
        }

        public bool AutoPlayOnEnable
        {
            get => _autoPlayOnEnable;
            set => _autoPlayOnEnable = value;
        }

        public bool CanPlay
        {
            get => _canPlay;
            set => _canPlay = value;
        }

        public bool CanPlayWhileAlreadyPlaying
        {
            get => _canPlayWhileAlreadyPlaying;
            set => _canPlayWhileAlreadyPlaying = value;
        }

        public bool CanMultiPlay
        {
            get => _canMultiPlay;
            set => _canMultiPlay = value;
        }

        public bool IsInitialized { get; private set; }
        public FeedbacksCoroutineHelper CoroutineHelper { get; private set; }
        public float TimeSinceLastPlay { get; private set; }

        private List<Coroutine> _coroutines;

        public bool HasFeedbackPlaying()
        {
            foreach (var feedback in _feedbacks)
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
            if (InitializeMode == FeedbacksInitializeMode.OnAwake)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (InitializeMode == FeedbacksInitializeMode.OnStart)
            {
                Initialize();
            }

            if (AutoPlayOnStart)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            foreach (var feedback in _feedbacks)
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
            foreach (var item in _feedbacks)
            {
                item.OnDisable();
            }
        }

        public void AddFeedback(IFeedback feedback)
        {
            _feedbacks.Add(feedback);
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
            foreach (var feedback in _feedbacks)
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

            foreach (var feedback in _feedbacks)
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
            foreach (var feedback in _feedbacks)
            {
                feedback.Stop();
            }
        }

        private void OnDestroy()
        {
            foreach (var feedback in _feedbacks)
            {
                feedback.OnDestroy();
            }
        }

        public void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;

            foreach (var feedback in _feedbacks)
            {
                feedback.Setup(this);
                feedback.Initialize();
            }
        }


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
    }

    public class FeedbacksCoroutineHelper : MonoBehaviour
    {
    }
}
