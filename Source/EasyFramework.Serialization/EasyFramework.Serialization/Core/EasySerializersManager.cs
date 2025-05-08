using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    public class EasySerializersManager : Singleton<EasySerializersManager>
    {
        private readonly TypeMatcher _serializerTypeMatcher = new TypeMatcher();

        EasySerializersManager()
        {
        }

        protected override void OnSingletonInit()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.HasInterface(typeof(IEasySerializer)))
                .ToArray();

            _serializerTypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var config = type.GetCustomAttribute<EasySerializerConfigAttribute>();
                config ??= EasySerializerConfigAttribute.Default;
                    
                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(EasySerializer<>));
                return new TypeMatchIndex(type, config.Priority, argType);
            }));

            _serializerTypeMatcher.AddMatchRule(GetMatchedSerializerType);
        }

        private Type GetMatchedSerializerType(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            var valueType = targets[0];
            var argType = matchIndex.Targets[0];

            // 如果参数不是泛型参数，并且是个不包含泛型参数的类型
            // 用于判断当前序列化器的参数必须是个具体类型
            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                if (argType == valueType)
                {
                    return matchIndex.Type;
                }

                return null;
            }

            var config = matchIndex.Type.GetCustomAttribute<EasySerializerConfigAttribute>();
            config ??= EasySerializerConfigAttribute.Default;

            var missingArgs = argType.ResolveMissingGenericTypeArguments(valueType, config.AllowInherit);
            if (missingArgs.Length == 0)
                return null;

            try
            {
                var serializeType = matchIndex.Type.MakeGenericType(missingArgs);
                return serializeType;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private readonly Dictionary<Type, IEasySerializer> _serializerCacheByValueType =
            new Dictionary<Type, IEasySerializer>();

        internal IEasySerializer GetSerializer(Type valueType)
        {
            if (_serializerCacheByValueType.TryGetValue(valueType, out var serializer))
            {
                return serializer;
            }

            var results = _serializerTypeMatcher.Match(new[] { valueType });
            foreach (var result in results)
            {
                var inst = result.MatchedType.CreateInstance<IEasySerializer>();
                if (inst.CanSerialize(valueType))
                {
                    serializer = inst;
                    break;
                }
            }
            
            _serializerCacheByValueType[valueType] = serializer;
            return serializer;
        }

        public EasySerializer<T> GetSerializer<T>()
        {
            var serializer = GetSerializer(typeof(T));
            return (EasySerializer<T>)serializer;
        }

        public void ClearCache()
        {
            _serializerCacheByValueType.Clear();
        }
    }
}
