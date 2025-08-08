using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericPropertyResolver : PropertyResolver
    {
        private readonly List<InspectorPropertyInfo> _propertyInfos = new List<InspectorPropertyInfo>();

        protected override void Initialize()
        {
            var memberInfos = Property.Info.PropertyType.GetMembers(BindingFlagsHelper.AllInstance());

            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo is FieldInfo fieldInfo)
                {
                    if (!fieldInfo.IsPublic)
                    {
                        if (fieldInfo.HasCustomAttribute<HideInInspector>())
                        {
                            continue;
                        }

                        var isNonSerialized = !fieldInfo.HasCustomAttribute<SerializeField>() || fieldInfo.HasCustomAttribute<NonSerializedAttribute>();

                        if (isNonSerialized && !fieldInfo.HasCustomAttribute<ShowInInspectorAttribute>())
                        {
                            continue;
                        }
                    }

                    if (!fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                        !fieldInfo.FieldType.IsValueType &&
                        !fieldInfo.FieldType.HasCustomAttribute<SerializableAttribute>())
                    {
                        continue;
                    }


                    _propertyInfos.Add(InspectorPropertyInfo.CreateForField(fieldInfo));
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    //TODO property
                }
                else if (memberInfo is MethodInfo methodInfo)
                {
                    //TODO method
                }
            }
        }

        public override int ChildNameToIndex(string name)
        {
            return _propertyInfos.FindIndex(info => info.PropertyName == name);
        }

        public override int GetChildCount()
        {
            return _propertyInfos.Count;
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            return _propertyInfos[childIndex];
        }
    }
}