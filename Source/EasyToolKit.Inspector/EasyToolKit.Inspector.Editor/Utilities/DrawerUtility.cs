using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class DrawerUtility
    {
        public static readonly GUIContent NotSupportedContent = new GUIContent("Not supported yet!");

        private static readonly TypeMatcher TypeMatcher = new TypeMatcher();

        private static readonly Dictionary<Type, Type> UnityPropertyDrawerTypesByDrawnType =
            new Dictionary<Type, Type>();

        private static readonly FieldInfo CustomPropertyDrawerTypeFieldInfo =
            typeof(UnityEditor.CustomPropertyDrawer).GetField("m_Type", BindingFlagsHelper.AllInstance());

        static DrawerUtility()
        {
            var easyDrawerTypes = new List<Type>();

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(asm => asm.GetTypes())
                         .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract))
            {
                if (type.IsInheritsFrom<IEasyDrawer>())
                {
                    easyDrawerTypes.Add(type);
                }
                else if (type.IsInheritsFrom<UnityEditor.PropertyDrawer>())
                {
                    var attr = type.GetCustomAttribute(typeof(UnityEditor.CustomPropertyDrawer));
                    if (attr != null)
                    {
                        var drawnType = (Type)CustomPropertyDrawerTypeFieldInfo.GetValue(attr);
                        if (drawnType != null)
                        {
                            UnityPropertyDrawerTypesByDrawnType[drawnType] = type;
                        }
                    }
                }
            }

            TypeMatcher.SetTypeMatchIndices(easyDrawerTypes
                .OrderByDescending(GetDrawerPriority)
                .Select((type, i) =>
                {
                    var index = new TypeMatchIndex(type, easyDrawerTypes.Count - i, null);
                    if (type.IsImplementsOpenGenericType(typeof(EasyValueDrawer<>)))
                    {
                        index.Targets = type.GetArgumentsOfInheritedOpenGenericType(typeof(EasyValueDrawer<>));
                    }
                    else if (type.IsImplementsOpenGenericType(typeof(EasyAttributeDrawer<>)))
                    {
                        index.Targets = type.GetArgumentsOfInheritedOpenGenericType(typeof(EasyAttributeDrawer<>));
                    }

                    return index;
                }));

            TypeMatcher.AddMatchRule(GetMatchedType);
        }

        private static Type GetMatchedType(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            if (targets.Length != 1) return null;
            if (!matchIndex.Targets[0].IsGenericTypeDefinition) return null;

            var valueType = targets[0];
            var argType = matchIndex.Targets[0];

            // 如果参数不是泛型参数，并且是个不包含泛型参数的类型
            // 用于判断当前序列化器的参数必须是个具体类型
            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                if (argType == valueType)
                {
                    return matchIndex.Type;
                }

                return null;
            }

            var missingArgs = argType.ResolveMissingGenericTypeArguments(valueType, false);
            if (missingArgs.Length == 0)
                return null;

            if (matchIndex.Type.AreGenericConstraintsSatisfiedBy(missingArgs))
            {
                var serializeType = matchIndex.Type.MakeGenericType(missingArgs);
                return serializeType;
            }

            return null;
        }

        public static bool IsDefinedUnityPropertyDrawer(Type targetType)
        {
            if (UnityPropertyDrawerTypesByDrawnType.ContainsKey(targetType))
            {
                return true;
            }

            //TODO 通过CustomPropertyDrawer判断是否需要检查继承
            foreach (var drawnType in UnityPropertyDrawerTypesByDrawnType.Keys)
            {
                if (drawnType.IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<TypeMatchResult> GetDefaultPropertyDrawerTypes(InspectorProperty property)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetCachedMatches(Type.EmptyTypes),
            };

            if (property.Info.PropertyType != null)
            {
                resultsList.Add(TypeMatcher.GetCachedMatches(property.Info.PropertyType));
            }

            foreach (var attribute in property.GetAttributes())
            {
                resultsList.Add(TypeMatcher.GetCachedMatches(attribute.GetType()));

                // if (property.ValueEntry != null)
                // {
                //     resultsList.Add(TypeMatcher.GetCachedMatches(attribute.GetType(), property.ValueEntry.ValueType));
                // }
            }

            return TypeMatcher.GetCachedMergedResults(resultsList)
                .Where(result => CanDrawProperty(result.MatchedType, property));
        }

        public static bool CanDrawProperty(Type drawerType, InspectorProperty property)
        {
            var drawer = (IEasyDrawer)FormatterServices.GetUninitializedObject(drawerType);
            return drawer.CanDrawProperty(property);
        }

        public static DrawerPriority GetDrawerPriority(Type drawerType)
        {
            var priority = DrawerPriority.DefaultPriority;

            var priorityAttribute = drawerType.GetCustomAttribute<DrawerPriorityAttribute>();
            if (priorityAttribute != null)
            {
                priority = priorityAttribute.Priority;
            }

            if (priority == DrawerPriority.DefaultPriority)
            {
                if (drawerType.IsImplementsOpenGenericType(typeof(EasyAttributeDrawer<>)))
                {
                    priority = DrawerPriority.AttributePriority;
                }
                else
                {
                    priority = DrawerPriority.ValuePriority;
                }
            }

            return priority;
        }
    }
}
