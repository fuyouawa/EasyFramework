using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyDrawer
    {
        private InspectorProperty _property;
        private bool _initialized;
        
        public bool SkipWhenDrawing { get; set; }
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
            if (!_initialized)
                throw new InvalidOperationException();  //TODO 异常信息
            OnDrawProperty(label);
        }

        public virtual bool CanDrawProperty(InspectorProperty property)
        {
            return true;
        }
        
        protected bool CallNextDrawer(GUIContent label)
        {
            var chain = Property.GetDrawerChain();
            if (chain.MoveNext())
            {
                chain.Current.DrawProperty(label);
                return true;
            }
            return false;
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnDrawProperty(GUIContent label)
        {
        }
    }
}
