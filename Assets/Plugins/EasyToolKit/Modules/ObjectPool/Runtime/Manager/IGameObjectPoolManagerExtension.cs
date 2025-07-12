namespace EasyToolKit.ToolKit
{
    using System;
    using UnityEngine;

    public static class IGameObjectPoolManagerExtension
    {
        public static IGameObjectPool GetPool<TObject>(this IGameObjectPoolManager manager, string poolName)
        {
            return manager.GetPool(poolName, typeof(TObject));
        }

        public static void CreatePool<TObject>(this IGameObjectPoolManager manager, string poolName, GameObject original)
        {
            manager.CreatePool(poolName, typeof(TObject), original);
        }

        public static IGameObjectPool GetOrCreatePool(this IGameObjectPoolManager manager, string poolName, Type objectType, GameObject original)
        {
            try
            {
                return manager.GetPool(poolName, objectType);
            }
            catch (InvalidOperationException)
            {
                manager.CreatePool(poolName, objectType, original);
                return manager.GetPool(poolName, objectType);
            }
        }

        public static IGameObjectPool GetOrCreatePool<TObject>(this IGameObjectPoolManager manager, string poolName, GameObject original)
        {
            return manager.GetOrCreatePool(poolName, typeof(TObject), original);
        }
    }
}
