using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Core
{
    public static class TypeExtensions
    {
        private static readonly Dictionary<Type, string> TypeAliasesByType = new Dictionary<Type, string>
        {
            { typeof(void), "void" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(object), "object" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(string), "string" }
        };

        public static string GetAliases(this Type t)
        {
            if (TypeAliasesByType.TryGetValue(t, out string alias))
            {
                return alias;
            }

            // If not found in the alias dictionary, return the full name without the namespace
            return t.IsGenericType ? GetGenericTypeName(t) : t.Name;
        }


        private static string GetGenericTypeName(this Type type)
        {
            var genericArguments = type.GetGenericArguments();
            var typeName = type.Name;
            var genericPartIndex = typeName.IndexOf('`');
            if (genericPartIndex > -1)
            {
                typeName = typeName.Substring(0, genericPartIndex);
            }

            var genericArgs = string.Join(", ", Array.ConvertAll(genericArguments, GetAliases));
            return $"{typeName}<{genericArgs}>";
        }

        public static bool IsInstantiable(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type.IsUnityObject()) return false;
            if (type.IsInterface) return false;
            if (type.IsAbstract) return false;
            if (type.IsArray) return false;
            if (type.ContainsGenericParameters) return false;

            if (type.IsPointer || type.IsByRef || type.IsGenericParameter) return false;
            if (typeof(Delegate).IsAssignableFrom(type)) return false;

            if (type.IsValueType) return true;

            var ctor = type.GetConstructor(
                BindingFlagsHelper.AllInstance,
                binder: null,
                types: Type.EmptyTypes,
                modifiers: null);
            return ctor != null;
        }


        public static object TryCreateInstance(this Type type, params object[] args)
        {
            try
            {
                return type.CreateInstance(args);
            }
            catch
            {
                return null;
            }
        }

        public static T TryCreateInstance<T>(this Type type, params object[] args)
        {
            try
            {
                return type.CreateInstance<T>(args);
            }
            catch
            {
                return default;
            }
        }

        public static object CreateInstance(this Type type, params object[] args)
        {
            if (type == null)
                return null;

            if (type == typeof(string))
                return string.Empty;

            return Activator.CreateInstance(type, args);
        }


        public static T CreateInstance<T>(this Type type, params object[] args)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"Generic type '{typeof(T)}' must be convertible by '{type}'");
            return (T)CreateInstance(type, args);
        }

        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().FirstOrDefault();
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit).FirstOrDefault();
        }


        public static bool HasCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any();
        }


        public static bool HasCustomAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            return type.GetCustomAttributes<T>(inherit).Any();
        }

        public static bool IsUnityBuiltInType(this Type type)
        {
            return type == typeof(Vector2) || type == typeof(Vector2Int) || type == typeof(Vector3) || type == typeof(Vector3Int) ||
                type == typeof(Vector4) || type == typeof(Quaternion) || type == typeof(Color) || type == typeof(Color32) ||
                type == typeof(Rect) || type == typeof(RectInt) || type == typeof(Bounds) || type == typeof(BoundsInt);
        }

        public static bool IsInteger(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFloatingPoint(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBoolean(this Type type)
        {
            return type == typeof(bool);
        }

        public static bool IsString(this Type type)
        {
            return type == typeof(string);
        }

        public static bool IsBasic([NotNull] this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsEnum || type.IsString() || type.IsBoolean() | type.IsFloatingPoint() || type.IsInteger();
        }

        public static bool IsUnityObject([NotNull] this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.IsInheritsFrom<UnityEngine.Object>();
        }

        public static bool IsBasicOrUnityObject([NotNull] this Type type)
        {
            return type.IsBasic() || type.IsUnityObject();
        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, BindingFlags flags, object target)
        {
            var property = type.GetProperty(propertyName, flags);
            if (property == null)
            {
                throw new ArgumentException(
                    $"Property '{propertyName}' with binding flags '{flags}' was not found on type '{type}'");
            }

            return (T)property.GetValue(target, null);
        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
        {
            return type.GetPropertyValue<T>(propertyName, BindingFlagsHelper.All, target);
        }

        public static MethodInfo GetMethodEx(this Type type, string methodName, BindingFlags flags, params Type[] argTypes)
        {
            return type.GetMethods(flags).FirstOrDefault(m =>
            {
                if (m.Name != methodName)
                {
                    return false;
                }

                var parameters = m.GetParameters();
                if (argTypes == null)
                {
                    return parameters.Length == 0;
                }

                if (argTypes.Length != parameters.Length)
                {
                    return false;
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!parameters[i].ParameterType.IsAssignableFrom(argTypes[i]))
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        public static object InvokeMethod(this Type type, string methodName, BindingFlags flags, object target,
            params object[] args)
        {
            var method = type.GetMethodEx(methodName, flags, args.Select(a => a.GetType()).ToArray());

            if (method == null)
            {
                throw new ArgumentException(
                    $"Method '{methodName}' with binding flags '{flags}' was not found on type '{type}'");
            }

            return method.Invoke(target, args);
        }

        public static object InvokeMethod(this Type type, string methodName, object target,
            params object[] args)
        {
            return type.InvokeMethod(methodName, BindingFlagsHelper.All, target, args);
        }

        public static void AddEvent(this Type type, string eventName, BindingFlags flags, object target, Delegate func)
        {
            var e = type.GetEvent(eventName, flags);
            if (e == null)
            {
                throw new ArgumentException(
                    $"Event '{eventName}' with binding flags '{flags}' was not found on type '{type}'");
            }

            e.GetAddMethod().Invoke(target, new object[] { func });
        }

        public static void AddEvent(this Type type, string eventName, object target, Delegate func)
        {
            type.AddEvent(eventName, BindingFlagsHelper.All, target, func);
        }

        public static Type[] GetAllBaseTypes(this Type type, bool includeInterface = true,
            bool includeTargetType = false)
        {
            var parentTypes = new List<Type>();

            if (includeTargetType)
            {
                parentTypes.Add(type);
            }

            var baseType = type.BaseType;

            while (baseType != null)
            {
                parentTypes.Add(baseType);
                baseType = baseType.BaseType;
            }

            if (includeInterface)
            {
                foreach (var i in type.GetInterfaces())
                {
                    parentTypes.Add(i);
                }
            }

            return parentTypes.ToArray();
        }

        public static bool IsInheritsFrom<T>(this Type type)
        {
            return type.IsInheritsFrom(typeof(T));
        }

        public static bool IsInheritsFrom(this Type type, Type baseType)
        {
            return ThirdParty.OdinSerializer.Utilities.TypeExtensions.InheritsFrom(type, baseType);
        }

        public static Type[] GetArgumentsOfInheritedOpenGenericType(this Type candidateType, Type openGenericType)
        {
            return ThirdParty.OdinSerializer.Utilities.TypeExtensions.GetArgumentsOfInheritedOpenGenericType(
                candidateType, openGenericType);
        }

        public static bool AreGenericConstraintsSatisfiedBy(this Type genericType, params Type[] parameters)
        {
            return ThirdParty.OdinSerializer.Utilities.TypeExtensions.AreGenericConstraintsSatisfiedBy(
                genericType, parameters);
        }

        public static bool TryInferGenericParameters(this Type genericTypeDefinition, out Type[] inferredParams, params Type[] knownParameters)
        {
            return ThirdParty.OdinSerializer.Utilities.TypeExtensions.TryInferGenericParameters(genericTypeDefinition,
                out inferredParams, knownParameters);
        }

        public static bool IsImplementsOpenGenericType(this Type candidateType, Type openGenericType)
        {
            return ThirdParty.OdinSerializer.Utilities.TypeExtensions.ImplementsOpenGenericType(candidateType,
                openGenericType);
        }

        /// <summary>
        /// <para>根据源类型（可能包含泛型参数）和目标类型，从目标类型中提取出原类型缺失的泛型参数类型。</para>
        /// <para>如果源类型是个泛型参数，则直接返回目标类型。</para>
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        /// <param name="allowInheritance">是否允许通过继承关系匹配泛型定义。</param>
        /// <returns></returns>
        public static Type[] ResolveMissingGenericTypeArguments(this Type sourceType, Type targetType,
            bool allowInheritance)
        {
            if (sourceType == null || targetType == null)
            {
                throw new ArgumentNullException();
            }

            if (sourceType.IsArray != targetType.IsArray ||
                sourceType.IsSZArray != targetType.IsSZArray)
            {
                return new Type[] { };
            }

            if (targetType.IsArray)
            {
                return new[] { targetType.GetElementType() };
            }

            if (sourceType.IsGenericParameter)
            {
                return new Type[] { targetType };
            }

            if (!sourceType.IsGenericType)
            {
                return new Type[] { };
            }

            if (!targetType.IsGenericType || sourceType.GetGenericTypeDefinition() != targetType.GetGenericTypeDefinition())
            {
                if (!allowInheritance)
                {
                    return new Type[] { };
                }
            }

            var sourceArgs = sourceType.GetGenericArguments();
            var targetArgs = targetType.GetArgumentsOfInheritedOpenGenericType(sourceType.GetGenericTypeDefinition());
            if (targetArgs.Length == 0)
            {
                return new Type[] { };
            }

            Assert.IsTrue(sourceArgs.Length == targetArgs.Length);

            var missingArgs = new List<Type>();
            for (int i = 0; i < sourceArgs.Length; i++)
            {
                if (sourceArgs[i].IsGenericParameter)
                {
                    missingArgs.Add(targetArgs[i]);
                }
            }

            return missingArgs.ToArray();
        }
    }
}
