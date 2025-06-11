using System;

namespace EasyFramework.Core
{
    public static class IObjectPoolManagerExtension
    {
        public static IObjectPool TryGetPool<TObject>(this IObjectPoolManager manager, string poolName)
            where TObject : IPooledObject
        {
            return manager.TryGetPool(poolName, typeof(TObject));
        }

        public static bool TryAllocatePool<TObject>(this IObjectPoolManager manager, string poolName)
            where TObject : IPooledObject
        {
            return manager.TryAllocatePool(poolName, typeof(TObject));
        }

        public static IObjectPool TryGetOrAllocatePool(this IObjectPoolManager manager, string poolName, Type objectType)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                if (manager.TryAllocatePool(poolName, objectType))
                {
                    pool = manager.TryGetPool(poolName, objectType);
                }
            }
            return pool;
        }

        public static IObjectPool TryGetOrAllocatePool<TObject>(this IObjectPoolManager manager, string poolName)
            where TObject : IPooledObject
        {
            return manager.TryGetOrAllocatePool(poolName, typeof(TObject));
        }
    }
}
