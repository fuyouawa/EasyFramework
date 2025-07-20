using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyChildren
    {
        private readonly InspectorProperty _property;
        private readonly Dictionary<int, InspectorProperty> _childrenByIndex = new Dictionary<int, InspectorProperty>();
        public int Count => _property.ChildrenResolver.GetChildCount();

        public InspectorProperty this[int index] => Get(index);
        public InspectorProperty this[string name] => Get(name);

        internal PropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;
        }

        public InspectorProperty Get(int childIndex)
        {
            if (childIndex < 0 || childIndex > Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (_childrenByIndex.TryGetValue(childIndex, out var child))
            {
                child.Update();
                return child;
            }

            child = new InspectorProperty(_property.Tree, _property, _property.ChildrenResolver.GetChildInfo(childIndex), childIndex);
            _childrenByIndex[childIndex] = child;
            child.Update();
            return child;
        }

        public InspectorProperty Get([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (Count == 0)
                return null;

            return Get(_property.ChildrenResolver.ChildNameToIndex(name));
        }

        internal void Update()
        {
        }

        public void Refresh()
        {
            _childrenByIndex.Clear();
        }

        public IEnumerable<InspectorProperty> Recurse()
        {
            for (var i = 0; i < Count; i++)
            {
                var child = this[i];
                yield return child;

                if (child.Children != null)
                {
                    foreach (var subChild in child.Children.Recurse())
                    {
                        yield return subChild;
                    }
                }
            }
        }
    }
}
