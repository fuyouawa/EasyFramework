using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty
    {
        private Attribute[] _attributes;
        private string _niceName;
        private PropertyState _state;
        private bool? _isSelfReadOnlyCache;

        private IPropertyResolver _childrenResolver;
        private IDrawerChainResolver _drawerChainResolver;
        private IAttributeResolver _attributeResolver;

        public InspectorProperty Parent { get; private set; }
        public PropertyTree Tree { get; }

        [CanBeNull] public PropertyChildren Children { get; }
        public InspectorPropertyInfo Info { get; }

        public PropertyState State
        {
            get
            {
                if (_state == null)
                {
                    _state = new PropertyState(this);
                }
                return _state;
            }
        }

        [CanBeNull] public IPropertyValueEntry ValueEntry { get; }

        public int Index { get; private set; }

        public string Name => Info.PropertyName;

        public string NiceName
        {
            get
            {
                if (_niceName == null)
                {
                    _niceName = ObjectNames.NicifyVariableName(Name);
                }
                return _niceName;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                if (IsSelfReadOnly)
                {
                    return true;
                }

                if (Parent != null && Parent.IsReadOnly)
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsSelfReadOnly
        {
            get
            {
                if (_isSelfReadOnlyCache.HasValue)
                {
                    return _isSelfReadOnlyCache.Value;
                }

                if (Info.ValueAccessor != null && Info.ValueAccessor.IsReadonly)
                {
                    _isSelfReadOnlyCache = true;
                }
                else if (GetAttribute<ReadOnlyAttribute>() != null)
                {
                    _isSelfReadOnlyCache = true;
                }
                else
                {
                    _isSelfReadOnlyCache = false;
                }

                return _isSelfReadOnlyCache.Value;
            }
        }

        public GUIContent Label { get; set; }

        [CanBeNull] public IPropertyResolver ChildrenResolver
        {
            get => _childrenResolver;
            set
            {
                InitializeResolver(value);
                _childrenResolver = value;
                Refresh();
            }
        }
        
        [CanBeNull] public IDrawerChainResolver DrawerChainResolver
        {
            get => _drawerChainResolver;
            set
            {
                InitializeResolver(value);
                _drawerChainResolver = value;
                Refresh();
            }
        }
        
        [CanBeNull] public IAttributeResolver AttributeResolver 
        {
            get => _attributeResolver;
            set
            {
                InitializeResolver(value);
                _attributeResolver = value;
                Refresh();
            }
        }

        internal InspectorProperty(PropertyTree tree, InspectorProperty parent, InspectorPropertyInfo info,
            int index)
        {
            if (tree == null)
            {
                throw new ArgumentNullException(nameof(tree));
            }

            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (parent != null)
            {
                if (tree != parent.Tree)
                {
                    throw new ArgumentException("The given tree and the given parent's tree are not the same tree.");
                }

                if (index < 0 || index >= parent.Children!.Count)
                {
                    throw new IndexOutOfRangeException("The given index for the property to create is out of bounds.");
                }
            }

            Tree = tree;
            Parent = parent;
            Info = info;
            Index = index;

            Label = new GUIContent(NiceName);

            DrawerChainResolver = new DefaultDrawerChainResolver();
            AttributeResolver = new DefaultAttributeResolver();

            if (Info.AllowChildren())
            {
                ChildrenResolver = Info.GetPreferencedChildrenResolver();
                Children = new PropertyChildren(this);
            }

            if (info.ValueAccessor != null)
            {
                var entryType = typeof(PropertyValueEntry<>).MakeGenericType(info.ValueAccessor.ValueType);
                ValueEntry = (IPropertyValueEntry)entryType.CreateInstance(this);
            }
        }
        
        public LocalPersistentContext<T> GetPersistentContext<T>(string key,
            T defaultValue = default)
        {
            var key1 = TwoWaySerializationBinder.Default.BindToName(Tree.TargetType);
            var key2 = Info.PropertyPath;

            var hash = HashCode.Combine(key1, key2, key);
            return PersistentContext.GetLocal(hash, defaultValue);
        }

        internal void Update()
        {
            _isSelfReadOnlyCache = null;
            if (ValueEntry != null)
            {
                ValueEntry.Update();
            }

            if (Children != null)
            {
                Children.Update();
            }
        }

        public void Refresh()
        {
            _attributes = null;
            _isSelfReadOnlyCache = null;
            if (Children != null)
            {
                Children.Refresh();
            }
        }

        public DrawerChain GetDrawerChain()
        {
            if (DrawerChainResolver == null)
            {
                throw new InvalidOperationException($"DrawerChainResolver of property '{Info.PropertyPath}' cannot be null.");
            }
            return DrawerChainResolver.GetDrawerChain();
        }

        public Attribute[] GetAttributes()
        {
            return _attributeResolver.GetAttributes();
        }

        public T GetAttribute<T>()
            where T : Attribute
        {
            foreach (var attribute in GetAttributes())
            {
                if (attribute is T attr)
                {
                    return attr;
                }
            }

            return null;
        }

        public void Draw()
        {
            Draw(Label);
        }

        public void Draw(GUIContent label)
        {
            var chain = GetDrawerChain();
            chain.Reset();

            if (chain.MoveNext())
            {
                EditorGUI.BeginDisabledGroup(IsSelfReadOnly);
                chain.Current.DrawProperty(label);
                EditorGUI.EndDisabledGroup();
            }
        }

        private void InitializeResolver(IInitializableResolver resolver)
        {
            if (resolver != null)
            {
                if (resolver.IsInitialized)
                {
                    resolver.Deinitialize();
                }

                resolver.Property = this;
                resolver.Initialize();
            }
        }
    }
}
