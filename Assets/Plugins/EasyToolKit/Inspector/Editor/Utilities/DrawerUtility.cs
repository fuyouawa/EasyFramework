using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public static class DrawerUtility
    {
        private static readonly TypeMatcher TypeMatcher = new TypeMatcher(false);
        private static readonly List<TypeMatchResult[]> CachedQueryResults = new List<TypeMatchResult[]>();

        static DrawerUtility()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.IsSubclassOf(typeof(EasyDrawer)))
                .ToArray();

            TypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var index = new TypeMatchIndex(type, 0, null);
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


        public static void GetDefaultPropertyDrawers(InspectorProperty property, List<TypeMatchResult> results)
        {
            results.Clear();

            var queries = CachedQueryResults;
            queries.Clear();

            queries.Add(TypeMatcher.Match(Type.EmptyTypes));


        }
    }
}
