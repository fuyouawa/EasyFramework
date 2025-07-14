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
        private readonly Dictionary<int, InspectorPropertyInfo> _childrenInfoByIndex;
        private Dictionary<int, InspectorProperty> _childrenByIndex;
        public int Count => _childrenInfoByIndex.Count;

        public InspectorProperty this[int index] => Get(index);
        public InspectorProperty this[string name] => Get(name);

        internal InspectorPropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;
            
            _childrenInfoByIndex = new Dictionary<int, InspectorPropertyInfo>();
            var iterator = _property.TryGetUnitySerializedProperty();
            if (iterator == null)
            {
                //TODO 非unity object的情况
                throw new NotImplementedException();
            }

            if (!iterator.NextVisible(true))
            {
                return;
            }
            
            int index = 0;
            do
            {
                _childrenInfoByIndex[index] =
                    InspectorPropertyInfo.CreateForUnityProperty(iterator, _property.Info.PropertyType);
                index++;
            } while (iterator.NextVisible(false));
        }

        public InspectorProperty Get(int index)
        {
            if (index < 0 || index > Count)
            {
                throw new IndexOutOfRangeException();
            }

            if (_childrenByIndex.TryGetValue(index, out var child))
            {
                return child;
            }

            child = new InspectorProperty(_property.Tree, _property, _childrenInfoByIndex[index], index);
            _childrenByIndex[index] = child;
            Update();
            return child;
        }

        public InspectorProperty Get([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (Count == 0)
                return null;

            foreach (var kv in _childrenInfoByIndex)
            {
                if (kv.Value.PropertyName == name)
                {
                    return Get(kv.Key);
                }
            }
            
            return null;
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

                foreach (var subChild in child.Children.Recurse())
                {
                    yield return subChild;
                }
            }
        }
    }
}
