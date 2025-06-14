namespace EasyFramework.ToolKit
{
    using System;
    using UnityEngine;

    public static class IGameObjectPoolManagerExtension
    {
        public static IGameObjectPool TryGetPool<TObject>(this IGameObjectPoolManager manager, string poolName)
        {
            return manager.TryGetPool(poolName, typeof(TObject));
        }

        public static IGameObjectPool GetPool(this IGameObjectPoolManager manager, string poolName, Type objectType)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                throw new InvalidOperationException(
                    $"Game object pool with name '{poolName}' and object type '{objectType}' does not exist.");
            }
            return pool;
        }

        public static void CreatePool(this IGameObjectPoolManager manager, string poolName, Type objectType, GameObject original)
        {
            if (!manager.TryCreatePool(poolName, objectType, original))
            {
                throw new InvalidOperationException(
                    $"Failed to create game object pool '{poolName}' with object type '{objectType}'. It may already exist.");
            }
        }

        public static IGameObjectPool GetOrCreatePool(this IGameObjectPoolManager manager, string poolName, Type objectType, GameObject original)
        {
            var pool = manager.TryGetPool(poolName, objectType);
            if (pool == null)
            {
                manager.CreatePool(poolName, objectType, original);
                pool = manager.GetPool(poolName, objectType);
            }
            return pool;
        }

        public static IGameObjectPool GetOrCreatePool<TObject>(this IGameObjectPoolManager manager, string poolName, GameObject original)
        {
            return manager.GetOrCreatePool(poolName, typeof(TObject), original);
        }

        public static IGameObjectPool GetPool<TObject>(this IGameObjectPoolManager manager, string poolName)
        {
            return manager.GetPool(poolName, typeof(TObject));
        }

        public static void CreatePool<TObject>(this IGameObjectPoolManager manager, string poolName, GameObject original)
        {
            manager.CreatePool(poolName, typeof(TObject), original);
        }
    }
}
