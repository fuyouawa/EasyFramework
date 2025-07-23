using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolKit.Core;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorAttributeUtility
    {
        private static readonly RegisterGroupAttributeScopeAttribute[] _groupAttributeScopes;

        private static readonly Dictionary<Type, Type> EndGroupAttributeTypeCache =
            new Dictionary<Type, Type>();

        private static readonly Dictionary<Type, Type> BeginGroupAttributeTypeCache =
            new Dictionary<Type, Type>();

        static InspectorAttributeUtility()
        {
            _groupAttributeScopes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetCustomAttributes<RegisterGroupAttributeScopeAttribute>())
                .ToArray();
        }

        public static Type GetCorrespondGroupAttributeType([NotNull] Type groupAttributeType)
        {
            if (groupAttributeType == null)
                throw new ArgumentNullException(nameof(groupAttributeType));

            if (groupAttributeType.IsInheritsFrom<BeginGroupAttribute>())
            {
                return GetCorrespondEndGroupAttributeType(groupAttributeType);
            }

            if (groupAttributeType.IsInheritsFrom<EndGroupAttribute>())
            {
                return GetCorrespondStartGroupAttributeType(groupAttributeType);
            }

            throw new ArgumentException();  //TODO 异常信息
        }

        private static Type GetCorrespondEndGroupAttributeType(Type beginGroupAttributeType)
        {
            if (EndGroupAttributeTypeCache.TryGetValue(beginGroupAttributeType, out var type))
            {
                return type;
            }

            var scope = _groupAttributeScopes.FirstOrDefault(scope => scope.BeginGroupAttributeType == beginGroupAttributeType);
            if (scope == null)
            {
                throw new InvalidOperationException();  //TODO 异常信息
            }
            type = scope.EndGroupAttributeType;
            EndGroupAttributeTypeCache[beginGroupAttributeType] = type;
            return type;
        }

        private static Type GetCorrespondStartGroupAttributeType(Type endGroupAttributeType)
        {
            if (BeginGroupAttributeTypeCache.TryGetValue(endGroupAttributeType, out var type))
            {
                return type;
            }

            var scope = _groupAttributeScopes.FirstOrDefault(scope => scope.EndGroupAttributeType == endGroupAttributeType);
            if (scope == null)
            {
                throw new InvalidOperationException();  //TODO 异常信息
            }
            type = scope.BeginGroupAttributeType;
            BeginGroupAttributeTypeCache[endGroupAttributeType] = type;
            return type;
        }
    }
}
