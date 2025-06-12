namespace EasyFramework.ToolKit
{
    using System;
    using UnityEngine;

    public static class IUnityObjectPoolManagerExtension
    {
        public static IUnityObjectPool TryGetPool<TObject>(this IUnityObjectPoolManager manager, string poolName)
            where TObject : Component
        {
            return manager.TryGetPool(poolName, typeof(TObject));
        }

        public static bool TryAllocatePool<TObject>(this IUnityObjectPoolManager manager, string poolName, GameObject original)
            where TObject : Component
        {
            return manager.TryAllocatePool(poolName, typeof(TObject), original);
        }

        public static IUnityObjectPool TryGetOrAllocatePool(this IUnityObjectPoolManager manager, string poolName, Type objectType, GameObject original)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                if (manager.TryAllocatePool(poolName, objectType, original))
                {
                    pool = manager.TryGetPool(poolName, objectType);
                }
            }
            return pool;
        }

        public static IUnityObjectPool TryGetOrAllocatePool<TObject>(this IUnityObjectPoolManager manager, string poolName, GameObject original)
            where TObject : Component
        {
            var pool = manager.TryGetPool<TObject>(poolName);
            if (pool == null)
            {
                if (manager.TryAllocatePool<TObject>(poolName, original))
                {
                    pool = manager.TryGetPool<TObject>(poolName);
                }
            }
            return pool;
        }
    }
}
