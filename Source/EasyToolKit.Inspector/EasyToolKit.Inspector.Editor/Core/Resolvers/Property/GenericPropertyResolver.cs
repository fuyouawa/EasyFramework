using System;
using System.Collections.Generic;
using System.Linq;
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
            var fieldInfos = Property.ValueEntry.ValueType.GetFields(BindingFlagsHelper.AllInstance);

            foreach (var fieldInfo in fieldInfos)
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

            var methodInfos = Property.ValueEntry.ValueType.GetMethods(BindingFlagsHelper.AllInstance);
            foreach (var methodInfo in methodInfos)
            {
                if (!methodInfo.GetCustomAttributes().Any())
                {
                    continue;
                }

                if (methodInfo.HasCustomAttribute<HideInInspector>())
                {
                    continue;
                }

                _propertyInfos.Add(InspectorPropertyInfo.CreateForMethod(methodInfo));
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