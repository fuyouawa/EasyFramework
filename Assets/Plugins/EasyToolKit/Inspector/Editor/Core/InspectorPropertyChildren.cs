using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorPropertyChildren : IReadOnlyList<InspectorProperty>
    {
        private InspectorProperty _property;
        private List<InspectorProperty> _children;

        public int Count
        {
            get
            {
                EnsureInitializeChildren();
                return _children.Count;
            }
        }

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

        public InspectorProperty Get(int index)
        {
            EnsureInitializeChildren();

            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            var result = _children[index];
            result.Update();
            return result;
        }

        public InspectorProperty Get([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (Count == 0)
                return null;

            var index = _children.FindIndex(property => property.Name == name);
            
            if (index >= 0 && index < Count)
            {
                return Get(index);
            }

            return null;
        }

        internal void Update()
        {
        }

        private void EnsureInitializeChildren()
        {
            if (_children == null)
            {
                _children = new List<InspectorProperty>();
                var iterator = _property.Info.SerializedProperty.Copy();
                if (!iterator.NextVisible(true))
                {
                    return;
                }
            
                int index = 0;
                do
                {
                    _children.Add(InspectorProperty.Create(_property.Tree, _property, GetInfo(iterator), index, false));
                    index++;
                } while (iterator.NextVisible(false));
            }
        }

        private InspectorPropertyInfo GetInfo(SerializedProperty serializedProperty)
        {
            return InspectorPropertyInfo.CreateForUnityProperty(serializedProperty);
        }

        public IEnumerable<InspectorProperty> Recurse()
        {
            foreach (var child in this)
            {
                yield return child;

                foreach (var subChild in child.Children.Recurse())
                {
                    yield return subChild;
                }
            }
        }

        public IEnumerator<InspectorProperty> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
