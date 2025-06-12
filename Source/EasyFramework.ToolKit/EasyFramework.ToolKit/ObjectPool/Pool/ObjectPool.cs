using System;
using Cysharp.Threading.Tasks;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 通用对象池实现，用于管理实现了IPooledObject接口的对象
    /// </summary>
    public class ObjectPool : ObjectPoolBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="objectType">对象池中存储的对象类型</param>
        /// <exception cref="InvalidOperationException">
        /// 当objectType未实现IPooledObject接口，
        /// 或者是接口、抽象类、泛型类型时抛出
        /// </exception>
        public ObjectPool(string name, Type objectType)
            : base(name, objectType)
        {
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

        /// <inheritdoc />
        protected override object GetNewObject()
        {
            return Activator.CreateInstance(ObjectType);
        }

        /// <inheritdoc />
        protected override async UniTask<object> GetNewObjectAsync()
        {
            return GetNewObject();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override void OnSpawn(object instance)
        {
            var obj = (IPooledObject)instance;
            obj.OwningPool = this;
            obj.OnSpawn();
        }

        /// <inheritdoc />
        protected override void OnRecycle(object instance)
        {
            var obj = (IPooledObject)instance;
            obj.OwningPool = this;
            obj.OnRecycle();
        }
    }
}
