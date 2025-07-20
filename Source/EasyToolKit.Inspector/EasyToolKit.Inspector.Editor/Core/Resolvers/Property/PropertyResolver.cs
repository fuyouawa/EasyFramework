using System;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyResolver : IInitializableResolver
    {
        InspectorPropertyInfo GetChildInfo(int childIndex);
        int ChildNameToIndex(string name);
        int GetChildCount();
        void ApplyChanges();
    }

    public abstract class PropertyResolver : IPropertyResolver
    {
        public InspectorProperty Property { get; private set; }
        public bool IsInitialized { get; private set; }
        private Action _changeAction;

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

        protected virtual void Initialize() {}
        protected virtual void Deinitialize() {}

        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);
        public abstract int ChildNameToIndex(string name);
        public abstract int GetChildCount();

        protected void QueueChange(Action action)
        {
            _changeAction += action;
            Property.Tree.QueueCallbackUntilRepaint(() =>
            {
                Property.Tree.SetPropertyDirty(Property);
            });
        }

        void IPropertyResolver.ApplyChanges()
        {
            if (_changeAction != null)
            {
                foreach (var target in Property.Tree.Targets)
                {
                    Undo.RecordObject(target, $"Change {Property.Info.PropertyPath} on {target.name}");
                }

                _changeAction();
                _changeAction = null;
            }
        }
    }
}
