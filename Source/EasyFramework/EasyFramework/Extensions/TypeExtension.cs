using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;

namespace EasyFramework
{
    public static class TypeExtension
    {
        public static readonly TypeCode[] IntegerTypes =
        {
            TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32,
            TypeCode.Int64, TypeCode.UInt64
        };

        public static readonly TypeCode[] FloatingPointTypes =
        {
            TypeCode.Double, TypeCode.Single, TypeCode.Decimal
        };


        private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string>
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
            if (TypeAliases.TryGetValue(t, out string alias))
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
            return Activator.CreateInstance(type);
        }


        public static T CreateInstance<T>(this Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
                throw new ArgumentException($"泛型T({typeof(T).Name})必须可以被参数type({type.Name})转换");
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

        public static bool IsIntegerType(this Type type)
        {
            return typeof(int).IsAssignableFrom(type);
        }

        public static bool IsFloatingPointType(this Type type)
        {
            return typeof(float).IsAssignableFrom(type);
        }

        public static bool IsBooleanType(this Type type)
        {
            return typeof(bool).IsAssignableFrom(type);
        }

        public static bool IsStringType(this Type type)
        {
            return typeof(string).IsAssignableFrom(type);
        }

        public static T GetPropertyValue<T>(this Type type, string propertyName, BindingFlags flags, object target)
        {
            var property = type.GetProperty(propertyName, flags);
            if (property == null)
            {
                throw new ArgumentException($"类型:\"{type.FullName}\"没有BindingFlags为:{flags}的属性:\"{propertyName}\"");
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
                throw new ArgumentException($"类型\"{type}\"中没有名为\"{methodName}\"并且\"{flags}\"的函数!");
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
                throw new ArgumentException($"类型\"{type}\"中没有名为\"{eventName}\"并且\"{flags}\"的事件!");
            }
            
            e.GetAddMethod().Invoke(target, new object[] { func });
        }

        public static void AddEvent(this Type type, string eventName, object target, Delegate func)
        {
            type.AddEvent(eventName, BindingFlagsHelper.All(), target, func);
        }

        public static Type[] GetAllBaseTypes(this Type type, bool includeInterface = true, bool includeTargetType = false)
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
                foreach (var iface in type.GetInterfaces())
                {
                    parentTypes.Add(iface);
                }
            }

            return parentTypes.ToArray();
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return Array.Exists(type.GetInterfaces(), t => t == interfaceType);
        }
    }
}
