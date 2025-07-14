using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityPropertyResolver : InspectorPropertyResolver
    {
        private SerializedProperty _serializedProperty;
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        protected override void Initialize()
        {
            _serializedProperty = Property.TryGetUnitySerializedProperty();
            if (_serializedProperty == null)
            {
                throw new InvalidOperationException();  //TODO 异常信息
            }

            var iterator = _serializedProperty.Copy();

            if (!iterator.Next(true))
            {
                return;
            }
            
            do
            {
                var info = InspectorPropertyInfo.CreateForUnityProperty(iterator, Property.Info.PropertyType);
                var field = (FieldInfo)info.MemberInfo;
                if (!field.IsPublic)
                {
                    if (field.GetCustomAttribute<HideInInspector>() != null)
                    {
                        continue;
                    }

                    if (!field.HasCustomAttribute<SerializeField>() &&
                        !field.HasCustomAttribute<OdinSerializeAttribute>() &&
                        !field.HasCustomAttribute<ShowInInspectorAttribute>())
                    {
                        continue;
                    }

                    if (!field.FieldType.IsValueType &&
                        !field.FieldType.HasCustomAttribute<SerializableAttribute>())
                    {
                        continue;
                    }
                }
                _propertyInfos.Add(info);
            } while (iterator.Next(false));
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            return _propertyInfos[childIndex];
        }

        public override int ChildNameToIndex(string name)
        {
            return _propertyInfos.FindIndex(info => info.PropertyName == name);
        }

        public override int GetChildCount()
        {
            return _propertyInfos.Count;
        }
    }
}
