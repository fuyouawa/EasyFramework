using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyFramework.Core
{
    public class GenericMethodMaker
    {
        private readonly MethodInfo _genericMethod;
        private readonly Dictionary<Type[], MethodInfo> _methodCacheByTypeArgs = new Dictionary<Type[], MethodInfo>();

        public GenericMethodMaker(MethodInfo genericMethod)
        {
            _genericMethod = genericMethod;
        }

        public MethodInfo Get(params Type[] typeArgs)
        {
            if (!_methodCacheByTypeArgs.TryGetValue(typeArgs, out var method))
            {
                method = _genericMethod.MakeGenericMethod(typeArgs);
                _methodCacheByTypeArgs[typeArgs] = method;
            }
            return method;
        }
    }
}
