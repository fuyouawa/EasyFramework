using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using System.Runtime.Serialization;

namespace EasyToolKit.Inspector.Editor
{
    public static class DrawerUtility
    {
        private static readonly TypeMatcher TypeMatcher = new TypeMatcher();


        static DrawerUtility()
        {
            var drawerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.IsSubclassOf(typeof(EasyDrawer)))
                .OrderByDescending(GetDrawerPriority)
                .ToArray();

            TypeMatcher.SetTypeMatchIndices(drawerTypes.Select((type, i) =>
            {
                var index = new TypeMatchIndex(type, drawerTypes.Length - i, null);
                if (type.ImplementsOpenGenericType(typeof(EasyValueDrawer<>)))
                {
                    index.Targets = type.GetArgumentsOfInheritedOpenGenericType(typeof(EasyValueDrawer<>));
                }
                else if (type.ImplementsOpenGenericType(typeof(EasyAttributeDrawer<>)))
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


        public static IEnumerable<TypeMatchResult> GetDefaultPropertyDrawerTypes(InspectorProperty property)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetCachedMatches(Type.EmptyTypes),
                TypeMatcher.GetCachedMatches(property.Info.ValueAccessor.ValueType)
            };

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
            var drawer = (EasyDrawer)FormatterServices.GetUninitializedObject(drawerType);
            return drawer.CanDrawProperty(property);
        }

        public static DrawerPriority GetDrawerPriority(Type drawerType)
        {
            var priority = DrawerPriority.DefaultPriority;
            
            var priorityAttribute = TypeExtension.GetCustomAttribute<DrawerPriorityAttribute>(drawerType);
            if (priorityAttribute != null)
            {
                priority = priorityAttribute.Priority;
            }

            if (priority == DrawerPriority.DefaultPriority)
            {
                if (drawerType.ImplementsOpenGenericClass(typeof(EasyAttributeDrawer<>)))
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
