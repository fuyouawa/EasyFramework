using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Core
{
    public class DiContainer
    {
        private readonly Dictionary<Type, object> _instancesByType = new Dictionary<Type, object>();

        public void Bind<T>(T instance)
        {
            Bind(instance, typeof(T));
        }

        public void Bind(object instance, Type type)
        {
            _instancesByType[type] = instance;
        }

        public bool Unbind<T>()
        {
            return Unbind(typeof(T));
        }

        public bool Unbind(Type type)
        {
            return _instancesByType.Remove(type);
        }

        public T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }

        public object Resolve(Type type)
        {
            return _instancesByType.GetValueOrDefault(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            var type = typeof(T);
            return _instancesByType.Values.Where(instance => type.IsInstanceOfType(instance)).Cast<T>();
        }

        public void Clear() => _instancesByType.Clear();
    }
}
