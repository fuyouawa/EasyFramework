using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyFramework.Core
{
    public static class TypeExtension
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


        public static object CreateInstance(this Type type)
        {
            if (type == null)
                return null;

            if (type == typeof(string))
                return string.Empty;
            return Activator.CreateInstance(type);
        }


        public static T CreateInstance<T>(this Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"Generic type '{typeof(T)}' must be convertible by '{type}'");
            return (T)CreateInstance(type);
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

        public static bool IsBasic(this Type type)
        {
            if (type == null)
                return false;

            return type.IsEnum || type.IsString() || type.IsBoolean() | type.IsFloatingPoint() || type.IsInteger();
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
            return type.GetPropertyValue<T>(propertyName, BindingFlagsHelper.All(), target);
        }

        public static MethodInfo GetMethodEx(this Type type, string methodName, BindingFlags flags, Type[] argTypes)
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
            return type.InvokeMethod(methodName, BindingFlagsHelper.All(), target, args);
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
            type.AddEvent(eventName, BindingFlagsHelper.All(), target, func);
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

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return Array.Exists(type.GetInterfaces(), t => t == interfaceType);
        }

        public static string GetNiceName(this Type type)
        {
            return $"{type.Name} ({type.Namespace})";
        }

        public static bool IsImplementsOpenGenericType(this System.Type candidateType, System.Type openGenericType)
        {
            return openGenericType.IsInterface
                ? candidateType.IsImplementsOpenGenericInterface(openGenericType)
                : candidateType.IsImplementsOpenGenericClass(openGenericType);
        }

        public static bool IsImplementsOpenGenericClass(this Type derivedType, Type openGenericClassType)
        {
            return derivedType.GetArgumentsOfInheritedOpenGenericClass(openGenericClassType).IsNotNullOrEmpty();
        }

        public static bool IsImplementsOpenGenericInterface(this Type candidateType, Type genericInterfaceType)
        {
            return candidateType.GetArgumentsOfInheritedOpenGenericInterface(genericInterfaceType).IsNotNullOrEmpty();
        }

        public static Type[] GetArgumentsOfInheritedOpenGenericType(this Type candidateType, Type openGenericType)
        {
            if (candidateType == null || openGenericType == null)
            {
                throw new ArgumentNullException();
            }

            if (openGenericType.IsInterface)
                return candidateType.GetArgumentsOfInheritedOpenGenericInterface(openGenericType);
            return candidateType.GetArgumentsOfInheritedOpenGenericClass(openGenericType);
        }

        public static Type[] GetArgumentsOfInheritedOpenGenericInterface(this Type candidateType,
            Type openGenericInterfaceType)
        {
            if (candidateType == null || openGenericInterfaceType == null)
            {
                throw new ArgumentNullException();
            }

            if (!openGenericInterfaceType.IsGenericTypeDefinition && !openGenericInterfaceType.IsInterface)
            {
                throw new ArgumentException(
                    $"Type {openGenericInterfaceType.Name} is not a generic type definition and an interface.");
            }

            if (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition() == openGenericInterfaceType)
            {
                return candidateType.GetGenericArguments();
            }

            var interfaces = candidateType.GetInterfaces();
            foreach (var i in interfaces)
            {
                if (!i.IsGenericType)
                    continue;

                var result = i.GetArgumentsOfInheritedOpenGenericInterface(openGenericInterfaceType);
                if (result.IsNotNullOrEmpty())
                    return result;
            }

            return new Type[] { };
        }

        public static Type[] GetArgumentsOfInheritedOpenGenericClass(this Type candidateType, Type openGenericClassType)
        {
            if (candidateType == null || openGenericClassType == null)
            {
                throw new ArgumentNullException();
            }

            if (!openGenericClassType.IsGenericTypeDefinition)
            {
                throw new ArgumentException($"Type '{openGenericClassType.Name}' is not a generic type definition.");
            }

            while (candidateType != null && candidateType != typeof(object))
            {
                if (candidateType.IsGenericType && candidateType.GetGenericTypeDefinition() == openGenericClassType)
                {
                    return candidateType.GetGenericArguments();
                }

                candidateType = candidateType.BaseType;
            }

            return new Type[] { };
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

            Assert.True(sourceArgs.Length == targetArgs.Length);

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

        private static readonly Dictionary<Type, Type> GenericConstraintsSatisfactionInferredParameters =
            new Dictionary<Type, Type>();

        public static bool TryInferGenericArguments(this Type genericTypeDefinition, out Type[] inferredArguments,
            params Type[] knownArguments)
        {
            if (genericTypeDefinition == null)
            {
                throw new ArgumentNullException("genericTypeDefinition");
            }

            if (knownArguments == null)
            {
                throw new ArgumentNullException("knownArguments");
            }

            if (!genericTypeDefinition.IsGenericType)
            {
                throw new ArgumentException("The genericTypeDefinition parameter must be a generic type.");
            }

            lock (GenericConstraintsSatisfactionInferredParameters)
            {
                Dictionary<Type, Type> matches = GenericConstraintsSatisfactionInferredParameters;
                matches.Clear();

                Type[] definitions = genericTypeDefinition.GetGenericArguments();

                if (!genericTypeDefinition.IsGenericTypeDefinition)
                {
                    Type[] constructedParameters = definitions;
                    genericTypeDefinition = genericTypeDefinition.GetGenericTypeDefinition();
                    definitions = genericTypeDefinition.GetGenericArguments();

                    int unknownCount = 0;

                    for (int i = 0; i < constructedParameters.Length; i++)
                    {
                        if (!constructedParameters[i].IsGenericParameter)
                        {
                            matches[definitions[i]] = constructedParameters[i];
                        }
                        else
                        {
                            unknownCount++;
                        }
                    }

                    if (unknownCount == knownArguments.Length)
                    {
                        int count = 0;

                        for (int i = 0; i < constructedParameters.Length; i++)
                        {
                            if (constructedParameters[i].IsGenericParameter)
                            {
                                constructedParameters[i] = knownArguments[count++];
                            }
                        }

                        if (genericTypeDefinition.AreGenericConstraintsSatisfiedBy(constructedParameters))
                        {
                            inferredArguments = constructedParameters;
                            return true;
                        }
                    }
                }

                if (definitions.Length == knownArguments.Length &&
                    genericTypeDefinition.AreGenericConstraintsSatisfiedBy(knownArguments))
                {
                    inferredArguments = knownArguments;
                    return true;
                }

                foreach (var type in definitions)
                {
                    if (matches.ContainsKey(type)) continue;

                    var constraints = type.GetGenericParameterConstraints();

                    foreach (var constraint in constraints)
                    {
                        foreach (var parameter in knownArguments)
                        {
                            if (!constraint.IsGenericType)
                            {
                                continue;
                            }

                            Type constraintDefinition = constraint.GetGenericTypeDefinition();

                            var constraintParams = constraint.GetGenericArguments();
                            Type[] paramParams;

                            if (parameter.IsGenericType && constraintDefinition == parameter.GetGenericTypeDefinition())
                            {
                                paramParams = parameter.GetGenericArguments();
                            }
                            else if (constraintDefinition.IsInterface || constraintDefinition.IsClass)
                            {
                                paramParams = parameter.GetArgumentsOfInheritedOpenGenericType(constraintDefinition);
                            }
                            else
                            {
                                continue;
                            }

                            matches[type] = parameter;

                            for (int i = 0; i < constraintParams.Length; i++)
                            {
                                if (constraintParams[i].IsGenericParameter)
                                {
                                    matches[constraintParams[i]] = paramParams[i];
                                }
                            }
                        }
                    }
                }

                if (matches.Count == definitions.Length)
                {
                    inferredArguments = new Type[matches.Count];

                    for (int i = 0; i < definitions.Length; i++)
                    {
                        inferredArguments[i] = matches[definitions[i]];
                    }

                    if (AreGenericConstraintsSatisfiedBy(genericTypeDefinition, inferredArguments))
                    {
                        return true;
                    }
                }

                inferredArguments = null;
                return false;
            }
        }

        public static bool AreGenericConstraintsSatisfiedBy(this Type genericType, params Type[] parameters)
        {
            if (genericType == null)
            {
                throw new ArgumentNullException("genericType");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            if (!genericType.IsGenericType)
            {
                throw new ArgumentException("The genericTypeDefinition parameter must be a generic type.");
            }

            return AreGenericConstraintsSatisfiedBy(genericType.GetGenericArguments(), parameters);
        }

        public static bool AreGenericConstraintsSatisfiedBy(Type[] definitions, Type[] parameters)
        {
            if (definitions.Length != parameters.Length)
            {
                return false;
            }

            lock (GenericConstraintsSatisfactionResolvedMap)
            {
                Dictionary<Type, Type> resolvedMap = GenericConstraintsSatisfactionResolvedMap;
                resolvedMap.Clear();

                for (int i = 0; i < definitions.Length; i++)
                {
                    Type definition = definitions[i];
                    Type parameter = parameters[i];

                    if (!definition.GenericParameterIsFulfilledBy(parameter, resolvedMap))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private static readonly Dictionary<Type, Type> GenericConstraintsSatisfactionResolvedMap =
            new Dictionary<Type, Type>();

        public static bool GenericParameterIsFulfilledBy(this Type genericParameterDefinition, Type parameterType)
        {
            lock (GenericConstraintsSatisfactionResolvedMap)
            {
                GenericConstraintsSatisfactionResolvedMap.Clear();
                return genericParameterDefinition.GenericParameterIsFulfilledBy(parameterType,
                    GenericConstraintsSatisfactionResolvedMap);
            }
        }

        private static readonly HashSet<Type> GenericConstraintsSatisfactionProcessedParams = new HashSet<Type>();

        private static bool GenericParameterIsFulfilledBy(this Type genericParameterDefinition, Type parameterType,
            Dictionary<Type, Type> resolvedMap, HashSet<Type> processedParams = null)
        {
            if (genericParameterDefinition == null)
            {
                throw new ArgumentNullException("genericParameterDefinition");
            }

            if (parameterType == null)
            {
                throw new ArgumentNullException("parameterType");
            }

            if (resolvedMap == null)
            {
                throw new ArgumentNullException("resolvedMap");
            }

            if (genericParameterDefinition.IsGenericParameter == false && genericParameterDefinition == parameterType)
            {
                return true;
            }

            if (genericParameterDefinition.IsGenericParameter == false)
            {
                return false;
            }

            if (processedParams == null)
            {
                processedParams =
                    GenericConstraintsSatisfactionProcessedParams; // This is safe because we are currently holding the lock
                processedParams.Clear();
            }

            processedParams.Add(genericParameterDefinition);

            // First, check up on the special constraint flags
            GenericParameterAttributes specialConstraints = genericParameterDefinition.GenericParameterAttributes;

            if (specialConstraints != GenericParameterAttributes.None)
            {
                // Struct constraint (must not be nullable)
                if ((specialConstraints & GenericParameterAttributes.NotNullableValueTypeConstraint) ==
                    GenericParameterAttributes.NotNullableValueTypeConstraint)
                {
                    if (!parameterType.IsValueType || (parameterType.IsGenericType &&
                                                       parameterType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                    {
                        return false;
                    }
                }
                // Class constraint
                else if ((specialConstraints & GenericParameterAttributes.ReferenceTypeConstraint) ==
                         GenericParameterAttributes.ReferenceTypeConstraint)
                {
                    if (parameterType.IsValueType)
                    {
                        return false;
                    }
                }

                // Must have a public parameterless constructor
                if ((specialConstraints & GenericParameterAttributes.DefaultConstructorConstraint) ==
                    GenericParameterAttributes.DefaultConstructorConstraint)
                {
                    if (parameterType.IsAbstract || (!parameterType.IsValueType &&
                                                     parameterType.GetConstructor(Type.EmptyTypes) == null))
                    {
                        return false;
                    }
                }
            }

            // If this parameter has already been resolved to a type, check if that resolved type is assignable with the argument type
            if (resolvedMap.ContainsKey(genericParameterDefinition))
            {
                if (!parameterType.IsAssignableFrom(resolvedMap[genericParameterDefinition]))
                {
                    return false;
                }
            }

            // Then, check up on the actual type constraints, of which there can be three kinds:
            // Type inheritance, Interface implementation and fulfillment of another generic parameter.
            Type[] constraints = genericParameterDefinition.GetGenericParameterConstraints();

            for (int i = 0; i < constraints.Length; i++)
            {
                Type constraint = constraints[i];

                // Replace resolved constraint parameters with their resolved types
                if (constraint.IsGenericParameter && resolvedMap.ContainsKey(constraint))
                {
                    constraint = resolvedMap[constraint];
                }

                if (constraint.IsGenericParameter)
                {
                    if (!constraint.GenericParameterIsFulfilledBy(parameterType, resolvedMap, processedParams))
                    {
                        return false;
                    }
                }
                else if (constraint.IsClass || constraint.IsInterface || constraint.IsValueType)
                {
                    if (constraint.IsGenericType)
                    {
                        Type constraintDefinition = constraint.GetGenericTypeDefinition();

                        Type[] constraintParams = constraint.GetGenericArguments();
                        Type[] paramParams;

                        if (parameterType.IsGenericType &&
                            constraintDefinition == parameterType.GetGenericTypeDefinition())
                        {
                            paramParams = parameterType.GetGenericArguments();
                        }
                        else
                        {
                            var o = parameterType.GetArgumentsOfInheritedOpenGenericType(constraintDefinition);
                            if (o.IsNotNullOrEmpty())
                            {
                                paramParams = o;
                            }
                            else
                            {
                                return false;
                            }
                        }

                        for (int j = 0; j < constraintParams.Length; j++)
                        {
                            var c = constraintParams[j];
                            var p = paramParams[j];

                            // Replace resolved constraint parameters with their resolved types
                            if (c.IsGenericParameter && resolvedMap.ContainsKey(c))
                            {
                                c = resolvedMap[c];
                            }

                            if (c.IsGenericParameter)
                            {
                                if (!processedParams.Contains(c) &&
                                    !GenericParameterIsFulfilledBy(c, p, resolvedMap, processedParams))
                                {
                                    return false;
                                }
                            }
                            else if (c != p && !c.IsAssignableFrom(p))
                            {
                                return false;
                            }
                        }
                    }
                    else if (!constraint.IsAssignableFrom(parameterType))
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Unknown parameter constraint type! " + constraint.GetNiceName());
                }
            }

            resolvedMap[genericParameterDefinition] = parameterType;
            return true;
        }
    }
}
