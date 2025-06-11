using System;
using Cysharp.Threading.Tasks;

namespace EasyFramework.Core
{
    public class ObjectPool : ObjectPoolBase
    {
        public ObjectPool(string name, Type objectType)
            : base(name, objectType)
        {
            // 检查 objectType 是否实现 IPooledObject 接口
            if (!typeof(IPooledObject).IsAssignableFrom(objectType))
            {
                throw new InvalidOperationException($"Type '{objectType}' must implement '{typeof(IPooledObject)}'.");
            }

            // 检查 objectType 是否可被实例化
            if (objectType.IsInterface || objectType.IsAbstract || objectType.IsGenericType)
            {
                throw new InvalidOperationException($"Type '{objectType}' cannot be interface, abstract or generic type.");
            }
        }

        protected override object GetNewObject()
        {
            return Activator.CreateInstance(ObjectType);
        }

        protected override async UniTask<object> GetNewObjectAsync()
        {
            return GetNewObject();
        }

        protected override bool CanRecycle(object instance)
        {
            var obj = (IPooledObject)instance;
            if (!ReferenceEquals(obj.OwningPool, this))
            {
                return false;
            }

            if (instance.GetType() != ObjectType)
            {
                return false;
            }
            return base.CanRecycle(instance);
        }

        protected override void OnSpawn(object instance)
        {
            var obj = (IPooledObject)instance;
            obj.OwningPool = this;
            obj.OnSpawn();
        }

        protected override void OnRecycle(object instance)
        {
            var obj = (IPooledObject)instance;
            obj.OnRecycle();
        }
    }
}
