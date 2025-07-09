// using System.Collections.Generic;
// using System.Reflection;
// using System;
// using System.Linq.Expressions;
//
// namespace EasyFramework.Core
// {
//     public static class MemberAccessor
//     {
//         private static readonly Dictionary<MemberInfo, Func<object, object>> GetterCache =
//             new Dictionary<MemberInfo, Func<object, object>>();
//
//         private static readonly Dictionary<MemberInfo, Action<object, object>> SetterCache =
//             new Dictionary<MemberInfo, Action<object, object>>();
//
//         public static Func<object, object> GetMemberValueGetter(MemberInfo member)
//         {
//             if (!GetterCache.TryGetValue(member, out var getter))
//             {
//                 getter = CreateGetter(member);
//                 if (getter == null)
//                 {
//                     return null;
//                 }
//
//                 GetterCache[member] = getter;
//             }
//
//             return getter;
//         }
//
//         public static Action<object, object> GetMemberValueSetter(MemberInfo member)
//         {
//             if (!SetterCache.TryGetValue(member, out var setter))
//             {
//                 setter = CreateSetter(member);
//                 if (setter == null)
//                 {
//                     return null;
//                 }
//
//                 SetterCache[member] = setter;
//             }
//
//             return setter;
//         }
//
//         private static Func<object, object> CreateGetter(MemberInfo member)
//         {
//             var instance = Expression.Parameter(typeof(object), "target");
//
//             if (member is FieldInfo field)
//             {
//                 var fieldAccess = Expression.Field(Expression.Convert(instance, field.DeclaringType!), field);
//                 var convertResult = Expression.Convert(fieldAccess, typeof(object));
//                 return Expression.Lambda<Func<object, object>>(convertResult, instance).Compile();
//             }
//
//             if (member is PropertyInfo property)
//             {
//                 var getMethod = property.GetGetMethod(true);
//                 if (getMethod == null)
//                     return null;
//
//                 var call = Expression.Call(Expression.Convert(instance, property.DeclaringType!), getMethod);
//                 var convertResult = Expression.Convert(call, typeof(object));
//                 return Expression.Lambda<Func<object, object>>(convertResult, instance).Compile();
//             }
//
//             throw new NotSupportedException("Only FieldInfo and PropertyInfo are supported.");
//         }
//
//         private static Action<object, object> CreateSetter(MemberInfo member)
//         {
//             var instance = Expression.Parameter(typeof(object), "target");
//             var value = Expression.Parameter(typeof(object), "value");
//
//             if (member is FieldInfo field)
//             {
//                 var fieldAccess = Expression.Field(Expression.Convert(instance, field.DeclaringType!), field);
//                 var assign = Expression.Assign(fieldAccess, Expression.Convert(value, field.FieldType));
//                 return Expression.Lambda<Action<object, object>>(assign, instance, value).Compile();
//             }
//
//             if (member is PropertyInfo property)
//             {
//                 var setMethod = property.GetSetMethod(true);
//                 if (setMethod == null)
//                     return null;
//
//                 var call = Expression.Call(
//                     Expression.Convert(instance, property.DeclaringType!),
//                     setMethod,
//                     Expression.Convert(value, property.PropertyType));
//                 return Expression.Lambda<Action<object, object>>(call, instance, value).Compile();
//             }
//
//             throw new NotSupportedException("Only FieldInfo and PropertyInfo are supported.");
//         }
//     }
// }
