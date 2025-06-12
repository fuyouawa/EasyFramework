using System;
using Cysharp.Threading.Tasks;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 通用对象池实现，用于管理实现了IPooledObject接口的对象
    /// </summary>
    public class ObjectPool : ObjectPoolBase
    {
        /// <inheritdoc />
        protected override object GetNewObject()
        {
            return Activator.CreateInstance(ObjectType);
        }

        // /// <inheritdoc />
        // protected override async UniTask<object> GetNewObjectAsync()
        // {
        //     return GetNewObject();
        // }

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
