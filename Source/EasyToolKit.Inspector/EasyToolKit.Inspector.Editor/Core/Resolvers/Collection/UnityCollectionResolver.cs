using EasyToolKit.Core.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityCollectionResolver: OrderedCollectionResolverBase
    {
        public override Type ElementType { get; }

        private SerializedProperty _serializedProperty;
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex = new Dictionary<int, InspectorPropertyInfo>();

        private readonly Func<SerializedProperty, object> ElementGetter;
        private readonly Action<SerializedProperty, object> ElementSetter;

        public UnityCollectionResolver(Type elementType)
        {
            ElementType = elementType;
            ElementGetter = SerializedPropertyUtility.GetWeakValueGetter(elementType);
            ElementSetter = SerializedPropertyUtility.GetWeakValueSetter(elementType);
        }

        protected override void Initialize()
        {
            if (Property.Info.IsLogicRoot)
            {
                _serializedProperty = Property.Tree.SerializedObject.GetIterator();
            }
            else
            {
                _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.Info.PropertyPath);
                if (_serializedProperty == null)
                {
                    throw new InvalidOperationException();  //TODO 异常信息
                }
            }
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForUnityArrayElement(
                _serializedProperty.GetArrayElementAtIndex(childIndex),
                childIndex,
                Property.Info.TypeOfProperty,
                ElementType);

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        public override int ChildNameToIndex(string name)
        {
            throw new NotImplementedException();
        }

        public override int GetChildCount()
        {
            return _serializedProperty.arraySize;
        }
        
        public override void InsertElement(object value)
        {
            InsertElementAt(_serializedProperty.arraySize, value);
        }

        public override void RemoveElement(object value)
        {
            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                var elementValue = ElementGetter(element);
                if (value.Equals(elementValue))
                {
                    RemoveElementAt(i);
                    return;
                }
            }
        }

        public override void InsertElementAt(int index, object value)
        {
            _serializedProperty.InsertArrayElementAtIndex(index);
            var element = _serializedProperty.GetArrayElementAtIndex(index);
            ElementSetter(element, value);
        }

        public override void RemoveElementAt(int index)
        {
            _serializedProperty.DeleteArrayElementAtIndex(index);
        }

        public override void MoveElemenetAt(int sourceIndex, int destinationIndex)
        {
            _serializedProperty.MoveArrayElement(sourceIndex, destinationIndex);
        }
    }
}
