using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace EasyFramework.Serialization
{
    public static class EasySerializerUtility
    {
        private static RegisterEasySerializerAttribute[] s_registerSerializerAttributes;

        private static RegisterEasySerializerAttribute[] GetRegisterSerializerAttributes()
        {
            if (s_registerSerializerAttributes == null)
            {
                s_registerSerializerAttributes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetCustomAttributes(typeof(RegisterEasySerializerAttribute), true))
                    .Select(attr => (RegisterEasySerializerAttribute)attr)
                    .ToArray();
            }

            return s_registerSerializerAttributes;
        }

        private struct ImplementationType
        {
            public struct ConstraintDefine
            {
                public Type[] Types;
                public bool HasClass;
                public bool HasStruct;
                public bool HasNew;
            }

            public Type Type;
            public ConstraintDefine Constraint;

            public bool IsUnconstrainedGeneric()
            {
                return !Constraint.Types.Any()
                       && Constraint is { HasClass: false, HasStruct: false, HasNew: false };
            }
        }

        private static ImplementationType GetSerializerImplementationType(Type serializerType)
        {
            // 获取接口类型
            var interfaceType = serializerType.GetInterfaces()
                .First(i => i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IEasySerializer<>));
            // 获取接口的泛型参数
            var implArg = interfaceType.GetGenericArguments()[0];
            var argAttr = implArg.GenericParameterAttributes;
            return new ImplementationType()
            {
                Type = implArg,
                Constraint = new ImplementationType.ConstraintDefine()
                {
                    Types = implArg.GetGenericParameterConstraints(),
                    HasClass = argAttr.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint),
                    HasNew = argAttr.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint),
                    HasStruct = argAttr.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint)
                }
            };
        }

        private static bool s_initializedSerializers = false;

        private struct SerializerInfo
        {
            public RegisterEasySerializerAttribute Attribute;
            public IEasySerializer Instance;
        }

        /// <summary>
        /// 具体类型的序列化
        /// </summary>
        private static readonly Dictionary<Type, List<SerializerInfo>> ConcreteSerializersDict =
            new Dictionary<Type, List<SerializerInfo>>();

        /// <summary>
        /// 无约束泛型序列化
        /// </summary>
        private static readonly List<SerializerInfo> UnconstrainedGenericSerializers = new List<SerializerInfo>();

        private static void RegisterSerializer(IEasySerializer serializer, RegisterEasySerializerAttribute attribute)
        {
            var info = new SerializerInfo()
            {
                Attribute = attribute,
                Instance = serializer,
            };

            var implType = GetSerializerImplementationType(serializer.GetType());

            if (!implType.Type.IsGenericType)
            {
                if (!ConcreteSerializersDict.TryGetValue(implType.Type, out var list))
                {
                    list = new List<SerializerInfo>();
                    ConcreteSerializersDict[implType.Type] = list;
                }

                list.Add(info);
            }
            else
            {
                if (implType.IsUnconstrainedGeneric())
                {
                    UnconstrainedGenericSerializers.Add(info);
                }
                else
                {
                    throw new NotImplementedException(
                        $"The constrained generic serializer '{implType.Type.FullName}' is not implemented!");
                }
            }
        }

        private static void PostProcessSerializers()
        {
            foreach (var infos in ConcreteSerializersDict.Values)
            {
                infos.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
            }

            UnconstrainedGenericSerializers.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
        }

        private static void EnsureInitializeSerializers()
        {
            if (!s_initializedSerializers)
            {
                var attrs = GetRegisterSerializerAttributes();
                foreach (var attr in attrs)
                {
                    var inst = (IEasySerializer)Activator.CreateInstance(attr.SerializerType);
                    RegisterSerializer(inst, attr);
                }

                PostProcessSerializers();

                s_initializedSerializers = true;
            }
        }

        private static SerializerInfo? InternalGetSerializer(Type valueType)
        {
            EnsureInitializeSerializers();

            SerializerInfo? ret = null;

            // 先从具体类型中查找序列化器
            if (ConcreteSerializersDict.TryGetValue(valueType, out var infos))
            {
                foreach (var info in infos)
                {
                    if (info.Instance.CanSerialize(valueType))
                    {
                        ret = info;
                        break;
                    }
                }
            }

            // 然后从无约束泛型中查找序列化器
            foreach (var info in UnconstrainedGenericSerializers)
            {
                if (info.Instance.CanSerialize(valueType))
                {
                    // 如果优先级大于先前找到的序列化器，则更新
                    if (ret.HasValue && info.Attribute.Priority > ret.Value.Attribute.Priority)
                    {
                        ret = info;
                    }

                    break;
                }
            }

            return ret;
        }

        internal static IEasySerializer GetSerializer(Type valueType)
        {
            var info = InternalGetSerializer(valueType);
            if (info.HasValue)
            {
                return info.Value.Instance;
            }

            return null;
        }

        public static IEasySerializer<T> GetSerializer<T>()
        {
            var serializer = GetSerializer(typeof(T));
            return (IEasySerializer<T>)serializer;
        }
    }
}
