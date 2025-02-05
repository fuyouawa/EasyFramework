using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework
{
    public class DiContainer
    {
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        public void Bind<T>(T instance)
        {
            var key = typeof(T);

            if (_instances.ContainsKey(key))
            {
                _instances[key] = instance;
            }
            else
            {
                _instances.Add(key, instance);
            }
        }

        public T Resolve<T>() where T : class
        {
            var key = typeof(T);

            if (_instances.TryGetValue(key, out var retInstance))
            {
                return retInstance as T;
            }

            return null;
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            var type = typeof(T);
            return _instances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => _instances.Clear();
    }
}
