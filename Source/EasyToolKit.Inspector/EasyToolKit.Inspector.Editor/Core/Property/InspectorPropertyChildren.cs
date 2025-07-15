using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorPropertyChildren
    {
        private readonly InspectorProperty _property;
        private readonly Dictionary<int, InspectorProperty> _childrenByIndex = new Dictionary<int, InspectorProperty>();
        public int Count => _property.PropertyResolver.GetChildCount();

        public InspectorProperty this[int index] => Get(index);
        public InspectorProperty this[string name] => Get(name);

        internal InspectorPropertyChildren(InspectorProperty property)
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
                return child;
            }

            child = new InspectorProperty(_property.Tree, _property, _property.PropertyResolver.GetChildInfo(childIndex), childIndex);
            _childrenByIndex[childIndex] = child;
            Update();
            return child;
        }

        public InspectorProperty Get([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (Count == 0)
                return null;

            return Get(_property.PropertyResolver.ChildNameToIndex(name));
        }

        internal void Update()
        {
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
