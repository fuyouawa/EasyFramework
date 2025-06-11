using System;

namespace EasyFramework.Core
{
    public static class IObjectPoolExtension
    {
        public static TObjectPool OnSpawn<TObjectPool>(this TObjectPool objectPool, Action<object> callback)
            where TObjectPool : IObjectPool
        {
            objectPool.AddSpawnCallback(callback);
            return objectPool;
        }

        public static TObjectPool OnRecycle<TObjectPool>(this TObjectPool objectPool, Action<object> callback)
            where TObjectPool : IObjectPool
        {
            objectPool.AddRecycleCallback(callback);
            return objectPool;
        }

        public static TObjectPool SetCapacity<TObjectPool>(this TObjectPool objectPool, int newCapacity)
            where TObjectPool : IObjectPool
        {
            objectPool.Capacity = newCapacity;
            return objectPool;
        }

        public static TObject TrySpawn<TObject>(this IObjectPool objectPool)
        {
            var obj = objectPool.TrySpawn();
            if (obj is TObject o)
            {
                return o;
            }
            else
            {
                // 泛型参数类型与对象池类型不兼容，抛出异常
                throw new ArgumentException($"Generic type '{typeof(TObject)}' is not assignable from object pool type '{objectPool.ObjectType}'.");
            }
        }
    }
}
