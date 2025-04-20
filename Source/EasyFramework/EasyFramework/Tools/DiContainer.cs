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
            Bind(instance, typeof(T));
        }

        public void Bind(object instance, Type type)
        {
            if (_instances.ContainsKey(type))
            {
                _instances[type] = instance;
            }
            else
            {
                _instances.Add(type, instance);
            }
        }

        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public object Resolve(Type type)
        {
            return _instances.GetValueOrDefault(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            var type = typeof(T);
            return _instances.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => _instances.Clear();
    }
}
