using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyFramework
{
    public class GenericMethodMaker
    {
        private readonly MethodInfo _genericMethod;
        private readonly Dictionary<Type[], MethodInfo> _methodCache = new Dictionary<Type[], MethodInfo>();

        public GenericMethodMaker(MethodInfo genericMethod)
        {
            _genericMethod = genericMethod;
        }

        public MethodInfo Get(params Type[] genericTypes)
        {
            if (!_methodCache.TryGetValue(genericTypes, out var method))
            {
                method = _genericMethod.MakeGenericMethod(genericTypes);
                _methodCache[genericTypes] = method;
            }
            return method;
        }
    }
}
