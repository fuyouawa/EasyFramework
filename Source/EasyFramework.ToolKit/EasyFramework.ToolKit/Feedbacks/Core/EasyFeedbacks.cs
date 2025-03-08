using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class EasyFeedbacks : SerializedMonoBehaviour
    {
        public enum InitializationModes
        {
            OnAwake,
            OnStart
        }

        public InitializationModes InitializationMode = InitializationModes.OnAwake;
        public bool AutoInitialization = true;
        public bool AutoPlayOnStart;
        public bool AutoPlayOnEnable;
        public bool CanPlay = true;
        public bool CanPlayWhileAlreadyPlaying = false;
        public bool CanMultiPlay = false;
        public List<AbstractEasyFeedback> Feedbacks => _feedbacks;

        [SerializeField, ListDrawerSettings(HideAddButton = true)]
        private readonly List<AbstractEasyFeedback> _feedbacks = new List<AbstractEasyFeedback>();

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
            if (InitializationMode == InitializationModes.OnAwake)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (InitializationMode == InitializationModes.OnStart)
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

        private void OnDrawGizmosSelected()
        {
            foreach (var feedback in _feedbacks)
            {
                feedback.OnDrawGizmosSelected();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var feedback in _feedbacks)
            {
                feedback.OnDrawGizmos();
            }
        }
    }

    public class FeedbacksCoroutineHelper : MonoBehaviour
    {
    }
}
