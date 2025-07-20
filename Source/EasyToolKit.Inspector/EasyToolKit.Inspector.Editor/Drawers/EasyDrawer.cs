using System;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public interface IEasyDrawer : IInitializable
    {
        bool SkipWhenDrawing { get; set; }
        InspectorProperty Property { get; set; }
        void DrawProperty(GUIContent label);
        bool CanDrawProperty(InspectorProperty property);
    }

    public abstract class EasyDrawer : IEasyDrawer
    {
        public bool SkipWhenDrawing { get; set; }
        public InspectorProperty Property { get; private set; }
        public bool IsInitialized { get; private set; }

        protected virtual void Initialize()
        {
        }

        protected virtual void Deinitialize()
        {
        }

        protected virtual bool CanDrawProperty(InspectorProperty property)
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

        protected virtual void OnDrawProperty(GUIContent label)
        {
        }

        InspectorProperty IEasyDrawer.Property
        {
            get => Property;
            set => Property = value;
        }
        
        bool IInitializable.IsInitialized => IsInitialized;

        void IInitializable.Initialize()
        {
            if (IsInitialized) return;
            Initialize();
            IsInitialized = true;
        }

        void IInitializable.Deinitialize()
        {
            if (!IsInitialized) return;
            Deinitialize();
            IsInitialized = false;
        }

        void IEasyDrawer.DrawProperty(GUIContent label)
        {
            OnDrawProperty(label);
        }

        bool IEasyDrawer.CanDrawProperty(InspectorProperty property)
        {
            return CanDrawProperty(property);
        }
    }
}
