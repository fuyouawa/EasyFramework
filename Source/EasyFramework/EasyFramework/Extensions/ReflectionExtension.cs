using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EasyFramework
{
    public static class ReflectionExtension
    {
        public static readonly BindingFlags AllBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        public static readonly BindingFlags NoPublicBindingFlags =
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

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


        public static bool HasCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any();
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
            return type.GetPropertyValue<T>(propertyName, AllBindingFlags, target);
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

        public static object InvokeMethod(this Type type, string methodName, BindingFlags flags, object target, params object[] args)
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
            return type.InvokeMethod(methodName, AllBindingFlags, target, args);
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
            type.AddEvent(eventName, AllBindingFlags, target, func);
        }

        public static Delegate CreateDelegate(this MethodInfo method, object target)
        {
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

            Type funcType;
            if (method.ReturnType == typeof(void))
            {
                funcType = Expression.GetActionType(parameterTypes);
            }
            else
            {
                funcType = Expression.GetFuncType(parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
            }

            return Delegate.CreateDelegate(funcType, target, method);
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttributes<T>().Any();
        }

        public static IEnumerable<Type> GetAllTypes(this IEnumerable<Assembly> assemblies)
        {
            return assemblies.SelectMany(a => a.GetTypes());
        }

        public static Type FindType(this IEnumerable<Assembly> assemblies, Func<Type, bool> predicate)
        {
            return assemblies.GetAllTypes().FirstOrDefault(predicate);
        }

        public static Type FindTypeByName(this IEnumerable<Assembly> assemblies, string fullName)
        {
            return assemblies.GetAllTypes().FirstOrDefault(t => t.FullName == fullName);
        }

        public static object InvokePrivateMethod(this object target, string methodName, params object[] args)
        {
            return target.GetType().InvokeMethod(methodName, NoPublicBindingFlags, target, args);
        }

        public static string GetSignature(this MemberInfo member)
        {
            var sb = new StringBuilder();

            // Append the member type (e.g., Method, Property, Field, etc.)
            sb.Append(member.MemberType.ToString());
            sb.Append(" ");

            // Append the member's declaring type (including namespace)
            sb.Append(member.DeclaringType.FullName);
            sb.Append(".");

            // Append the member name
            sb.Append(member.Name);

            // If the member is a method, append parameter types and return type
            if (member is MethodInfo methodInfo)
            {
                sb.Append($"({GetMethodParametersSignature(methodInfo)}) : ");
                sb.Append(methodInfo.ReturnType.FullName);
            }
            else if (member is PropertyInfo propertyInfo)
            {
                // If the member is a property, append the property type
                sb.Append(" : ");
                sb.Append(propertyInfo.PropertyType.FullName);
            }
            else if (member is FieldInfo fieldInfo)
            {
                // If the member is a field, append the field type
                sb.Append(" : ");
                sb.Append(fieldInfo.FieldType.FullName);
            }
            else if (member is EventInfo eventInfo)
            {
                // If the member is an event, append the event handler type
                sb.Append(" : ");
                sb.Append(eventInfo.EventHandlerType.FullName);
            }

            return sb.ToString();
        }


        public static string GetMethodParametersSignature(this MethodInfo method)
        {
            return string.Join(", ",
                method.GetParameters().Select(x => $"{TypeExtension.GetAliases(x.ParameterType)} {x.Name}"));
        }


        public static bool TryGetValueType(this MemberInfo member, out Type type)
        {
            if (member is MethodInfo m)
            {
                type = m.ReturnType;
            }
            else if (member is FieldInfo f)
            {
                type = f.FieldType;
            }
            else if (member is PropertyInfo p)
            {
                type = p.PropertyType;
            }
            else
            {
                type = null;
                return false;
            }
            return true;
        }
        
        public static object GetObjectValue(this object obj, string name, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                return f.GetValue(obj);
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                return p.GetValue(obj);
            }

            throw new ArgumentException($"No field or property name:{name}");
        }
        
        public static Type GetObjectValueType(this object obj, string name, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                return f.FieldType;
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                return p.PropertyType;
            }

            throw new ArgumentException($"No field or property name:{name}");
        }
        
        public static void SetObjectValue(this object obj, string name, object val, BindingFlags flags)
        {
            var t = obj.GetType();
            var f = t.GetField(name, flags);
            if (f != null)
            {
                f.SetValue(obj, val);
            }
            var p = t.GetProperty(name, flags);
            if (p != null)
            {
                p.SetValue(obj, val);
            }

            throw new ArgumentException($"No field or property name:{name}");
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return Array.Exists(type.GetInterfaces(), t => t == interfaceType);
        }
    }
}
