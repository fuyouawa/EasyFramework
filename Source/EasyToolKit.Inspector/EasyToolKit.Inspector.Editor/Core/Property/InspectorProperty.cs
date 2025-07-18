using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty
    {
        private Attribute[] _attributes;
        private string _niceName;
        private InspectorPropertyState _state;
        private bool? _isSelfReadOnlyCache;

        public InspectorProperty Parent { get; private set; }
        public InspectorPropertyTree Tree { get; }

        [CanBeNull] public InspectorPropertyChildren Children { get; }
        public InspectorPropertyInfo Info { get; }

        public InspectorPropertyState State
        {
            get
            {
                if (_state == null)
                {
                    _state = new InspectorPropertyState(this);
                }
                return _state;
            }
        }

        [CanBeNull] public IInspectorValueEntry ValueEntry { get; }

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
        
        [CanBeNull] public InspectorPropertyResolver ChildrenResolver { get; private set; }
        public DrawerChainResolver DrawerChainResolver { get; private set; }
        [CanBeNull] public AttributeAccessorResolver AttributeAccessorResolver { get; private set; }

        internal InspectorProperty(InspectorPropertyTree tree, InspectorProperty parent, InspectorPropertyInfo info,
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

            if (Info.DefaultDrawerChainResolverType != null)
            {
                SetDrawerChainResolver(Info.DefaultDrawerChainResolverType);
            }

            if (Info.DefaultAttributeAccessorResolver != null)
            {
                SetAttributeAccessorResolver(Info.DefaultAttributeAccessorResolver);
            }
            
            if (Info.AllowChildren)
            {
                Children = new InspectorPropertyChildren(this);
                SetChildrenResolver(Info.DefaultChildrenResolverType);
            }

            if (info.ValueAccessor != null)
            {
                var entryType = typeof(InspectorValueEntry<>).MakeGenericType(info.ValueAccessor.ValueType);
                ValueEntry = (IInspectorValueEntry)entryType.CreateInstance(this);
            }
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

        public void SetChildrenResolver(Type resolverType)
        {
            ChildrenResolver = InspectorPropertyResolver.Create(resolverType, this);
            Refresh();
        }

        public void SetDrawerChainResolver(Type resolverType)
        {
            DrawerChainResolver = DrawerChainResolver.Create(resolverType, this);
            Refresh();
        }

        public void SetAttributeAccessorResolver(Type resolverType)
        {
            AttributeAccessorResolver = AttributeAccessorResolver.Create(resolverType, this);
            Refresh();
        }

        public void Refresh()
        {
            _attributes = null;
            _isSelfReadOnlyCache = null;
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
            if (AttributeAccessorResolver == null)
            {
                return _attributes ??= new Attribute[0];
            }

            if (_attributes == null)
            {
                var total = new List<Attribute>();
                var accessors = AttributeAccessorResolver.GetAttributeAccessors();
                foreach (var accessor in accessors)
                {
                    total.AddRange(accessor.GetAttributes());
                }

                _attributes = total.ToArray();
            }

            return _attributes;
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
    }
}
