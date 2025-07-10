using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class InspectorPropertyChildren : IReadOnlyList<InspectorProperty>
    {
        private InspectorProperty _property;
        private List<InspectorProperty> _children;

        public int Count => _children.Count;
        public InspectorProperty this[int index] => _children[index];
        public InspectorProperty this[string name] => _children.Find(property => property.Name == name);

        internal InspectorPropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            _property = property;

            _children = new List<InspectorProperty>();
            var tempSerializedProperty = _property.SerializedProperty.Copy();
            if (!tempSerializedProperty.NextVisible(true))
            {
                return;
            }
            
            var parent = _property == _property.Tree.RootProperty ? null : _property;
            int index = 0;
            do
            {
                _children.Add(InspectorProperty.Create(_property.Tree, parent, GetInfo(tempSerializedProperty), index, false));
                index++;
            } while (tempSerializedProperty.NextVisible(false));
        }

        private InspectorPropertyInfo GetInfo(SerializedProperty serializedProperty)
        {
            return new InspectorPropertyInfo();
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
