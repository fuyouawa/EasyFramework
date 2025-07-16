using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EasyToolKit.Core
{
    public static class ReflectionExtensions
    {
        public static bool HasCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            return member.GetCustomAttributes<T>().Any();
        }

        public static bool HasCustomAttribute<T>(this MemberInfo member, bool inherit) where T : Attribute
        {
            return member.GetCustomAttributes<T>(inherit).Any();
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
                method.GetParameters().Select(x => $"{TypeExtensions.GetAliases(x.ParameterType)} {x.Name}"));
        }

        // public static object GetObjectValue(this object obj, string name, BindingFlags flags)
        // {
        //     var t = obj.GetType();
        //     var f = t.GetField(name, flags);
        //     if (f != null)
        //     {
        //         return f.GetValue(obj);
        //     }
        //     var p = t.GetProperty(name, flags);
        //     if (p != null)
        //     {
        //         return p.GetValue(obj);
        //     }
        //
        //     throw new ArgumentException($"No field or property name:{name}");
        // }
        //
        // public static Type GetObjectValueType(this object obj, string name, BindingFlags flags)
        // {
        //     var t = obj.GetType();
        //     var f = t.GetField(name, flags);
        //     if (f != null)
        //     {
        //         return f.FieldType;
        //     }
        //     var p = t.GetProperty(name, flags);
        //     if (p != null)
        //     {
        //         return p.PropertyType;
        //     }
        //
        //     throw new ArgumentException($"No field or property name:{name}");
        // }

        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            if (member is MethodInfo method)
            {
                return method.ReturnType;
            }

            throw new NotSupportedException();
        }

        public static object GetMemberValue(this MemberInfo member, object obj)
        {
            if (member is FieldInfo field)
                return field.GetValue(obj);

            return member is PropertyInfo property
                ? property.GetGetMethod(true).Invoke(obj, null)
                : throw new ArgumentException($"Can't get the value of '{member.DeclaringType}.{member.Name}'");
        }

        public static void SetMemberValue(this MemberInfo member, object obj, object value)
        {
            if (member is FieldInfo field)
            {
                field.SetValue(obj, value);
            }
            else
            {
                var methodInfo = member is PropertyInfo property
                    ? property.GetSetMethod(true)
                    : throw new ArgumentException($"Can't set the value of '{member.DeclaringType}.{member.Name}'");

                if (methodInfo == null)
                    throw new ArgumentException($"Property '{member.DeclaringType}.{member.Name}' has no setter");

                methodInfo.Invoke(obj, new[] { value });
            }
        }

        // public static void SetObjectValue(this object obj, string name, object val, BindingFlags flags)
        // {
        //     var t = obj.GetType();
        //     var f = t.GetField(name, flags);
        //     if (f != null)
        //     {
        //         f.SetValue(obj, val);
        //     }
        //     var p = t.GetProperty(name, flags);
        //     if (p != null)
        //     {
        //         p.SetValue(obj, val);
        //     }
        //
        //     throw new ArgumentException($"No field or property name:{name}");
        // }
    }
}
