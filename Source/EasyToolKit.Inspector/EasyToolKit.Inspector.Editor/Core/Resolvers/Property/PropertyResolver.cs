using System;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyResolver : IInitializableResolver
    {
        InspectorPropertyInfo GetChildInfo(int childIndex);
        int ChildNameToIndex(string name);
        int GetChildCount();
        bool ApplyChanges();
    }

    public abstract class PropertyResolver : IPropertyResolver
    {
        public InspectorProperty Property { get; private set; }
        public bool IsInitialized { get; private set; }

        InspectorProperty IInitializableResolver.Property
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

        protected virtual void Initialize() { }
        protected virtual void Deinitialize() { }

        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);
        public abstract int ChildNameToIndex(string name);
        public abstract int GetChildCount();

        bool IPropertyResolver.ApplyChanges()
        {
            return ApplyChanges();
        }

        protected virtual bool ApplyChanges() => false;
    }
}
