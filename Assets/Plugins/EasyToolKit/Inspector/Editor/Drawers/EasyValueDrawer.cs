using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyValueDrawer
    {
        private InspectorProperty _property;
        private bool _initialized;

        public InspectorProperty Property => _property;

        internal void Initialize(InspectorProperty property)
        {
            if (_initialized) return;

            _property = property;

            try
            {
                OnInitialize();
            }
            finally
            {
                _initialized = true;
            }
        }

        internal void DrawProperty(GUIContent label)
        {
            OnDrawProperty(label);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDrawProperty(GUIContent label)
        {
        }
    }
}
