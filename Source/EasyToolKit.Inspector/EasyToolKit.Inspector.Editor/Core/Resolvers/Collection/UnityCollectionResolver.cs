using EasyToolKit.Core.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    //TODO UnityCollectionResolver在多选情况下有问题
    public class UnityCollectionResolver<TElement> : OrderedCollectionResolverBase<TElement>
    {
        private SerializedProperty _serializedProperty;

        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        private static readonly Func<SerializedProperty, TElement> ElementValueGetter =
            SerializedPropertyUtility.GetValueGetter<TElement>();

        private static readonly Action<SerializedProperty, TElement> ElementValueSetter =
            SerializedPropertyUtility.GetValueSetter<TElement>();


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
                    throw new InvalidOperationException(); //TODO 异常信息
                }
            }
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForUnityProperty(
                _serializedProperty.GetArrayElementAtIndex(childIndex),
                Property.Info.PropertyType,
                ElementType);

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException();
        }

        public override int GetChildCount()
        {
            return _serializedProperty.arraySize;
        }

        protected override void InsertElement(int targetIndex, TElement value)
        {
            InsertElementAt(targetIndex, _serializedProperty.arraySize, value);
        }

        protected override void RemoveElement(int targetIndex, TElement value)
        {
            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                var elementValue = ElementValueGetter(element);
                if (value.Equals(elementValue))
                {
                    RemoveElementAt(targetIndex, i);
                    return;
                }
            }
        }

        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            _serializedProperty.InsertArrayElementAtIndex(index);
            var element = _serializedProperty.GetArrayElementAtIndex(index);
            ElementValueSetter(element, value);
        }

        protected override void RemoveElementAt(int targetIndex, int index)
        {
            _serializedProperty.DeleteArrayElementAtIndex(index);
        }

        protected override void MoveElemenetAt(int targetIndex, int sourceIndex, int destinationIndex)
        {
            _serializedProperty.MoveArrayElement(sourceIndex, destinationIndex);
        }
    }
}
