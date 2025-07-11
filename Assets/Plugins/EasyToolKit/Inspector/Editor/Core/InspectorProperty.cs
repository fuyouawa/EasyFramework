using Codice.Client.BaseCommands.BranchExplorer;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty
    {
        private DrawerChain _drawerChain;

        public InspectorProperty Parent { get; private set; }
        public InspectorPropertyTree Tree { get; private set; }
        public InspectorPropertyChildren Children { get; private set; }
        public InspectorPropertyInfo Info { get; private set; }
        public int Index { get; private set; }

        public string Name => Info.SerializedProperty.name;
        public string DisplayName => Info.SerializedProperty.displayName;

        public GUIContent Label { get; set; }

        internal static InspectorProperty Create(InspectorPropertyTree tree, InspectorProperty parent, InspectorPropertyInfo info, int index, bool isRoot)
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

            var property = new InspectorProperty
            {
                Tree = tree,
                Parent = parent,
                Info = info,
                Index = index
            };

            property.Label = new GUIContent(property.DisplayName);
            
            property.Children = new InspectorPropertyChildren(property);
            return property;
        }

        internal void Update()
        {
            Children.Update();
        }

        public void Refresh()
        {
            _drawerChain = null;
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
