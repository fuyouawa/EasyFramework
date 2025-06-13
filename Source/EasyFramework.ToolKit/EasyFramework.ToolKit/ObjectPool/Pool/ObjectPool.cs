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
            if (instance.GetType() != ObjectType)
            {
                return false;
            }
            return base.CanRecycle(instance);
        }

        /// <inheritdoc />
        protected override void OnSpawn(object instance)
        {
            if (instance is IPooledObjectCallbackReceiver receiver)
            {
                receiver.OnSpawn(this);
            }
        }

        /// <inheritdoc />
        protected override void OnRecycle(object instance)
        {
            if (instance is IPooledObjectCallbackReceiver receiver)
            {
                receiver.OnRecycle(this);
            }
        }
    }
}
