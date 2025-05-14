using System;
using EasyFramework.Core;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// Display settings for when a toggle is activated or deactivated.
    /// </summary>
    public enum EasyToggleTransition
    {
        /// <summary>
        /// Show / hide the toggle instantly
        /// </summary>
        None,

        /// <summary>
        /// Fade the toggle in / out smoothly.
        /// </summary>
        Fade
    }

    [RequireComponent(typeof(RectTransform))]
    public class EasyToggle : Selectable, IPointerClickHandler, ISubmitHandler, ICanvasElement
    {
        /// <summary>
        /// Transition mode for the toggle.
        /// </summary>
        public EasyToggleTransition ToggleTransition = EasyToggleTransition.Fade;

        /// <summary>
        /// Graphic the toggle should be working with.
        /// </summary>
        public Graphic Graphic;

        [SerializeField]
        private EasyToggleGroup _group;

        /// <summary>
        /// Group the toggle belongs to.
        /// </summary>
        public EasyToggleGroup Group
        {
            get { return _group; }
            set
            {
                SetToggleGroup(value, true);
                PlayEffect(true);
            }
        }

        /// <summary>
        /// Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
        /// </summary>
        public EasyEvent<bool> OnValueChanged;

        // Whether the toggle is on
        [Tooltip("Is the toggle currently on or off?")]
        [SerializeField]
        private bool _isOn;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Application.isPlaying)
            {
                var isPrefab = (bool)Type.GetType("UnityEditor.PrefabUtility, UnityEditor").InvokeMethod("IsPartOfPrefabAsset",
                    BindingFlagsHelper.PublicStatic(), null, this);
                if (isPrefab)
                {
                    CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
                }
            }
        }

        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing == CanvasUpdate.Prelayout)
                OnValueChanged?.Invoke(_isOn);
        }

        public virtual void LayoutComplete()
        {}

        public virtual void GraphicUpdateComplete()
        {}

        protected override void OnDestroy()
        {
            if (_group != null)
                _group.EnsureValidState();
            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            SetToggleGroup(_group, false);
            PlayEffect(true);
        }

        protected override void OnDisable()
        {
            SetToggleGroup(null, false);
            base.OnDisable();
        }

        protected override void OnDidApplyAnimationProperties()
        {
            // Check if isOn has been changed by the animation.
            // Unfortunately there is no way to check if we don�t have a graphic.
            if (Graphic != null)
            {
                bool oldValue = !Mathf.Approximately(Graphic.canvasRenderer.GetColor().a, 0);
                if (_isOn != oldValue)
                {
                    _isOn = oldValue;
                    Set(!oldValue);
                }
            }

            base.OnDidApplyAnimationProperties();
        }

        private void SetToggleGroup(EasyToggleGroup newGroup, bool setMemberValue)
        {
            // Sometimes IsActive returns false in OnDisable so don't check for it.
            // Rather remove the toggle too often than too little.
            if (_group != null)
                _group.UnRegisterToggle(this);

            // At runtime the group variable should be set but not when calling this method from OnEnable or OnDisable.
            // That's why we use the setMemberValue parameter.
            if (setMemberValue)
                _group = newGroup;

            // Only register to the new group if this Toggle is active.
            if (newGroup != null && IsActive())
                newGroup.RegisterToggle(this);

            // If we are in a new group, and this toggle is on, notify group.
            // Note: Don't refer to m_Group here as it's not guaranteed to have been set.
            if (newGroup != null && IsOn && IsActive())
                newGroup.NotifyToggleOn(this);
        }

        /// <summary>
        /// Whether the toggle is currently active.
        /// </summary>
        public bool IsOn
        {
            get { return _isOn; }

            set
            {
                Set(value);
            }
        }

        /// <summary>
        /// Set isOn without invoking onValueChanged callback.
        /// </summary>
        /// <param name="value">New Value for isOn.</param>
        public void SetIsOnWithoutNotify(bool value)
        {
            Set(value, false);
        }

        void Set(bool value, bool sendCallback = true)
        {
            if (_isOn == value)
                return;

            // if we are in a group and set to true, do group logic
            _isOn = value;
            if (_group != null && _group.isActiveAndEnabled && IsActive())
            {
                if (_isOn || (!_group.AnyTogglesOn() && !_group.AllowSwitchOff))
                {
                    _isOn = true;
                    _group.NotifyToggleOn(this, sendCallback);
                }
            }
            
            PlayEffect(ToggleTransition == EasyToggleTransition.None);
            // Always send event when toggle is clicked, even if value didn't change
            // due to already active toggle in a toggle group being clicked.
            // Controls like Dropdown rely on this.
            // It's up to the user to ignore a selection being set to the same value it already was, if desired.
            if (sendCallback)
            {
                UISystemProfilerApi.AddMarker("Toggle.value", this);
                OnValueChanged?.Invoke(_isOn);
            }
        }

        /// <summary>
        /// Play the appropriate effect.
        /// </summary>
        private void PlayEffect(bool instant)
        {
            if (Graphic == null)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                Graphic.canvasRenderer.SetAlpha(_isOn ? 1f : 0f);
            else
#endif
            Graphic.CrossFadeAlpha(_isOn ? 1f : 0f, instant ? 0f : 0.1f, true);
        }

        /// <summary>
        /// Assume the correct visual state.
        /// </summary>
        protected override void Start()
        {
            PlayEffect(true);
        }

        private void InternalToggle()
        {
            if (!IsActive() || !IsInteractable())
                return;

            IsOn = !IsOn;
        }

        /// <summary>
        /// React to clicks.
        /// </summary>
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            InternalToggle();
        }

        void ISubmitHandler.OnSubmit(BaseEventData eventData)
        {
            InternalToggle();
        }
    }
}
