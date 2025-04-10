using EasyFramework;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Pokeworld.UI.Tools
{
    public class EasyToggleGroup : MonoBehaviour
    {
        [SerializeField] private bool _allowSwitchOff = false;

        /// <summary>
        /// Is it allowed that no toggle is switched on?
        /// </summary>
        /// <remarks>
        /// If this setting is enabled, pressing the toggle that is currently switched on will switch it off, so that no toggle is switched on. If this setting is disabled, pressing the toggle that is currently switched on will not change its state.
        /// Note that even if allowSwitchOff is false, the Toggle Group will not enforce its constraint right away if no toggles in the group are switched on when the scene is loaded or when the group is instantiated. It will only prevent the user from switching a toggle off.
        /// </remarks>
        public bool AllowSwitchOff
        {
            get { return _allowSwitchOff; }
            set { _allowSwitchOff = value; }
        }

        private readonly List<EasyToggle> _toggles = new List<EasyToggle>();

        public EasyEvent<EasyToggle> OnToggleSelected;

        /// <summary>
        /// Because all the Toggles have registered themselves in the OnEnabled, Start should check to
        /// make sure at least one Toggle is active in groups that do not AllowSwitchOff
        /// </summary>
        private void Start()
        {
            EnsureValidState();
        }

        private void OnEnable()
        {
            EnsureValidState();
        }

        private void ValidateToggleIsInGroup(EasyToggle toggle)
        {
            if (toggle == null || !_toggles.Contains(toggle))
                throw new ArgumentException(string.Format("Toggle {0} is not part of ToggleGroup {1}",
                    new object[] { toggle, this }));
        }

        /// <summary>
        /// Notify the group that the given toggle is enabled.
        /// </summary>
        /// <param name="toggle">The toggle that got triggered on.</param>
        /// <param name="sendCallback">If other toggles should send onValueChanged.</param>
        internal void NotifyToggleOn(EasyToggle toggle, bool sendCallback = true)
        {
            ValidateToggleIsInGroup(toggle);
            // disable all toggles in the group
            for (var i = 0; i < _toggles.Count; i++)
            {
                if (_toggles[i] == toggle)
                    continue;

                if (sendCallback)
                    _toggles[i].IsOn = false;
                else
                    _toggles[i].SetIsOnWithoutNotify(false);
            }

            if (sendCallback)
            {
                OnToggleSelected?.Invoke(toggle);
            }
        }

        /// <summary>
        /// Unregister a toggle from the group.
        /// </summary>
        /// <param name="toggle">The toggle to remove.</param>
        internal void UnRegisterToggle(EasyToggle toggle)
        {
            if (_toggles.Contains(toggle))
                _toggles.Remove(toggle);
        }

        public void Clear(bool withDestroy = true)
        {
            foreach (var toggle in _toggles.ToArray())
            {
                toggle.Group = null;
                if (withDestroy)
                    Destroy(toggle.gameObject);
            }
        }

        /// <summary>
        /// Register a toggle with the toggle group so it is watched for changes and notified if another toggle in the group changes.
        /// </summary>
        /// <param name="toggle">The toggle to register with the group.</param>
        internal void RegisterToggle(EasyToggle toggle)
        {
            if (!_toggles.Contains(toggle))
                _toggles.Add(toggle);
        }

        /// <summary>
        /// Ensure that the toggle group still has a valid state. This is only relevant when a ToggleGroup is Started
        /// or a Toggle has been deleted from the group.
        /// </summary>
        public void EnsureValidState()
        {
            if (!AllowSwitchOff && !AnyTogglesOn() && _toggles.Count != 0)
            {
                _toggles[0].IsOn = true;
                NotifyToggleOn(_toggles[0]);
            }

            var activeToggles = ActiveToggles();

            if (activeToggles.Length > 1)
            {
                var firstActive = GetFirstActiveToggle();

                foreach (var toggle in activeToggles)
                {
                    if (toggle == firstActive)
                    {
                        continue;
                    }

                    toggle.IsOn = false;
                }
            }
        }

        /// <summary>
        /// Are any of the toggles on?
        /// </summary>
        /// <returns>Are and of the toggles on?</returns>
        public bool AnyTogglesOn()
        {
            return _toggles.Find(x => x.IsOn) != null;
        }

        /// <summary>
        /// Returns the toggles in this group that are active.
        /// </summary>
        /// <returns>The active toggles in the group.</returns>
        /// <remarks>
        /// Toggles belonging to this group but are not active either because their GameObject is inactive or because the Toggle component is disabled, are not returned as part of the list.
        /// </remarks>
        public EasyToggle[] ActiveToggles()
        {
            return _toggles.Where(x => x.IsOn).ToArray();
        }

        /// <summary>
        /// Returns the toggle that is the first in the list of active toggles.
        /// </summary>
        /// <returns>The first active toggle from m_Toggles</returns>
        /// <remarks>
        /// Get the active toggle for this group. As the group
        /// </remarks>
        public EasyToggle GetFirstActiveToggle()
        {
            var activeToggles = ActiveToggles();
            return activeToggles.FirstOrDefault();
        }

        /// <summary>
        /// Switch all toggles off.
        /// </summary>
        /// <remarks>
        /// This method can be used to switch all toggles off, regardless of whether the allowSwitchOff property is enabled or not.
        /// </remarks>
        public void SetAllTogglesOff(bool sendCallback = true)
        {
            bool oldAllowSwitchOff = _allowSwitchOff;
            _allowSwitchOff = true;

            if (sendCallback)
            {
                for (var i = 0; i < _toggles.Count; i++)
                    _toggles[i].IsOn = false;
            }
            else
            {
                for (var i = 0; i < _toggles.Count; i++)
                    _toggles[i].SetIsOnWithoutNotify(false);
            }

            _allowSwitchOff = oldAllowSwitchOff;
        }
    }
}
