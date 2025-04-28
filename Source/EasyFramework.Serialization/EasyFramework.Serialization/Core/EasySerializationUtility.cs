using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    internal readonly struct SerializerStore : IEquatable<SerializerStore>
    {
        public EasySerializerConfigAttribute Attribute { get; }
        public IEasySerializer Instance { get; }

        public SerializerStore(EasySerializerConfigAttribute attribute, IEasySerializer instance)
        {
            Attribute = attribute;
            Instance = instance;
        }

        public bool Equals(SerializerStore other)
        {
            return Equals(Instance, other.Instance);
        }

        public override bool Equals(object obj)
        {
            return obj is SerializerStore other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Instance != null ? Instance.GetHashCode() : 0);
        }
    }

    internal readonly struct GenericSerializerInfo : IEquatable<GenericSerializerInfo>
    {
        public EasySerializerConfigAttribute Attribute { get; }
        public Type SerializerType { get; }
        public Type ArgType { get; }

        public GenericSerializerInfo(EasySerializerConfigAttribute attribute, Type serializerType, Type argType)
        {
            Attribute = attribute;
            SerializerType = serializerType;
            ArgType = argType;
        }

        public bool Equals(GenericSerializerInfo other)
        {
            return SerializerType == other.SerializerType;
        }

        public override bool Equals(object obj)
        {
            return obj is GenericSerializerInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (SerializerType != null ? SerializerType.GetHashCode() : 0);
        }
    }

    public static class EasySerializationUtility
    {
        #region Initialization

        /// <summary>
        /// 具体类型的序列化
        /// </summary>
        private static readonly Dictionary<Type, HashSet<Type>> ConcreteSerializerTypesDict =
            new Dictionary<Type, HashSet<Type>>();

        /// <summary>
        /// 泛型序列化的信息存储
        /// </summary>
        private static readonly HashSet<GenericSerializerInfo> GenericSerializerInfos =
            new HashSet<GenericSerializerInfo>();

        private static void RegisterConcreteSerializerType(Type serializerType, Type argType)
        {
            if (!ConcreteSerializerTypesDict.TryGetValue(argType, out var list))
            {
                list = new HashSet<Type>();
                ConcreteSerializerTypesDict[argType] = list;
            }

            var suc = list.Add(serializerType);
            Debug.Assert(suc);
        }

        private static void RegisterGenericSerializerType(Type serializerType, Type argType)
        {
            var attr = serializerType.GetCustomAttribute<EasySerializerConfigAttribute>() ??
                       new EasySerializerConfigAttribute();
            var suc = GenericSerializerInfos.Add(new GenericSerializerInfo(attr, serializerType, argType));
            Debug.Assert(suc);
        }

        private static void RegisterSerializerType(Type serializerType)
        {
            var interfaceType = serializerType.GetInterface("IEasySerializer`1");
            var argType = interfaceType.GetGenericArguments()[0];

            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                RegisterConcreteSerializerType(serializerType, argType);
            }
            else
            {
                RegisterGenericSerializerType(serializerType, argType);
            }
        }

        private static bool s_initializedSerializers = false;

        private static void EnsureInitializeSerializers()
        {
            if (!s_initializedSerializers)
            {
                var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(asm => asm.GetTypes())
                    .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                                t.GetInterface("IEasySerializer`1") != null)
                    .ToArray();

                foreach (var type in types)
                {
                    RegisterSerializerType(type);
                }

                s_initializedSerializers = true;
            }
        }

        #endregion

        #region FindSerializer

        private static readonly Dictionary<Type, List<SerializerStore>> ConcreteSerializerCachesDict =
            new Dictionary<Type, List<SerializerStore>>();

        private static SerializerStore? FindSerializerInSerializersDict(Type valueType)
        {
            if (!ConcreteSerializerCachesDict.TryGetValue(valueType, out var list))
            {
                if (ConcreteSerializerTypesDict.TryGetValue(valueType, out var types))
                {
                    list = new List<SerializerStore>();

                    foreach (var type in types)
                    {
                        var attr = type.GetCustomAttribute<EasySerializerConfigAttribute>() ??
                                   new EasySerializerConfigAttribute();
                        var inst = (IEasySerializer)Activator.CreateInstance(type);
                        list.Add(new SerializerStore(attr, inst));
                    }

                    list.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
                    ConcreteSerializerCachesDict[valueType] = list;
                }
            }

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (item.Instance.CanSerialize(valueType))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        private static Type[] GetNeededGenericTypes(Type srcType, Type destType, bool allocInherit)
        {
            if (srcType.IsArray != destType.IsArray ||
                srcType.IsSZArray != destType.IsSZArray)
            {
                return new Type[] { };
            }

            if (destType.IsArray)
            {
                return new[] { destType.GetElementType() };
            }

            Type[] typeArgs;
            if (srcType.IsGenericType && destType.IsGenericType)
            {
                var srcArg = srcType.GenericTypeArguments;
                var destArg = destType.GenericTypeArguments;
                if (destArg.Length != srcArg.Length)
                    return new Type[] { };

                var srcDef = srcType.GetGenericTypeDefinition();
                var destDef = destType.GetGenericTypeDefinition();
                if (srcDef != destDef)
                {
                    if (!allocInherit || !TypeUtility.IsDerivedOrImplementsGeneric(destDef, srcType))
                        return new Type[] { };
                }

                typeArgs = srcArg.Select((t, i) => (t, i))
                    .Where(x => x.t.IsGenericParameter)
                    .Select(x => destArg[x.i])
                    .ToArray();
            }
            else
            {
                typeArgs = new[] { destType };
            }

            return typeArgs;
        }

        private static SerializerStore? FindSerializerInGenericSerializers(Type valueType, int? lastMaxPriority)
        {
            var serializes = new List<SerializerStore>();
            foreach (var info in GenericSerializerInfos)
            {
                if (lastMaxPriority.HasValue)
                {
                    if (info.Attribute.Priority < lastMaxPriority.Value)
                        continue;
                }

                if (info.ArgType.IsGenericType && !valueType.IsGenericType)
                {
                    continue;
                }

                var needTypes = GetNeededGenericTypes(info.ArgType, valueType, info.Attribute.AllowInherit);
                if (needTypes.Length == 0)
                    continue;

                Type type;
                try
                {
                    type = info.SerializerType.MakeGenericType(needTypes);
                }
                catch (Exception e)
                {
                    continue;
                }

                var inst = (IEasySerializer)Activator.CreateInstance(type);
                serializes.Add(new SerializerStore(info.Attribute, inst));
            }

            if (serializes.Count == 0)
            {
                return null;
            }

            serializes.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
            return serializes[0];
        }

        #endregion

        private static readonly Dictionary<Type, SerializerStore> SerializerCache =
            new Dictionary<Type, SerializerStore>();

        private static SerializerStore? InternalGetSerializer(Type valueType)
        {
            EnsureInitializeSerializers();

            if (SerializerCache.TryGetValue(valueType, out var serializer))
            {
                return serializer;
            }

            var sortList = new List<SerializerStore>();
            var tmp = FindSerializerInSerializersDict(valueType);
            if (tmp.HasValue)
            {
                sortList.Add(tmp.Value);
            }

            tmp = FindSerializerInGenericSerializers(valueType, tmp?.Attribute.Priority);
            if (tmp.HasValue)
            {
                sortList.Add(tmp.Value);
            }

            if (sortList.Count == 0)
            {
                return null;
            }

            sortList.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));

            serializer = sortList[0];
            SerializerCache[valueType] = serializer;
            return serializer;
        }

        public static EasySerializer GetSerializer(Type valueType)
        {
            var info = InternalGetSerializer(valueType);
            if (info.HasValue)
            {
                return (EasySerializer)info.Value.Instance;
            }

            return null;
        }

        public static EasySerializer<T> GetSerializer<T>()
        {
            var serializer = GetSerializer(typeof(T));
            return (EasySerializer<T>)serializer;
        }

        public static void AutoCopy(object source, object destination)
        {
            var type = source.GetType();
            if (type != destination.GetType())
            {
                throw new ArgumentException(
                    $"Both sides of the automatic copy must be of the same type. " +
                    $"Source is '{type.FullName}', but destination is '{destination.GetType().FullName}')");
            }

            var members = MembersGetterPresets.Default(type);
            foreach (var member in members)
            {
                var sourceValue = ReflectionUtility.GetMemberValue(member, source);
                ReflectionUtility.SetMemberValue(member, destination, sourceValue);
            }
        }
    }
}
