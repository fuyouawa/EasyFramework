using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty
    {
        private DrawerChain _drawerChain;
        private Attribute[] _attributes;

        public InspectorProperty Parent { get; private set; }
        public InspectorPropertyTree Tree { get; }
        public InspectorPropertyChildren Children { get; }
        public InspectorPropertyInfo Info { get; }


        [CanBeNull]
        public IInspectorValueEntry ValueEntry { get; }

        public string Path => Info.SerializedProperty.propertyPath;

        public int Index { get; private set; }

        public string Name => Info.SerializedProperty.name;
        public string DisplayName => Info.SerializedProperty.displayName;

        public GUIContent Label { get; set; }

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

                if (index < 0 || index >= parent.Children.Count)
                {
                    throw new IndexOutOfRangeException("The given index for the property to create is out of bounds.");
                }
            }

            Tree = tree;
            Parent = parent;
            Info = info;
            Index = index;

            Label = new GUIContent(DisplayName);
            Children = new InspectorPropertyChildren(this);

            if (info.ValueAccessor != null)
            {
                var entryType = typeof(InspectorValueEntry<>).MakeGenericType(info.ValueAccessor.ValueType);
                ValueEntry = (IInspectorValueEntry)entryType.CreateInstance();
            }
        }

        internal void Update()
        {
            if (ValueEntry != null)
            {
                ValueEntry.Update();
            }
            Children.Update();
        }

        public void Refresh()
        {
            _drawerChain = null;
            _attributes = null;
        }

        public DrawerChain GetDrawerChain()
        {
            if (_drawerChain == null)
            {
                _drawerChain = Tree.DrawerChainResolver.GetDrawerChain(this);
                foreach (var drawer in _drawerChain)
                {
                    drawer.Initialize(this);
                }
            }

            return _drawerChain;
        }

        public Attribute[] GetAttributes()
        {
            if (_attributes == null)
            {
                var total = new List<Attribute>();
                var accessors = Tree.AttributeAccessorsResolver.GetAttributeAccessors(this);
                foreach (var accessor in accessors)
                {
                    total.AddRange(accessor.GetAttributes());
                }

                _attributes = total.ToArray();
            }

            return _attributes;
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
