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
            var memberInfos = Property.ValueEntry.ValueType.GetMembers(BindingFlagsHelper.AllInstance());

            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo is FieldInfo fieldInfo)
                {
                    if (fieldInfo.HasCustomAttribute<HideInInspector>())
                    {
                        continue;
                    }

                    var definedShowInInspector = fieldInfo.HasCustomAttribute<ShowInInspectorAttribute>();
                    if (!InspectorPropertyInfoUtility.IsSerializableField(fieldInfo) && !definedShowInInspector)
                    {
                        continue;
                    }

                    if (!definedShowInInspector)
                    {
                        if (!fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                            !fieldInfo.FieldType.IsValueType &&
                            !fieldInfo.FieldType.HasCustomAttribute<SerializableAttribute>())
                        {
                            continue;
                        }
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

        protected override void Deinitialize()
        {
            _propertyInfos.Clear();
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