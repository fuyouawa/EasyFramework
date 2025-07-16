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
        private DrawerChain _drawerChain;
        private Attribute[] _attributes;
        private string _niceName;
        private InspectorPropertyState _state;

        public InspectorProperty Parent { get; private set; }
        public InspectorPropertyTree Tree { get; }

        [CanBeNull]
        public InspectorPropertyChildren Children { get; }
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

        [CanBeNull]
        public IInspectorValueEntry ValueEntry { get; }

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

        public GUIContent Label { get; set; }
        
        public InspectorPropertyResolver PropertyResolver { get; private set; }
        public DrawerChainResolver DrawerChainResolver { get; private set; }
        public AttributeAccessorResolver AttributeAccessorResolver { get; private set; }

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

            ChangeDrawerChainResolver(Info.DefaultDrawerChainResolverType);
            ChangeAttributeAccessorResolver(Info.DefaultAttributeAccessorResolver);
            
            if (Info.PropertyType != null && !Info.PropertyType.IsBasic())
            {
                Children = new InspectorPropertyChildren(this);
                ChangePropertyResolver(Info.DefaultPropertyResolverType);
            }

            if (info.ValueAccessor != null)
            {
                var entryType = typeof(InspectorValueEntry<>).MakeGenericType(info.ValueAccessor.ValueType);
                ValueEntry = (IInspectorValueEntry)entryType.CreateInstance(this);
            }
        }

        internal void Update()
        {
            if (ValueEntry != null)
            {
                ValueEntry.Update();
            }

            if (Children != null)
            {
                Children.Update();
            }
        }

        public void ChangePropertyResolver(Type resolverType)
        {
            PropertyResolver = InspectorPropertyResolver.Create(resolverType, this);
            Refresh();
        }

        public void ChangeDrawerChainResolver(Type resolverType)
        {
            DrawerChainResolver = DrawerChainResolver.Create(resolverType, this);
            Refresh();
        }

        public void ChangeAttributeAccessorResolver(Type resolverType)
        {
            AttributeAccessorResolver = AttributeAccessorResolver.Create(resolverType, this);
            Refresh();
        }

        public object GetDeclaringObject(int index)
        {
            if (Parent == null)
            {
                return Tree.Targets[index];
            }
            Assert.True(Parent.ValueEntry != null);
            return Parent.ValueEntry.WeakValues[index];
        }

        public void Refresh()
        {
            _drawerChain = null;
            _attributes = null;
        }

        public DrawerChain GetDrawerChain()
        {
            return DrawerChainResolver.GetDrawerChain();
        }

        public Attribute[] GetAttributes()
        {
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
                chain.Current.DrawProperty(label);
            }
        }
    }
}
