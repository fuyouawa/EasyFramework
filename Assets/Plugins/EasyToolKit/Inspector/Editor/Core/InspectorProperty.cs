using Codice.Client.BaseCommands.BranchExplorer;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorProperty
    {
        private SerializedProperty _serializedProperty;
        private InspectorProperty _parent;
        private InspectorPropertyChildren _children;
        private InspectorPropertyTree _tree;
        private InspectorPropertyInfo _info;
        private int _index;

        public SerializedProperty SerializedProperty => _serializedProperty;
        public InspectorProperty Parent => _parent;
        public InspectorPropertyTree Tree => _tree;
        public InspectorPropertyChildren Children => _children;
        public InspectorPropertyInfo Info => _info;
        public int Index => _index;

        public string Name => _serializedProperty.name;
        public string DisplayName => _serializedProperty.displayName;

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

            var property = new InspectorProperty();
            property._tree = tree;
            property._parent = parent;
            property._info = info;
            property._index = index;

            if (!isRoot)
            {
                property._children = new InspectorPropertyChildren(property);
            }

            return property;
        }

        public void Draw()
        {
        }

        public void Draw(GUIContent label)
        {
        }
    }
}
