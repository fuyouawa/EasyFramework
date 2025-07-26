using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityPropertyResolver : PropertyResolver
    {
        private SerializedProperty _serializedProperty;
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

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

            var iterator = _serializedProperty.Copy();
            if (!iterator.Next(true))
            {
                return;
            }
            
            do
            {
                if (!Property.Info.IsLogicRoot)
                {
                    if (!iterator.propertyPath.StartsWith(_serializedProperty.propertyPath + "."))
                    {
                        break;
                    }
                }

                var field = Property.Info.PropertyType.GetField(iterator.name, BindingFlagsHelper.AllInstance());
                if (field == null)
                {
                    continue;
                }

                if (!field.IsPublic)
                {
                    if (field.HasCustomAttribute<HideInInspector>())
                    {
                        continue;
                    }

                    if (!field.HasCustomAttribute<SerializeField>() &&
                        !field.HasCustomAttribute<OdinSerializeAttribute>() &&
                        !field.HasCustomAttribute<ShowInInspectorAttribute>())
                    {
                        continue;
                    }

                    if (field.HasCustomAttribute<NonSerializedAttribute>())
                    {
                        continue;
                    }
                }

                if (!field.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                    !field.FieldType.IsValueType &&
                    !field.FieldType.HasCustomAttribute<SerializableAttribute>())
                {
                    continue;
                }

                _propertyInfos.Add(InspectorPropertyInfo.CreateForUnityProperty(iterator, Property.Info.PropertyType, field.FieldType));
            } while (iterator.Next(false));
        }

        protected override void Deinitialize()
        {
            _propertyInfos.Clear();
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
