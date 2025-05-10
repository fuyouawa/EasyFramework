using EasyFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.ToolKit
{
    internal class TweenManager : Singleton<TweenManager>
    {
        private readonly TypeMatcher _tweenerProcessorTypeMatcher = new TypeMatcher();

        TweenManager()
        {
        }

        protected override void OnSingletonInit()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract &&
                            t.HasInterface(typeof(ITweenerProcessor)))
                .ToArray();

            _tweenerProcessorTypeMatcher.SetTypeMatchIndices(types.Select(type =>
            {
                var argType = type.GetArgumentsOfInheritedOpenGenericType(typeof(AbstractTweenerProcessor<,>));
                return new TypeMatchIndex(type, 0, argType);
            }));
        }

        private readonly Dictionary<(Type, Type), Type> _tweenerProcessorTypesByValueType = new Dictionary<(Type, Type), Type>();

        private Type GetTweenerProcessorType(Type valueType, Type effectConfigType)
        {
            var key = (valueType, effectConfigType);
            if (_tweenerProcessorTypesByValueType.TryGetValue(key, out var tweenerType))
            {
                return tweenerType;
            }

            var results = _tweenerProcessorTypeMatcher.Match(new[] { valueType, effectConfigType });
            if (results.IsNotNullOrEmpty())
            {
                tweenerType = results[0].MatchedType;
            }

            _tweenerProcessorTypesByValueType[key] = tweenerType;
            return tweenerType;
        }

        public ITweenerProcessor GetTweenerProcessor(Type valueType, Type effectConfigType)
        {
            var processor = GetTweenerProcessorType(valueType, effectConfigType)?.CreateInstance<ITweenerProcessor>();
            if (processor == null || !processor.CanProcess(valueType))
                return null;
            return processor;
        }

        public TweenSequence GetSequence()
        {
            return new TweenSequence();
        }

        public Tweener GetTweener()
        {
            return new Tweener();
        }

        public TweenCallback GetCallback()
        {
            return new TweenCallback();
        }
    }
}
