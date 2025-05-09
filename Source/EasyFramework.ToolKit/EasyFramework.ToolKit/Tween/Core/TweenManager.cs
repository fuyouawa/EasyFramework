using EasyFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.ToolKit
{
    public class TweenManager : Singleton<TweenManager>
    {
        private readonly TypeMatcher _tweenerTypeMatcher = new TypeMatcher();

        TweenManager()
        {
        }

        protected override void OnSingletonInit()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.IsSubclassOf(typeof(AbstractTweener)))
                .ToArray();

            _tweenerTypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(AbstractTweener<>));
                return new TypeMatchIndex(type, 0, argType);
            }));

            _tweenerTypeMatcher.AddMatchRule(GetMatchedTweenerType);
        }

        private Type GetMatchedTweenerType(TypeMatchIndex matchIndex, Type[] targets, ref bool stopMatch)
        {
            var valueType = targets[0];
            var argType = matchIndex.Targets[0];

            // 如果参数不是泛型参数，并且是个不包含泛型参数的类型
            // 用于判断当前序列化器的参数必须是个具体类型
            if (!argType.IsGenericParameter && !argType.ContainsGenericParameters)
            {
                if (argType == valueType)
                {
                    stopMatch = true;
                    return matchIndex.Type;
                }

                return null;
            }

            var missingArgs = argType.ResolveMissingGenericTypeArguments(valueType, true);
            if (missingArgs.Length == 0)
                return null;

            try
            {
                var tweenerType = matchIndex.Type.MakeGenericType(missingArgs);
                stopMatch = true;
                return tweenerType;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private readonly Dictionary<Type, Type> _tweenerTypesByValueType = new Dictionary<Type, Type>();

        private Type GetTweenerType(Type valueType)
        {
            if (_tweenerTypesByValueType.TryGetValue(valueType, out var tweenerType))
            {
                return tweenerType;
            }

            var results = _tweenerTypeMatcher.Match(new[] { valueType });
            if (results.IsNotNullOrEmpty())
            {
                tweenerType = results[0].MatchedType;
            }

            _tweenerTypesByValueType[valueType] = tweenerType;
            return tweenerType;
        }

        public AbstractTweener GetTweener(Type valueType)
        {
            var tweener = GetTweenerType(valueType)?.CreateInstance<AbstractTweener>();
            return tweener;
        }

        public AbstractTweener<T> GetTweener<T>()
        {
            return (AbstractTweener<T>)GetTweener(typeof(T));
        }

        public TweenSequence GetSequence()
        {
            return new TweenSequence();
        }

        public TweenCallback GetCallback()
        {
            return new TweenCallback();
        }
    }
}
