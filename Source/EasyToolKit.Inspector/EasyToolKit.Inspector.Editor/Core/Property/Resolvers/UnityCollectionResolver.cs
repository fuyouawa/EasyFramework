using System;
using System.Collections.Generic;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityCollectionResolver<TCollection, TElement> : InspectorCollectionResolver<TCollection, TElement>
        where TCollection : ICollection<TElement>
    {
        private SerializedProperty _serializedProperty;
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex = new Dictionary<int, InspectorPropertyInfo>();

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
                typeof(TCollection),
                typeof(TElement));

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

        public override void RemoveElement(int index)
        {
            _serializedProperty.DeleteArrayElementAtIndex(index);
        }

        public override void InsertElement(int index)
        {
            _serializedProperty.InsertArrayElementAtIndex(index);
        }

        public override void MoveElement(int sourceIndex, int destinationIndex)
        {
            _serializedProperty.MoveArrayElement(sourceIndex, destinationIndex);
        }
    }
}
