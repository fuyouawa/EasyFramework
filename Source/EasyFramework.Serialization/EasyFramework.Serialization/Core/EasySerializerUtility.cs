using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    internal static class EasySerializerUtility
    {
        private static bool s_initializedSerializers = false;

        private struct SerializerStore
        {
            public EasySerializerConfigAttribute Attribute { get; }
            public IEasySerializer Instance { get; }

            public SerializerStore(EasySerializerConfigAttribute attribute, IEasySerializer instance)
            {
                Attribute = attribute;
                Instance = instance;
            }
        }

        private struct GenericSerializerInfo
        {
            public EasySerializerConfigAttribute Attribute { get; }
            public Type SerializerType { get; }

            public GenericSerializerInfo(EasySerializerConfigAttribute attribute, Type serializerType)
            {
                Attribute = attribute;
                SerializerType = serializerType;
            }
        }

        /// <summary>
        /// 具体类型的序列化
        /// </summary>
        private static readonly Dictionary<Type, List<SerializerStore>> ConcreteSerializersDict =
            new Dictionary<Type, List<SerializerStore>>();

        /// <summary>
        /// 泛型序列化的信息存储
        /// </summary>
        private static readonly List<GenericSerializerInfo> GenericSerializerInfos = new List<GenericSerializerInfo>();

        private static void RegisterSerializerType(Type serializerType)
        {
            var interfaceType = serializerType.GetInterface("IEasySerializer`1");
            var argType = interfaceType.GetGenericArguments()[0];
            var attr = serializerType.GetCustomAttribute<EasySerializerConfigAttribute>();
            if (attr == null)
            {
                attr = new EasySerializerConfigAttribute();
            }

            if (!argType.IsGenericParameter)
            {
                if (!ConcreteSerializersDict.TryGetValue(argType, out var list))
                {
                    list = new List<SerializerStore>();
                    ConcreteSerializersDict[argType] = list;
                }

                var inst = (IEasySerializer)Activator.CreateInstance(serializerType);
                list.Add(new SerializerStore(attr, inst));
            }
            else
            {
                GenericSerializerInfos.Add(new GenericSerializerInfo(attr, serializerType));
            }
        }

        private static void ProcessSerializers()
        {
            foreach (var infos in ConcreteSerializersDict.Values)
            {
                infos.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
            }
        }

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

                ProcessSerializers();

                s_initializedSerializers = true;
            }
        }

        private static SerializerStore? FindSerializerInSerializersDict(Type valueType)
        {
            if (ConcreteSerializersDict.TryGetValue(valueType, out var infos))
            {
                foreach (var info in infos)
                {
                    if (info.Instance.CanSerialize(valueType))
                    {
                        return info;
                    }
                }
            }

            return null;
        }

        private static SerializerStore? FindSerializerInGenericSerializers(Type valueType)
        {
            var serializes = new List<SerializerStore>();
            foreach (var info in GenericSerializerInfos)
            {
                try
                {
                    var type = info.SerializerType.MakeGenericType(valueType);
                    var inst = (IEasySerializer)Activator.CreateInstance(type);
                    serializes.Add(new SerializerStore(info.Attribute, inst));
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            if (serializes.Count == 0)
            {
                return null;
            }

            serializes.Sort((a, b) => b.Attribute.Priority.CompareTo(a.Attribute.Priority));
            return serializes[0];
        }


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

            tmp = FindSerializerInGenericSerializers(valueType);
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
    }
}
